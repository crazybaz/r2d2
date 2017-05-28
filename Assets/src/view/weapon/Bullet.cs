using System;
using strange.extensions.mediation.impl;
using UnityEngine;

public class Bullet : EventView
{
    [Inject]
    public GameLogic GameLogic { get; set; }

    public BulletType Type;
    public GameObject DestroyFx;

    [NonSerialized] public BulletConfig Config;
    [NonSerialized] public float Speed;
    [NonSerialized] public float Damage;

    // params from weapon
    [NonSerialized] public WeaponOwnerType WeaponOwnerType;
    [NonSerialized] public float WeaponMultiplier = 1f;
    [NonSerialized] public MovingConfig MovingConfig;

    // CAUTION WITH CHANGING THIS PROPERTIES
    [NonSerialized] public bool DestroyOutsideBounds = true;

    private float lifeTime;
    private bool isLifeTime;

    protected CapsuleCollider damageCollider; // для поражающих элементов, у которых есть коллайдер аффекта/взрыва (он как правило больше, чем коллайдер активации/детонации)
    protected bool startExplode;

    private const float RAYCAST_DISTANCE = 500.0f;
    private static Transform CAMERA_TRANSFORM;

    public void Init(BulletConfig config, float damageMultiplier = 1f)
    {
        if (CAMERA_TRANSFORM == null)
            CAMERA_TRANSFORM = Camera.main.transform;

        Config = config;

        Speed = Config.BaseSpeed * WeaponMultiplier;
        Damage = Config.BaseDamage * WeaponMultiplier * damageMultiplier;

        damageCollider = gameObject.GetComponent<CapsuleCollider>();

        if (Config.ActivationRadius > 0)
        {
            var activationCollider = GetComponent<BoxCollider>();
            activationCollider.size = new Vector3(Config.ActivationRadius, activationCollider.size.y, Config.ActivationRadius);
        }

        lifeTime = Config.LifeTime;
        isLifeTime = lifeTime > 0;

        if (MovingConfig.MovingType == MovingType.None)
            return;

        // fill moving config
        MovingConfig.Speed = Speed;

        if (MovingConfig.Distance == 0) // can be allready filled
            MovingConfig.Distance = Config.Distance;

        MovingConfig.TargetKind = WeaponOwnerType == WeaponOwnerType.Enemy ? PawnKind.Paladin : PawnKind.Enemy;

        var mover = GetComponent<MovingController>();
        if (mover == null)
            throw new MissingComponentException("MovingController is missing");

        mover.Init(GameLogic, MovingConfig, Config);
    }

    private void FixedUpdate()
    {
        if (Damage <= 0)
        {
            Explode(true);
            return;
        }

        if (!startExplode)
        {
            if (WeaponOwnerType == WeaponOwnerType.Paladin && GameLogic.isAnyFlyingPawn)
                LookingForCollisionUpdate();
            return;
        }

        if (damageCollider.enabled == false)
            damageCollider.enabled = true;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (isLifeTime)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                Explode(true);
                return;
            }
        }

        if (DestroyOutsideBounds && global::Config.I.GAMEPLAY_BOUNDS.IsOutside(gameObject.transform.position))
            Explode();
    }

    /// <summary>
    /// Используется только для пуль игрока (паладина), обрабатываются только высоко летящие цели PawnKind.FlyingPawn
    /// Обрабатывает попала ли пуля во врага вне зависимости от того на какой высоте находится враг
    /// <summary>
    private void LookingForCollisionUpdate()
    {
        // not inited
        if (Config == null)
            return;

        RaycastHit hit;
        Physics.Raycast(CAMERA_TRANSFORM.position, transform.position-CAMERA_TRANSFORM.position, out hit, RAYCAST_DISTANCE, global::Config.I.PAWN_MASK);

        if (hit.collider == null) return;

        var pawn = hit.collider.GetComponent<IPawn>();

        if (pawn == null)
            return;

        // обрабатываем только высоко летящих pawn
        if (pawn != null && pawn.Kind == PawnKind.FlyingEnemy)
        {
            // TODO: refactor перенести это на паладина, сделать применение стана по колбеку

            // try activate all buffs from bullet
            // var buffs = ComponentExtention.CopyComponents<BuffView>(gameObject, (pawn as Component).gameObject);

            // foreach (var buff in buffs)
            // buff.TryActivate();

            if (damageCollider == null)
            {
                Hit(pawn);
                Explode(true);
            }
            else if (damageCollider.enabled)
            {
                Hit(pawn);
            }
            else
            {
                Explode(true);
            }
        }
    }

    /// <summary>
    /// Используется в данный момент для пуль вражеских pawn и пуль паладина по наземным и низколетящим pawn, а также по пулям
    /// Обрабатывает попала ли пуля в паладина
    /// <summary>
    private void OnTriggerEnter(Collider other)
    {
        // если не проинициализован, выходим
        if (Config == null)
            return;

        var pawn = other.GetComponent<IPawn>();
        var bullet = other.GetComponent<Bullet>();
        //var paladin = other.GetComponent<PaladinView>(); // TODO: refactor

        if (pawn == null && bullet == null)
        {
            // стандартная пуля бороздить своим большим коллайдером может все, что угодноб а аффектит только то, что должна
            // а вот нестандартная наподобие гранаты, будет взрываться от соприкосновения с чем угодно
            if (Config.ExplodeOnAnyCollision)
                Explode(true);

            return;
        }

        // pawn collision
        if (pawn != null)
        {
            // высоко летящих pawn мы не обстреливаем коллайдером, а обстреливаем камерой
            if (pawn.Kind == PawnKind.FlyingEnemy)
                return;

            // obstacle hit
            if (pawn.Kind == PawnKind.Obstacle)
            {
                // player bullet hit obstacle
                if (WeaponOwnerType == WeaponOwnerType.Paladin)
                {
                    switch (Config.ObstacleCollisionType)
                    {
                        case ObstacleCollisionType.Ignore:
                            break;
                        case ObstacleCollisionType.Collision:
                            Hit(pawn);
                            Explode(true);
                            break;
                        case ObstacleCollisionType.ReduceDamage:
                            Damage -= GameLogic.GetPawnModel(pawn as Pawn).Health;
                            Hit(pawn);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    // enemy bullet ignores obstacles
                    return;
                }
            }
            // enemy to player cross hit
            else if (pawn.Kind == PawnKind.Enemy && WeaponOwnerType == WeaponOwnerType.Paladin  ||  pawn.Kind == PawnKind.Paladin && WeaponOwnerType == WeaponOwnerType.Enemy)
            {
                // TODO: refactor перенести это на паладина, сделать применение стана по колбеку

                // try activate all buffs from bullet
                // var buffs = ComponentExtention.CopyComponents<BuffView>(gameObject, (pawn as Component).gameObject);

                // foreach (var buff in buffs)
                    // buff.TryActivate();

                if (damageCollider == null)
                {
                    Hit(pawn);
                    Explode(true);
                }
                else if (damageCollider.enabled)
                {
                    Hit(pawn);
                }
                else
                {
                    Explode(true);
                }

                // if still alive TODO: подозреваю, что легаси
                // if (!pawn.Disabled)
                // {
                // }
            }
        }

        // bullet collision
        if (bullet != null && Config.Destroyable && WeaponOwnerType != bullet.WeaponOwnerType)
        {
            Explode(true);
        }
    }

    protected virtual void Explode(bool playFx = false)
    {
        if (damageCollider != null)
        {
            if (startExplode) return;

            damageCollider.radius = Config.DamageRadius;
            startExplode = true;
        }

        if (playFx && DestroyFx != null)
            Instantiate(DestroyFx, transform.position, transform.rotation);

        if (damageCollider == null)
            Destroy(gameObject);
    }

    public event EnemyHitHandler EnemyHitEvent;
    public delegate void EnemyHitHandler(IPawn pawn);

    private void Hit(IPawn victim)
    {
        if (victim.Kind == PawnKind.Enemy && EnemyHitEvent != null)
            EnemyHitEvent.Invoke(victim);

        if (victim != null && victim.dispatcher != null)
            victim.dispatcher.Dispatch(ViewEvent.BULLET_HIT, Damage);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Config = null;
        DestroyFx = null;
        damageCollider = null;
    }
}
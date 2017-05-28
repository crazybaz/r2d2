using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using SWS;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Pawn : EventView, IPawn
{
    [Inject]
    public GameLogic GameLogic { get; set; }

#region Sentinel mode, immovable

    [Header("Режим 'Стража'")]
    [Tooltip("Если включено, юнит сразу стоит неподвижно, не начиная движения по траектории")]
    [SerializeField] protected bool immovable = false;

    [Header("Событие при уничтожении юнита")]
    [Tooltip("Подписывай методы PawnCommandProxy, к примеру ResumeMove, чтобы один юнит продолжил движение после смерти другого")]
    public UnityEvent DestroyEvent;

    [Header("Отслеживание приближения паладина")]
    [Tooltip("Если больше 0, при приближении паладина на заданную дистанцию юнит продолжит движение по траектории")]
    [SerializeField] private float trackingDistance = -1.0f;

    [Header("Событие при приближении паладина")]
    [Tooltip("Подписывай методы PawnCommandProxy, к примеру PauseMove, ResumeMove... и т.д.")]
    public UnityEvent TrackingEvent;

    public void ResumeMove()
    {
        if (Disabled) return;

        (Mover.pathContainer as BezierPathManager).CalculatePath();
        Mover.moveToPath = false;
        if (immovable)
            Mover.StartMove();
        else
            Mover.Resume();

        immovable = false;

        for (int i = 0; i < Anim.layerCount; i++)
            Anim.Play(BattleStateMachine.STATE_RUN, i);
    }

    public void PauseMove()
    {
        if (Disabled) return;

        Mover.Pause();

        for (int i = 0; i < Anim.layerCount; i++)
            Anim.Play(BattleStateMachine.STATE_STAY, i);
    }

#endregion

#region Serialized

    public uint Level = 1;
    public PawnType Type;
    public FadeOutSettings FadeOutSettings;

    [SerializeField] private PawnKind kind;
    public PawnKind Kind { get { return kind; } set { kind = value; } }

#endregion

#region NonSerialized

    [NonSerialized] public PawnConfig Config;
    [NonSerialized] public Animator Anim;
    [NonSerialized] public splineMove Mover; // can be null

    public bool Disabled { get; set; }
    public bool MoverRequired { get; protected set; }
    public bool AnimatorRequired { get; protected set; }

    protected List<GameObject> dropItems = new List<GameObject>();

    protected BattleState currentState;
    protected BattleState nextState;
    protected List<BattleStateTransition> transitions;

#endregion

#region Public

    public void Init(IPawnConfig config)
    {
        Config = (PawnConfig)config;
        Init();
    }

    protected virtual void Init()
    {
        Anim = GetComponent<Animator>();
        Mover = GetComponentInParent<splineMove>();

        InitPawnGroup();
        FadeOutSettings.Init();

        transitions = new List<BattleStateTransition>
        {
            new BattleStateTransition(BattleState.Any, BattleState.Death, AnyDeathHandler)
        };

        if (immovable)
        {
            Mover.Stop();
            for (int i = 0; i < Anim.layerCount; i++)
                Anim.Play(BattleStateMachine.STATE_STAY, i);
        }
    }

    public virtual void ChangeSpeed(float value)
    {
        if (Mover == null) return;

        Mover.ChangeSpeed(value);
    }

    public virtual void HitHandler()
    {
        FadeOutSettings.Hit();
    }

    public virtual void DestroyAnim()
    {
        nextState = BattleState.Death;
        FadeOut();
    }

    public void DisablePawn()
    {
        Disabled = true;

        if (DestroyEvent != null)
            DestroyEvent.Invoke();

        BoxCollider[] colliders = GetComponents<BoxCollider>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;

        var weapons = GetComponents<Weapon>();
        foreach (var weapon in weapons)
            weapon.enabled = false;

        if (Mover != null)
            Mover.Stop();
    }

#endregion

#region DROP

    public void AddDropItem(GameObject dropObject)
    {
        if (dropObject == null)
            throw new ArgumentNullException("Drop Item can not be null");

        dropItems.Add(dropObject);
    }

    public IEnumerator ProcessDrop(float delay)
    {
        // начисление очков
        if (Config.RewardPoints > 0)
            GameLogic.CurrentSession.AddScore(Mathf.CeilToInt(Config.RewardPoints * GameLogic.BuffModule.ScoreMultiplier));

        foreach (var prefab in dropItems)
        {
            yield return new WaitForSeconds(delay);
            var controller = prefab.GetComponent<DropItemController>();
            var position = transform.position + Random.insideUnitSphere * controller.Radius;
            position.y = 4f;
            var instance = Instantiate(prefab, position, new Quaternion(0, 90, 90, 0)) as GameObject;
            instance.GetComponent<Rigidbody>().AddForce((position - transform.position) * controller.ForceModifier, ForceMode.Impulse);
        }

        dropItems.Clear();
    }

#endregion

#region Private

    protected virtual void Update()
    {
        FadeOutSettings.Update();
        currentState = BattleStateMachine.Update(Anim, nextState, transitions);

        if (Disabled || !immovable || trackingDistance <= 0) return;

        if ((GameLogic.Paladin.View.transform.position - transform.position).magnitude < trackingDistance)
        {
            if (TrackingEvent != null)
                TrackingEvent.Invoke();
        }
    }

    protected virtual void FadeOut()
    {
        FadeOutSettings.FadeOut();
        FadeOutSettings.FadeComplete += FadeCompleteHandler;
    }

    private void FadeCompleteHandler()
    {
        FadeOutSettings.FadeComplete -= FadeCompleteHandler;
        Destroy(gameObject);
    }

    protected virtual void AnyDeathHandler()
    {
        Anim.SetTrigger(BattleStateMachine.TRIGGER_ANY_DEATH);
        nextState = BattleState.Disappear;
    }

    protected virtual void InitPawnGroup()
    {
        var pattern = GetPattern(transform.parent);

#if !UNITY_EDITOR

        if (pattern == null)
            throw new MissingComponentException("Top parent of the Pawn does not contain the PatternConfig");
#endif

        if (pattern != null)
            pattern.AddPawn(this);
    }

    private IPattern GetPattern(Transform transform)
    {
        if (transform == null)
            return null;

        while (true)
        {

#if UNITY_EDITOR

            if (transform == null)
                return null;
#endif

            var pattern = transform.GetComponent<IPattern>();
            if (pattern == null)
            {
                transform = transform.parent;

                if (transform == null)
                    return null;

                continue;
            }
            return pattern;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Config = null;
        dropItems = null;
        Anim = null;
        Mover = null;
        DestroyEvent.RemoveAllListeners();
        DestroyEvent = null;
        TrackingEvent.RemoveAllListeners();
        TrackingEvent = null;
    }

#endregion
}
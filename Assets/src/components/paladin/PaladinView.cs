using System;
using System.Collections.Generic;
using Paladin.UI;
using strange.extensions.mediation.impl;
using UnityEngine;

[Serializable]
public struct BulletOrigin
{
    public SuspensionType Type;
    public GameObject Origin;
}

public class PaladinView : EventView, IPawn, IWeaponInfo
{
    [Inject]
    public Logic Logic { get; set; }


    public PaladinType Type;

    public PawnKind Kind { get { return PawnKind.Paladin; } }

    public BulletOrigin[] Suspensions;

    [NonSerialized]
    public Animator Anim;
    public Animator AnimTop;

    public PaladinConfig Config;

    public bool Disabled { get; set; }
    public bool IgnoreObstacleCollisions;
    public Material HologramMaterial;
    public GameObject FxDust;

    private Dictionary<Pawn, float> collisions;
    private List<Pawn> damagers;
    private List<Pawn> toRemove;

    [NonSerialized]
    public BattleState AnimState;
    private string activeAnimTrigger;

    public Promise<WeaponType, WeaponInfo> WeaponInfoPromise { get; private set; }

    public WeaponOwnerType OwnerType
    {
        get { return WeaponOwnerType.Paladin; }
    }

    public PaladinView()
    {
        WeaponInfoPromise = new Promise<WeaponType, WeaponInfo>();
    }

    public void Init(IPawnConfig config)
    {
        Config = (PaladinConfig)config;
        Anim = GetComponent<Animator>();
        AnimState = BattleState.Run;
    }

    protected override void Awake()
    {
        base.Awake();

        collisions = new Dictionary<Pawn, float>();
        damagers = new List<Pawn>();
        toRemove = new List<Pawn>();
    }

    private void Update()
    {
        if (!Disabled)
            HandleCollisionDamage();
    }

    public virtual void ShotHandler()
    {
        switch (Type)
        {
            case PaladinType.Ranger:
            case PaladinType.Peacemaker:
            case PaladinType.Crusader:
                AnimTop.Play(BattleStateMachine.STATE_SHOT_LEFT);
                AnimTop.Play(BattleStateMachine.STATE_SHOT_RIGHT);
                break;
            case PaladinType.Assault:
                AnimTop.Play(BattleStateMachine.STATE_SHOT_LEFT1);
                AnimTop.Play(BattleStateMachine.STATE_SHOT_LEFT2);
                AnimTop.Play(BattleStateMachine.STATE_SHOT_RIGHT);
                break;
            case PaladinType.Juggernaut:
                // not implemented yet
                break;
        }
    }

    public void TriggerAnimState(BattleState nextState)
    {
        if (nextState != AnimState)
        {
            AnimState = nextState;

            if (activeAnimTrigger != null)
            {
                Anim.ResetTrigger(activeAnimTrigger);
                AnimTop.ResetTrigger(activeAnimTrigger);
            }

            if (nextState == BattleState.Stay)
            {
                activeAnimTrigger = BattleStateMachine.TRIGGER_ANY_STAY;
            }
            else if (nextState == BattleState.Run)
            {
                activeAnimTrigger = BattleStateMachine.TRIGGER_ANY_RUN;
            }
            else if (nextState == BattleState.RunReverce)
            {
                activeAnimTrigger = BattleStateMachine.TRIGGER_ANY_RUN_REVERCE;
            }
            else
            {
                throw new Exception("BattleState " + nextState + " not handled");
            }

            Anim.SetTrigger(activeAnimTrigger);
            AnimTop.SetTrigger(activeAnimTrigger);
        }
    }

    private void HandleCollisionDamage()
    {
        foreach (KeyValuePair<Pawn, float> pair in collisions)
        {
            var enemy = pair.Key;
            var collisionTime = pair.Value;

            if (enemy == null || enemy.Disabled)
            {
                toRemove.Add(enemy);
            }
            else if (Time.fixedTime - collisionTime >= Logic.Defs.Config.COLLISION_COOLDOWN)
            {
                dispatcher.Dispatch(ViewEvent.ENEMY_COLLISION, enemy);
                enemy.dispatcher.Dispatch(ViewEvent.PALADIN_COLLISION);

                damagers.Add(enemy);
            }
        }

        // Remove destroyed objects
        foreach (Pawn item in toRemove)
            collisions.Remove(item);

        // Update time
        foreach (Pawn damager in damagers)
            collisions[damager] = Time.fixedTime;

        toRemove.Clear();
        damagers.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<Pawn>();

        if (enemy != null)
        {
            if (enemy.Kind == PawnKind.Obstacle && IgnoreObstacleCollisions)
                return;

            if (!collisions.ContainsKey(enemy))
                collisions.Add(enemy, 0);
        }

        // сбор лута
        var dropComponent = other.GetComponent<IDropItem>() as Component;
        if (dropComponent != null)
            dispatcher.Dispatch(ViewEvent.COLLECT_DROP_ITEM, dropComponent);
    }

    private void OnTriggerExit(Collider other)
    {
        var enemy = other.GetComponent<Pawn>();
        if (enemy != null && collisions.ContainsKey(enemy))
            collisions.Remove(enemy);
    }

    public virtual void HitHandler()
    {
        // TODO: hit effect
    }

    public void DestroyAnim()
    {
        // TODO: здесь будет анимация гибели игрока
        gameObject.SetActive(false);
    }

    public void DisablePawn()
    {
        Disabled = true;

        GetComponent<BoxCollider>().enabled = false;

        var weapons = GetComponents<Weapon>();
        foreach (var weapon in weapons)
            weapon.enabled = false;
    }
}
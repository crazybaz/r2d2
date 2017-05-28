using System.Collections.Generic;
using Paladin.Controller;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.Events;

public class PaladinMediator : EventMediator
{
    [Inject]
    public PaladinView View { get; set; }

    [Inject]
    public PaladinModel Model { get; set; }

    [Inject]
    public Logic Logic { get; set; }

    [Inject]
    public GameLogic GameLogic { get; set; }

    [Inject]
    public AbilityFactory AbilityFactory { get; set; }

    private void FixedUpdate()
    {
        Model.BuffModule.FixedUpdate();
    }

    public override void OnRegister()
    {
        var config = Logic.Defs.GetPaladin(View.Type);

        View.Init(config);
        Model.Init(config, View.dispatcher);

        View.WeaponInfoPromise.Assign(GetWeaponInfo);

        GameLogic.Init(View, Model);

        // Навешиваем оружия
        foreach (var type in Logic.GetPaladin(View.Type).Weapons)
        {
            var weapon = View.gameObject.AddComponent(WeaponFactory.GetWeapon(type)) as Weapon;
            weapon.Origins = new List<Transform>();
            weapon.enabled = false;

            foreach (var suspension in View.Suspensions)
            {
                if (suspension.Type == weapon.SuspensionType)
                weapon.Origins.Add(suspension.Origin.transform);
            }

            // Добавим колбэк на выстрел только для легкого оружия
            if (weapon.SuspensionType == SuspensionType.Light)
            {
                weapon.OnShot = new UnityEvent();
                weapon.OnShot.AddListener(View.ShotHandler);
            }
        }

        // запускаем пассивки
        foreach (var abilityType in Logic.GetPaladin(View.Type).Ability.Passive)
        {
            AbilityFactory.Create(abilityType, Logic.GetAbilityLevel(abilityType)).Activate();
        }

        View.GetComponent<PlayerInputController>().Init(Model);

        View.dispatcher.AddListener(ViewEvent.BULLET_HIT, BulletHitHandler);
        View.dispatcher.AddListener(ViewEvent.ENEMY_COLLISION, EnemyCollisionHandler);
        View.dispatcher.AddListener(ViewEvent.RESTORE_HEALTH, RestoreHealthHandler);
        View.dispatcher.AddListener(ViewEvent.COLLECT_DROP_ITEM, CollectDropItem);

        Model.Dispatcher.AddListener(ModelEvent.DESTROY, DestroyHandler);

        dispatcher.AddListener(GameEvent.PAWN_DESTROY, PawnDestroyHandler);
        dispatcher.AddListener(GameEvent.START_SHOOTING, StartShootingHandler);
    }

    public override void OnRemove()
    {
        View.dispatcher.RemoveListener(ViewEvent.BULLET_HIT, BulletHitHandler);
        View.dispatcher.RemoveListener(ViewEvent.ENEMY_COLLISION, EnemyCollisionHandler);
        View.dispatcher.RemoveListener(ViewEvent.RESTORE_HEALTH, RestoreHealthHandler);
        View.dispatcher.RemoveListener(ViewEvent.COLLECT_DROP_ITEM, CollectDropItem);

        Model.Dispatcher.RemoveListener(ModelEvent.DESTROY, DestroyHandler);

        dispatcher.RemoveListener(GameEvent.PAWN_DESTROY, PawnDestroyHandler);
        dispatcher.RemoveListener(GameEvent.START_SHOOTING, StartShootingHandler);
    }

    private WeaponInfo GetWeaponInfo(WeaponType weaponType)
    {
        return new WeaponInfo
        {
            Type = weaponType,
            Level = Logic.GetWeaponLevel(weaponType)
        };
    }

    private void CollectDropItem(IEvent evt)
    {
        var dropComponent = evt.data as Component;
        var dropItem = ComponentExtention.CopyComponent<Component>(dropComponent, View.gameObject) as IDropItem;
        dropItem.TryActivate();
        Destroy(dropComponent.gameObject);

        // TODO: refactor, временная залепа
        //if (itemComponent is BuffView)
        //    View.CollectBuff(itemComponent as BuffView);
    }

    private void BulletHitHandler(IEvent evt)
    {
        Model.Hit((float)evt.data);
    }

    private void EnemyCollisionHandler(IEvent evt)
    {
        var enemy = (Pawn)evt.data;
        Model.Hit(enemy.Config.CollisionDamage);
    }

    private void RestoreHealthHandler(IEvent evt)
    {
        Model.Heal((float)evt.data);
    }

    private void DestroyHandler()
    {
        View.DestroyAnim();
        View.DisablePawn();
        dispatcher.Dispatch(CommandEvent.FINISH_BATTLE, false);
    }

    private void StartShootingHandler()
    {
        foreach (var weapon in View.gameObject.GetComponents<Weapon>())
            weapon.enabled = true;
    }

    private void PawnDestroyHandler(IEvent e)
    {
        var pawn = (Pawn)e.data;
        Model.Heal(pawn.Config.MaxHealth * GameLogic.BuffModule.VampirismMultiplier);
    }
}
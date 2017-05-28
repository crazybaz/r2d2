using System.Collections.Generic;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;
using strange.extensions.mediation.impl;

public class PawnMediator : EventMediator
{
    [Inject]
    public IPawn View { get; set; }

    [Inject]
    public PawnModel Model { get; set; }

    [Inject]
    public Logic Logic { get; set; }

    [Inject]
    public GameLogic GameLogic { get; set; }

    protected Pawn pawn;

    public override void OnRegister()
    {
        CheckRequirements();
        RegisterHandlers();
        RegisterConfig();
        InitDrop();

        GameLogic.RegisterPawn(pawn, Model);
    }

    protected void RegisterHandlers()
    {
        pawn = View as Pawn;

        // view events
        View.dispatcher.AddListener(ViewEvent.BULLET_HIT, BulletHitHandler);
        View.dispatcher.AddListener(ViewEvent.PALADIN_COLLISION, PaladinCollisionHandler);
        //View.dispatcher.AddListener(ViewEvent.PAWN_COLLISION, PawnCollisionHandler);
        View.dispatcher.AddListener(ViewEvent.STUN_SPEED, StunSpeedHandler);
        View.dispatcher.AddListener(ViewEvent.BOOST_SPEED, BoostSpeedHandler);

        // model events
        Model.Dispatcher.AddListener(ModelEvent.SPEED_CHANGED, ChangeSpeedHandler);
        Model.Dispatcher.AddListener(ModelEvent.DESTROY, DestroyHandler);
    }

    protected void RegisterConfig()
    {
        var config = Logic.Defs.GetPawn(pawn.Type, pawn.Level);

        pawn.Init(config);
        Model.Init(config);
    }

    public override void OnRemove()
    {
        if (!View.Disabled)
            GameLogic.UnregisterPawn(pawn);

        View.dispatcher.RemoveListener(ViewEvent.BULLET_HIT, BulletHitHandler);
        View.dispatcher.RemoveListener(ViewEvent.PALADIN_COLLISION, PaladinCollisionHandler);
        //View.dispatcher.RemoveListener(ViewEvent.PAWN_COLLISION, PawnCollisionHandler);
        View.dispatcher.RemoveListener(ViewEvent.STUN_SPEED, StunSpeedHandler);
        View.dispatcher.RemoveListener(ViewEvent.BOOST_SPEED, BoostSpeedHandler);

        Model.Dispatcher.RemoveListener(ModelEvent.SPEED_CHANGED, ChangeSpeedHandler);
        Model.Dispatcher.RemoveListener(ModelEvent.DESTROY, DestroyHandler);

        base.OnRemove();
    }

    public void InitDrop()
    {
        Dictionary<DropType, List<DropItem>> dropConfig;
        if (!Logic.Defs.TryGetDrop(pawn.Config.GroupType, out dropConfig))
            return;

        // loot
        if (dropConfig.ContainsKey(DropType.Loot))
        {
            if (Dice.Roll(Logic.Defs.Config.LOOT_DROP_PROBABILITY + GameLogic.BuffModule.LootDropRateBonus))
            {
                var lootItem = Dice.Roll(dropConfig[DropType.Loot]);
                pawn.AddDropItem(Resources.Load<GameObject>("prefabs/drop/loot/" + lootItem.Id));
            }
        }

        // ability
        if (dropConfig.ContainsKey(DropType.Ability))
        {
            var dropItems = GameLogic.AbilityModule.PrepareAbilityDrop(dropConfig[DropType.Ability]);
            if (dropItems.Count > 0)
            {
                var dropItem = Dice.Roll(dropItems, 100 - GameLogic.BuffModule.AbilityDropRateBonus);
                if (dropItem != null)
                    pawn.AddDropItem(Resources.Load<GameObject>("prefabs/drop/ability/" + dropItem.Id));
            }
        }
    }

    // >>>>>>>>>>>>>>>>>>>>>>>> HANDLERS >>>>>>>>>>>>>>>>>>>>>>>>

    protected void ChangeSpeedHandler(IEvent evt)
    {
        pawn.ChangeSpeed((float)evt.data);
    }

    /*private void PawnCollisionHandler(IEvent evt)
    {
        var slave = (Pawn)evt.data;
        var slaveModel = Logic.GetPawnModel(slave);

        if (Model.Speed < slaveModel.Speed)
        {
            pawn.Slave = slave;
            slaveModel.MakeSlave(Model);
        }
    }*/

    protected void BulletHitHandler(IEvent evt)
    {
        // TODO: здесь напрашивается более грамотная проверка на габариты объекта
        if (Config.I.GAMEPLAY_BOUNDS.IsOutside(pawn.transform.position))
            return;

        Model.Hit((float)evt.data);

        if (!View.Disabled)
            View.HitHandler();
    }

    protected void PaladinCollisionHandler()
    {
        if (View.Kind == PawnKind.Obstacle && GameLogic.BuffModule.IsIgnoringObstacles)
            return;

        Model.Hit(GameLogic.Paladin.Model.CollisionDamage);
    }

    protected void CheckRequirements()
    {
        var battlePawn = View as BattlePawn;
        var humanoid = View as HumanoidEnemy;

        if (battlePawn != null)
        {
            if (battlePawn.Mover == null && battlePawn.MoverRequired)
                throw new MissingComponentException("component SplineMover not found in BattlePawn parent");

            if (battlePawn.Anim == null && battlePawn.AnimatorRequired)
                throw new MissingComponentException("Animator not found for BattlePawn");
        }
        if (humanoid != null)
        {
            if (humanoid.AnimTop == null)
                throw new MissingComponentException("Animator for top not found for Humanoid");
        }
    }

    protected void StunSpeedHandler(IEvent evt)
    {
        Model.StunSpeed((bool)evt.data);
    }

    protected void BoostSpeedHandler(IEvent evt)
    {
        Model.BoostSpeed((float)evt.data);
    }

    protected virtual void DestroyHandler()
    {
        pawn.DisablePawn();
        pawn.DestroyAnim();

        //ReleaseSlave(pawn.Slave);

        dispatcher.Dispatch(GameEvent.PAWN_DESTROY, pawn); // TODO: not using

        GameLogic.UnregisterPawn(pawn);

        StartCoroutine(pawn.ProcessDrop(Logic.Defs.Config.DROP_ITEMS_DELAY));
    }
    // >>>>>>>>>>>>>>>>>>>>>>>> PRIVATES >>>>>>>>>>>>>>>>>>>>>>>>
}
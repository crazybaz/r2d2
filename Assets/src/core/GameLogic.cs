using System;
using System.Collections.Generic;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class CurrentPaladin
{
    public PaladinView View;
    public PaladinModel Model;
}

public class GameLogic
{
    [Inject(ContextKeys.CROSS_CONTEXT_DISPATCHER)]
    public IEventDispatcher Dispatcher { get; set; }

    [Inject]
    public DefinitionManager Defs { get; set; }

    public CurrentPaladin Paladin; // move to current session
    public Session CurrentSession { get; private set; }

    [Inject]
    public LocalConfig LocalConfig { get; set; }

    // Some sugar
    public BuffModule BuffModule { get { return Paladin.Model.BuffModule; }}

    [Inject]
    public AbilityModule AbilityModule { get; set; }

    private Dictionary<Pawn, PawnModel> pawnPool;
    private ushort flyingPawnCount; // счетчик высоколетящих целей в данный момент находящихся на сцене (PawnKind.FlyingPawn)

    public void Init(PaladinView view, PaladinModel model)
    {
        Paladin = new CurrentPaladin
        {
            View = view,
            Model = model
        };
    }

    public void StartSession(PaladinType paladinType)
    {
        CurrentSession = new Session
        {
            Type = paladinType,
        };

        CurrentSession.ChangeFieldMoveSpeed(Defs.Config.FIELD_MOVE_SPEED);

        flyingPawnCount = 0;
        if (pawnPool == null)
            pawnPool = new Dictionary<Pawn, PawnModel>();
        else
            pawnPool.Clear();
    }

    public void EndSession()
    {
        GameObject.Destroy(Paladin.View.gameObject);
        Paladin.View = null;
        Paladin.Model = null;
    }

    public void ChangeFieldMoveSpeed(float value)
    {
        CurrentSession.ChangeFieldMoveSpeed(value);
        Dispatcher.Dispatch(GameEvent.CHANGE_FIELD_SPEED);
    }

    // >>>>>>>>>>>>>>>>>>>>>>> PAWN POOL >>>>>>>>>>>>>>>>>>>>>>>

    public IEnumerable<Pawn> Pawns
    {
        get { return pawnPool.Keys; }
    }

    public bool isAnyFlyingPawn
    {
        get { return flyingPawnCount > 0; }
    }

    public void RegisterPawn(Pawn pawn, PawnModel model)
    {
        if (pawnPool.ContainsKey(pawn))
        {
            Debug.LogWarning("WARNING: PAWN IS ALREADY REGISTERED");
        }
        else
        {
            pawnPool.Add(pawn, model);
            if (pawn.Kind == PawnKind.FlyingEnemy)
                flyingPawnCount++;
        }
    }

    public void UnregisterPawn(Pawn pawn)
    {
        if (pawn == null) return;

        if (pawnPool.ContainsKey(pawn))
        {
            pawnPool.Remove(pawn);
            if (pawn.Kind == PawnKind.FlyingEnemy)
                flyingPawnCount--;
        }
        else
        {
            Debug.LogWarning("WARNING: PAWN IS'T REGISTERED");
        }
    }

    public PawnModel GetPawnModel(Pawn pawn, bool withException = true)
    {
        PawnModel model;
        if (!pawnPool.TryGetValue(pawn, out model) && withException)
            throw new Exception("PAWN IS'T REGISTERED");
        return model;
    }

    // >>>>>>>>>>>>>>>>>>>>>>> PAWN POOL END >>>>>>>>>>>>>>>>>>>>>>>
}
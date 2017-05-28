using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public enum PatternType
{
    sp, dp, ap
}

public class Pattern : EventView, IPattern
{
    public PatternType Type;

    protected bool destroyed;
    private readonly List<Pawn> pawnList = new List<Pawn>();

    protected override void Awake()
    {
        base.Awake();

        var pattern = Utils.FindInParent<MasterPattern>(transform);

#if !UNITY_EDITOR
        if (pattern == null)
            throw new MissingComponentException("MasterParent not found for pattern " + this);
#endif
        if (pattern != null)
            pattern.AddPattern(this);
    }

    public void AddPawn(Pawn pawn)
    {
        pawnList.Add(pawn);
    }

    public List<Pawn> PawnList
    {
        get { return pawnList; }
    }

    public bool Destroyed
    {
        get { return destroyed; }
        set
        {
            destroyed = value;
            if (destroyed)
                Destroy(this);
        }
    }

    protected override void OnDestroy()
    {
        if (pawnList != null)
            pawnList.Clear();

        base.OnDestroy();
    }
}
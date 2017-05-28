using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Paladin.View;

public enum BuffType
{
    Shield,
    Stun,
    IgnoreObstacle,
    Torn
}

public class BuffModule
{
    [Inject]
    public Logic Logic { get; set; }

    [Inject]
    public GameLogic GameLogic { get; set; }

    // >>>>>>>>>>>>>>>>>>>>>>> BOOSTERS >>>>>>>>>>>>>>>>>>>>>>>

    public float ScoreMultiplier = 1;
    public float CoinMultiplier = 1;
    public float HealthMultiplier = 1;
    public float SpeedMultiplier = 1;
    public float DamageMultiplier = 1;

    public uint FireRateStackCount { get; private set; }
    public float FireRateMultiplier { get; private set; } // = 1; // The ability to have auto property initializers is included since C# 6.0
    public void AddFireRate(float multiplier)
    {
        if (FireRateStackCount >= Logic.Defs.Config.FIRERATE_STACK_COUNT)
            return;

        FireRateMultiplier *= multiplier;
        FireRateStackCount += 1;

        GameLogic.Dispatcher.Dispatch(GameEvent.FIRE_RATE_COLLECTED);
    }

    public float ArmorMultiplier = 1;

    public int LootDropRateBonus = 0;
    public int AbilityDropRateBonus = 0;

    public float VampirismMultiplier = 0; // тут именно ноль


    public BuffModule()
    {
        FireRateMultiplier = 1;
    }

    // >>>>>>>>>>>>>>>>>>>>>>> BUFFS >>>>>>>>>>>>>>>>>>>>>>>

    private readonly Dictionary<BuffType, BuffView> buffs = new Dictionary<BuffType, BuffView>();
    private readonly Dictionary<BuffType, float> durations = new Dictionary<BuffType, float>();
    private readonly List<BuffType> expired = new List<BuffType>();

    public void FixedUpdate()
    {
        expired.Clear();
        foreach (var buffType in durations.Keys.ToList())
        {
            durations[buffType] -= Time.fixedDeltaTime;
            if (durations[buffType] < 0)
                expired.Add(buffType);
        }

        foreach (var buffType in expired)
        {
            buffs[buffType].Deactivate();
            buffs.Remove(buffType);
            durations.Remove(buffType);
        }
    }

    public void ActivateBuff(BuffView view, float duration)
    {
        // проверка на обновление перка
        if (buffs.ContainsKey(view.Type))
        {
            durations[view.Type] += duration;
            GameObject.Destroy(view);
        }
        else
        {
            buffs[view.Type] = view;
            durations[view.Type] = duration;
            view.Paladin = GameLogic.Paladin;
            view.Activate();
        }
    }

    // UNLIMIT DURATION BUFF
    public void ActivateBuff(BuffView view)
    {
        if (buffs.ContainsKey(view.Type))
            throw new Exception("DUPLICATE BUFF ACTIVATION");

        buffs[view.Type] = view;
        view.Paladin = GameLogic.Paladin;
        view.Activate();
    }


    // >>>>>>>>>>>>>>>>>>>>>>> HELPERS >>>>>>>>>>>>>>>>>>>>>>>

    public bool IsUnderShield
    {
        get { return buffs.ContainsKey(BuffType.Shield); }
    }

    public bool IsIgnoringObstacles
    {
        get { return buffs.ContainsKey(BuffType.IgnoreObstacle);  }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class GameConfig
{
    public readonly int FIELD_MOVE_SPEED;
    public readonly Vector2[] COIN_PRICE;
    public readonly int LOOT_DROP_PROBABILITY;
    public readonly float DROP_ITEMS_DELAY;
    public readonly float COLLISION_COOLDOWN;
    public readonly uint FIRERATE_STACK_COUNT;
    public readonly uint NANOBOTS_DURATION;
    public readonly uint PATTERN_START_DELAY;
    public readonly int PATTERN_APPEAR_Z;
    public readonly float SOFT_TIME_RATE;
    public readonly float TIME_HARD_RATE;
    public readonly uint DISCOUNT_HOUR;
    public readonly float START_LOGO_TIMEOUT;

    public readonly uint ENERGY_RESTORE_INTERVAL;
    public readonly uint ENERGY_MAX_COUNT;
    public readonly uint FULL_ENERGY_COST;
    public readonly uint UNLIM_ENERGY_COST;
    public readonly uint UNLIM_ENERGY_DURATION;

    public readonly uint QUEST_CHEST_MAX_PROGRESS;

    private JSONObject data;

    public GameConfig(JSONObject data)
    {
        this.data = data;

        FIELD_MOVE_SPEED = GetInt("FIELD_MOVE_SPEED");

        var coinPrice = GetList("COIN_PRICE");
        COIN_PRICE = new Vector2[coinPrice.Count];
        for (var i = 0; i < coinPrice.Count; i++)
        {
            var coinPriceItem = coinPrice[i].list;
            COIN_PRICE[i] = new Vector2(coinPriceItem[0].i, coinPriceItem[1].i);
        }

        LOOT_DROP_PROBABILITY = GetInt("LOOT_DROP_PROBABILITY");
        DROP_ITEMS_DELAY = GetFloat("DROP_ITEMS_DELAY");
        COLLISION_COOLDOWN = GetFloat("COLLISION_COOLDOWN");
        FIRERATE_STACK_COUNT = GetUint("FIRERATE_STACK_COUNT");
        NANOBOTS_DURATION = GetUint("NANOBOTS_DURATION");

        PATTERN_START_DELAY = GetUint("PATTERN_START_DELAY");
        PATTERN_APPEAR_Z = GetInt("PATTERN_APPEAR_Z");
        SOFT_TIME_RATE = GetFloat("SOFT_TIME_RATE");
        TIME_HARD_RATE = GetFloat("TIME_HARD_RATE");
        DISCOUNT_HOUR = GetUint("DISCOUNT_HOUR");
        START_LOGO_TIMEOUT = GetFloat("START_LOGO_TIMEOUT");

        ENERGY_RESTORE_INTERVAL = GetUint("energy_restore_interval");
        ENERGY_MAX_COUNT = GetUint("energy_max_count");
        FULL_ENERGY_COST = GetUint("full_energy_cost");
        UNLIM_ENERGY_COST = GetUint("unlim_energy_cost");
        UNLIM_ENERGY_DURATION = GetUint("unlim_energy_duration");

        QUEST_CHEST_MAX_PROGRESS = GetUint("quest_chest_max_progress");
    }

    private uint GetUint(string param)
    {
        return (uint)data.GetField(param).i;
    }

    private int GetInt(string param)
    {
        return (int)data.GetField(param).i;
    }

    private float GetFloat(string param)
    {
        return data.GetField(param).f;
    }

    private string GetString(string param)
    {
        return data.GetField(param).str;
    }

    private List<JSONObject> GetList(string param)
    {
        return data.GetField(param).list;
    }
}
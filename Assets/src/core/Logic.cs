using System;
using System.Collections.Generic;
using System.Linq;
using core;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class Logic
{
//    [Inject(ContextKeys.CONTEXT_DISPATCHER)]
//    public IEventDispatcher Dispatcher { get; set; }

    [Inject]
    public DefinitionManager Defs { get; set; }
    public LeaderBoard Leaderboard = new LeaderBoard(); // обновляется с информацией от сервера, служит для сохранения и отображения топов

    // TODO: maybe refactor to private
    //public Loot Loot { get; set; }

    /*public void Init()
    {
        // Dispatcher.AddListener(GameEvent.ADD_LOOT_ITEM, AddLootItemHandler);
    }*/

    // =============================================================================================

    // Milliseconds represent the moment we received state data
    public long ServerTime;

    public uint ClientTime;

    // Delta time from last state data received
    public uint DeltaTime
    {
        get { return MillisSinceStartup - ClientTime; }
    }

    public long CurrentTime
    {
        get { return ServerTime + DeltaTime; }

    }

    public uint MillisSinceStartup
    {
        get { return (uint) (Time.realtimeSinceStartup * 1000); }
    }

    public uint Level { get; private set; } // TODO: завернуть в структуру Player (Player.Level)
    public EnergyState Energy;
    public uint Exp;
    public uint RecordScores;
    public uint FacebookLoginBonus;

    // TODO: залипуха, будем делать параллельно с сервером
    public Dictionary<object, long> UpgradesInProgress = new Dictionary<object, long>();

    private Loot loot; // type => count
    private Dictionary<WeaponType, uint> weapon; // type => level
    private Dictionary<AbilityType, uint> ability; // type => level
    private Dictionary<PaladinType, PaladinState> paladins;

    public void UpdateState(string stateData)
    {
        ServerTime = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds; // TODO: parse from server responce
        ClientTime = MillisSinceStartup;

        var stateJSON = new JSONObject(stateData);

        Exp = (uint)stateJSON["player"]["exp"].i;
        Level = (uint)stateJSON["player"]["level"].i;
        Energy = new EnergyState(stateJSON["player"]["energy"]);
        RecordScores = (uint)stateJSON["player"]["recordScores"].i;
        FacebookLoginBonus = (uint)stateJSON["player"]["facebookLoginBonus"].i;

        loot = DefinitionManager.GetLoot(stateJSON.GetField("loot"));

        ability = new Dictionary<AbilityType, uint>();
        var abilityData = stateJSON.GetField("ability");
        foreach (var key in abilityData.keys)
            ability[DefinitionManager.EnumValue<AbilityType>(key)] = (uint)abilityData.GetField(key).i;

        weapon = new Dictionary<WeaponType, uint>();
        var weaponData = stateJSON.GetField("weapon");
        foreach (var key in weaponData.keys)
            weapon[DefinitionManager.EnumValue<WeaponType>(key)] = (uint)weaponData.GetField(key).i;

        paladins = new Dictionary<PaladinType, PaladinState>();
        var paladinsData = stateJSON.GetField("paladin");
        foreach (var key in paladinsData.keys)
        {
            var paladinData = paladinsData.GetField(key);

            var paladinState = new PaladinState();

            // TODO: возможно понадобятся проверки
            paladinState.Upgrades.Add(NodeType.Head, (uint)paladinData.GetField("head").i);
            paladinState.Upgrades.Add(NodeType.Frame, (uint)paladinData.GetField("frame").i);
            paladinState.Upgrades.Add(NodeType.Chassis, (uint)paladinData.GetField("chassis").i);

            if (paladinData.HasField("headUpgrade"))
                paladinState.UpgradesInProgress.Add(NodeType.Head, paladinData.GetField("headUpgrade").i);
            if (paladinData.HasField("frameUpgrade"))
                paladinState.UpgradesInProgress.Add(NodeType.Frame, paladinData.GetField("frameUpgrade").i);
            if (paladinData.HasField("chassisUpgrade"))
                paladinState.UpgradesInProgress.Add(NodeType.Chassis, paladinData.GetField("chassisUpgrade").i);
            // TODO: теперь надо проверить данные на завершение текущих апгрейдов и если что отправить серверу сообщение и инкрементировать показатели

            if (paladinData.HasField("weapon"))
                foreach (var item in paladinData.GetField("weapon").list)
                    paladinState.Weapons.Add(DefinitionManager.EnumValue<WeaponType>(item.str));

            if (paladinData.HasField("ability"))
            {
                if (paladinData["ability"].HasField("active"))
                    foreach (var item in paladinData["ability"]["active"].list)
                        paladinState.Ability.Active.Add(DefinitionManager.EnumValue<AbilityType>(item.str));

                if (paladinData["ability"].HasField("passive"))
                    foreach (var item in paladinData["ability"]["passive"].list)
                        paladinState.Ability.Passive.Add(DefinitionManager.EnumValue<AbilityType>(item.str));
            }

            paladins.Add(DefinitionManager.EnumValue<PaladinType>(key), paladinState);
        }
    }

    public uint GetLoot(LootType type)
    {
        return loot.Get(type);
    }

    public List<AbilityType> GetAllAbilities()
    {
        return ability.Keys.ToList();
    }

    public bool HasAbility(AbilityType type)
    {
        return ability.ContainsKey(type);
    }

    public uint GetAbilityLevel(AbilityType type)
    {
        uint level;
        if (!ability.TryGetValue(type, out level))
            throw new Exception("Ability level not found in state for " + type);
        return level;
    }

    public bool HasWeapon(WeaponType type)
    {
        return weapon.ContainsKey(type);
    }

    public uint GetWeaponLevel(WeaponType type)
    {
        uint level;
        if (!weapon.TryGetValue(type, out level))
            throw new Exception("Weapon level not found in state for " + type);
        return level;
    }

    public bool HasPaladin(PaladinType type)
    {
        return paladins.ContainsKey(type);
    }

    public PaladinState GetPaladin(PaladinType type)
    {
        PaladinState state;
        if (!paladins.TryGetValue(type, out state))
            throw new Exception("Paladin not found in state for " + type);
        return state;
    }

// FYI: мы больше не меняем сейт, он у нас readonly
//    public void AddLootItemHandler(IEvent evt)
//    {
//        var lootItem = evt.data as LootItem;
//        Loot.Add(lootItem.Type, lootItem.Count);
//    }
}

public class EnergyState
{
    public readonly ushort Count;
    public readonly long LastRestoreTime;

    public EnergyState(JSONObject data)
    {
        Count = (ushort)data.GetField("count").i;
        LastRestoreTime = data.GetField("lastRestoreTime").i;
    }
}

public class PaladinState
{
    public readonly Dictionary<NodeType, long> UpgradesInProgress; // paladin part : remaining time
    public readonly Dictionary<NodeType, uint> Upgrades;
    public readonly List<WeaponType> Weapons;
    public readonly AbilityState Ability;

    public PaladinState()
    {
        UpgradesInProgress = new Dictionary<NodeType, long>();
        Upgrades = new Dictionary<NodeType, uint>();
        Weapons = new List<WeaponType>();
        Ability = new AbilityState();
    }
}

public class AbilityState
{
    public readonly List<AbilityType> Active;
    public readonly List<AbilityType> Passive;

    public AbilityState()
    {
        Active = new List<AbilityType>();
        Passive = new List<AbilityType>();
    }
}

public class LeaderBoard
{
    public List<TopVO> WorldTop;
    public List<TopVO> LocalTop;
    public List<TopVO> FriendTop;
}
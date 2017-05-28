using System;
using System.Collections.Generic;

public enum NodeType
{
    Head,
    Frame,
    Chassis
}

[Flags]
public enum SuspensionType
{
    Light = 1 << 0,
    Normal = 1 << 1,
    Heavy = 1 << 2,
    Passive = 1 << 3,
    Drone = 1 << 4
}

public enum PaladinType
{
    [StringValue("prefabs/paladin/ranger")]
    Ranger,
    [StringValue("prefabs/paladin/assault")]
    Assault,
    [StringValue("prefabs/paladin/peacemaker")]
    Peacemaker,
    [StringValue("prefabs/paladin/crusader")]
    Crusader,
    [StringValue("prefabs/paladin/juggernaut")]
    Juggernaut
}

public class PaladinParams
{   // FYI: все переделано на float т.к. используется еще в мультиплаерах
    public float Health; // uint
    public float Speed; // uint
    public float Armor; // uint
    public float Resist;
    public float CollisionDamage; // uint

    public PaladinParams Init(JSONObject def, uint defaultValue = 0)
    {
        if (def.HasField("health"))
            Health = def["health"].f;
        else if (defaultValue > 0)
            Health = defaultValue;

        if (def.HasField("speed"))
            Speed = def["speed"].f;
        else if (defaultValue > 0)
            Speed = defaultValue;

        if (def.HasField("armor"))
            Armor = def["health"].f;
        else if (defaultValue > 0)
            Armor = defaultValue;

        if (def.HasField("resist"))
            Resist = def["resist"].f;
        else if (defaultValue > 0)
            Resist = defaultValue;

        if (def.HasField("collisionDamage"))
            CollisionDamage = def["collisionDamage"].f;
        else if (defaultValue > 0)
            CollisionDamage = defaultValue;

        return this;
    }
}

public class AbilitySlots
{
    public uint Active { get; protected internal set; }
    public uint Passive { get; protected internal set; }
}

public class UpgradePart
{
    public Loot UpgradeCost;
    public PaladinParams ParamMultipliers;

    public UpgradePart(JSONObject upgradeCost, JSONObject multipliers)
    {
        UpgradeCost = DefinitionManager.GetLoot(upgradeCost);
        ParamMultipliers = new PaladinParams().Init(multipliers, 1);
    }
}

public struct PaladinRequirements
{
    public int playerLevel;
    public PaladinType paladin;
    public int upgrades;
}

public class PaladinConfig : IPawnConfig
{
    public PaladinType Type;

    private readonly PaladinParams pparams = new PaladinParams();

    // sugar & short access & typisation
    public uint Health { get { return (uint)pparams.Health; }}
    public uint Speed { get { return (uint)pparams.Speed; }}
    public uint Armor { get { return (uint)pparams.Armor; }}
    public float Resist { get { return (uint)pparams.Resist; }}
    public uint CollisionDamage { get { return (uint)pparams.CollisionDamage; }}

    public List<SuspensionType> Suspensions;
    //public ExtraBonus || ExtraAbility; // TODO: сделать это скорее всего надо через абилки т.к. каждое условие уникально
    public AbilitySlots AbilitySlots;

    public Dictionary<uint, UpgradePart> HeadUpgrades = new Dictionary<uint, UpgradePart>();
    public Dictionary<uint, UpgradePart> FrameUpgrades = new Dictionary<uint, UpgradePart>();
    public Dictionary<uint, UpgradePart> ChasisUpgrades = new Dictionary<uint, UpgradePart>();

    public Reqs Reqs;

    public PaladinConfig Init(JSONObject def)
    {
        Type = DefinitionManager.EnumValue<PaladinType>(def.GetField("id").str);

        pparams.Init(def);

        if (def.HasField("suspensions"))
        {
            Suspensions = new List<SuspensionType>();
            foreach (var item in def["suspensions"].list)
                Suspensions.Add(DefinitionManager.EnumValue<SuspensionType>(item.str));
        }

        // TODO: research
        if (def.HasField("extraBonus"))
        {
        }

        if (def.HasField("abilitySlots"))
        {
            AbilitySlots = new AbilitySlots
            {
                Active = (uint)def["abilitySlots"]["active"].i,
                Passive = (uint)def["abilitySlots"]["passive"].i
            };
        }

        if (def.HasField("upgradeLevel"))
        {
            var upgradeLevel = (uint)def["upgradeLevel"].i;
            HeadUpgrades[upgradeLevel] = new UpgradePart(def["headUpgradeCost"], def["headMultiplier"]);
            FrameUpgrades[upgradeLevel] = new UpgradePart(def["frameUpgradeCost"], def["frameMultiplier"]);
            ChasisUpgrades[upgradeLevel] = new UpgradePart(def["chassisUpgradeCost"], def["chassisMultiplier"]);
        }

        if (def.HasField("unlockReqs"))
        {
           Reqs.Level = (ushort)def["unlockReqs"]["playerLevel"].i;
           Reqs.PaladinType = DefinitionManager.EnumValue<PaladinType>(def["unlockReqs"]["paladinType"].str);
           Reqs.PaladinUpgrades = (ushort) def["unlockReqs"]["paladinUpgrades"].i;
        }

        return this;
    }
}
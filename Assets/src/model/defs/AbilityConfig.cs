using System;
using System.Collections.Generic;

[Serializable]
public enum AbilityType
{
    ClusterGrenades,
    Invulnerability,
    Nanobots,
    ShockShells,
    Phantom,
    ShockWaves,
    VacuumBomb,
    Artillery,
    DroneX,
    DroneY,

    Rocket,
    HiScores,
    Bankir,
    HealthBonus,
    SpeedBonus,
    DamageBonus,
    FireRate,
    ArmorBonus,
    DropRate,
    Vampirism,
    Torn,
    Magnet
}

public class AbilityConfig
{
    public AbilityType Type;
    public uint ActivationCount;
    public Loot UpgradeCost;
    public Dictionary<string, object> Options;

    public AbilityConfig(JSONObject def)
    {
        if (def.HasField("id"))
            Type = DefinitionManager.EnumValue<AbilityType>(def.GetField("id").str);

        if (def.HasField("activationCount"))
            ActivationCount = (uint)def["activationCount"].n;

        if (def.HasField("upgradeCost"))
            UpgradeCost = DefinitionManager.GetLoot(def["upgradeCost"]);

        if (def.HasField("options"))
        {
            var options = def["options"];
            Options = new Dictionary<string, object>();
            foreach (var key in options.keys)
            {
                if (options[key].IsNumber)
                {
                    if (options[key].useInt)
                        Options.Add(key, (int)options[key].i);
                    else
                        Options.Add(key, options[key].f);
                }
                else if (options[key].IsBool)
                    Options.Add(key, options[key].b);
                else if (options[key].IsString)
                    Options.Add(key, options[key].str);
            }
        }
    }
}
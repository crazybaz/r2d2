using System.Collections.Generic;

public enum LevelBackType
{
    Desert,
    Woods,
    Urban
}

public class LevelConfig
{
    public uint PlayerLevel;
    public uint BossLevel;
    public LevelBackType BackType;
    public Dictionary<uint, uint> Patterns; // level => count

    public LevelConfig(JSONObject def)
    {
        PlayerLevel = (uint)def.GetField("playerLevel").i;
        BossLevel = (uint)def.GetField("bossLevel").i;
        BackType = DefinitionManager.EnumValue<LevelBackType>(def.GetField("back").str);

        Patterns = new Dictionary<uint, uint>();
        if (def.HasField("patterns"))
        {
            var patternsData = def.GetField("patterns");
            foreach (var key in patternsData.keys)
                Patterns[uint.Parse(key)] = (uint) patternsData.GetField(key).i;
        }
    }
}
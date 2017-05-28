using System.Collections.Generic;

public enum QuestType
{
    fight,
    kill,
    collect,
    activate,
    survive
}

public class QuestGoal
{
    public readonly int Level;
    public readonly int Amount;

    public QuestGoal(JSONObject def)
    {
        Level = (int) def.GetField("level").i;
        Amount = (int) def.GetField("amount").i;
    }
}

public class QuestConfig
{
    public QuestType Type;
    public List<QuestGoal> Goals;
    public Loot Reward;

    public QuestConfig(JSONObject def)
    {
        Type = DefinitionManager.EnumValue<QuestType>(def.GetField("type").str);

        Goals = new List<QuestGoal>();
        foreach (var goal in def.GetField("goal").list)
            Goals.Add(new QuestGoal(goal));

        Reward = DefinitionManager.GetLoot(def.GetField("reward"));
    }
}
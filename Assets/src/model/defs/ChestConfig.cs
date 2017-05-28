using System.Collections.Generic;
using System.Linq;

public class ChestReward
{
    public readonly uint Count;
    public readonly List<LootType> Items;

    public ChestReward(JSONObject def)
    {
        Count = DefinitionManager.GetUint(def, "count");
        Items = new List<LootType>();
        foreach (var item in def.GetField("items").list)
            Items.Add(DefinitionManager.EnumValue<LootType>(item.str));
    }
}

public class ChestConfig
{
    public readonly uint Id;
    public readonly List<ChestReward> Reward;
    public readonly uint RewardCount;

    public ChestConfig(JSONObject def)
    {
        Id = DefinitionManager.GetUint(def, "id");
        Reward = new List<ChestReward>();
        foreach (var rewardDef in def.GetField("reward").list)
        {
            var reward = new ChestReward(rewardDef);
            Reward.Add(reward);
            RewardCount += reward.Count;
        }
    }

    /*public uint RewardCount
    {
        get
        {
            return Reward.Aggregate<ChestReward, uint>(0, (current, reward) => current + reward.Count);
        }
    }*/
}
using System.Collections.Generic;

public enum DropType
{
    Loot,
    Ability
}

public class DropConfig : Dictionary<PawnGroupType, Dictionary<DropType, List<DropItem>>>
{
    public void AddItem(JSONObject item)
    {
        var id = item.GetField("id").str;
        var dropType = DefinitionManager.EnumValue<DropType>(item.GetField("kind").str);

        item.RemoveField("id");
        item.RemoveField("kind");

        foreach (var key in item.keys)
        {
            var value = item.GetField(key);
            var type = DefinitionManager.EnumValue<PawnGroupType>(key);

            if (!ContainsKey(type))
                Add(type, new Dictionary<DropType, List<DropItem>>());

            var dropConfig = this[type];

            if (!dropConfig.ContainsKey(dropType))
                dropConfig.Add(dropType, new List<DropItem>());

            var dropList = dropConfig[dropType];

            dropList.Add(new DropItem(id, dropType, value));
        }
    }
}

public class DropItem
{
    public string Id;
    public DropType Type;
    public uint Probability;

    public List<AmountCondition> AmountCondition;
    public List<HealthCondition> HealthCondition;

    public void AddAmountCondition(uint amount, uint value)
    {
        if (AmountCondition == null)
            AmountCondition = new List<AmountCondition>();

        AmountCondition.Add(new AmountCondition
        {
            Amount = amount,
            Value = value
        });
    }

    public void AddHealthCondition(uint health, uint value)
    {
        if (HealthCondition == null)
            HealthCondition = new List<HealthCondition>();

        HealthCondition.Add(new HealthCondition
        {
            Health = health,
            Value = value
        });
    }

    public DropItem(string id, DropType type, JSONObject value)
    {
        Id = id;
        Type = type;

        if (value.IsObject)
            Probability = (uint)value.GetField("dpopPossibility").i;

        if (value.IsArray)
        {
            foreach (var condition in value.list)
            {
                if (condition.HasField("amount"))
                    AddAmountCondition((uint)condition.GetField("amount").i, (uint)condition.GetField("dpopPossibility").i);
                if (condition.HasField("hpAmountPercent"))
                    AddHealthCondition((uint)condition.GetField("hpAmountPercent").i, (uint)condition.GetField("dpopPossibility").i);
            }
        }
    }

    public void UpdateProbability(GameLogic gameLogic)
    {
        if (AmountCondition != null)
        {
            Probability = 0;
            var amount = gameLogic.BuffModule.FireRateStackCount; // INFO: fast fix, refactor: get StackCount по Type
            foreach (var cond in AmountCondition)
            {
                if (amount <= cond.Amount)
                {
                    Probability = cond.Value;
                    break;
                }
            }
        }

        if (HealthCondition != null)
        {
            Probability = 0;
            var hpPercent = gameLogic.Paladin.Model.HealthProgress * 100;
            foreach (var cond in HealthCondition)
            {
                if (hpPercent <= cond.Health)
                {
                    Probability = cond.Value;
                    break;
                }
            }
        }
    }
}

public class AmountCondition
{
    public uint Amount;
    public uint Value;
}

public class HealthCondition
{
    public uint Health;
    public uint Value;
}
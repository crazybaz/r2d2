using System.Collections.Generic;
using System.Linq;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;

public class Loot
{
    [Inject(ContextKeys.CONTEXT_DISPATCHER)]
    public IEventDispatcher Dispatcher { get; set; }

    private Dictionary<LootType, uint> loot;

    public Loot()
    {
        loot = new Dictionary<LootType, uint>();
    }

    public uint this[LootType type]
    {
        get
        {
            return Get(type);
        }
        set
        {
            Add(type, value);
        }
    }

    public void Add(LootType key, uint count)
    {
        if (loot.ContainsKey(key))
            loot[key] += count;
        else
            loot[key] = count;
    }

    public bool Substract(Loot value)
    {
        foreach (var type in value.loot.Keys)
        {
            if (!loot.ContainsKey(type) || loot[type] < value.loot[type])
                return false;
        }

        foreach (var type in value.loot.Keys)
        {
            loot[type] -= value.loot[type];
        }

        return true;
    }

    public uint Get(LootType type)
    {
        return loot.ContainsKey(type) ? loot[type] : 0;
    }

    public LootType GetFirstType()
    {
        return loot.Keys.First();
    }
}
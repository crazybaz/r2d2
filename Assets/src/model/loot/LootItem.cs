using strange.extensions.mediation.impl;

namespace Paladin.View
{
    public class LootItem : EventView, IDropItem
    {
        public LootType Type;
        public uint Count = 1;

        public void TryActivate()
        {
            dispatcher.Dispatch(GameEvent.ADD_LOOT_ITEM, this);
            Destroy(this);
        }
    }
}
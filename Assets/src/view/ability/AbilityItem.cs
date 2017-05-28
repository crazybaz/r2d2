using Paladin.Controller;
using strange.extensions.mediation.impl;

namespace Paladin.View
{
    public class AbilityItem : EventView, IDropItem
    {
        [Inject]
        public Logic Logic { get; set; }

        [Inject]
        public AbilityFactory Factory { get; set; }

        public AbilityType Type;

        public void TryActivate()
        {
            // INFO: может стоит это вынести в команду
            var ability = Factory.Create(Type, Logic.GetAbilityLevel(Type));
            ability.Activate();
        }
    }
}
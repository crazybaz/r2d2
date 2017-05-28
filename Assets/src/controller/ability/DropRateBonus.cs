namespace Paladin.Controller.Ability
{
    public class DropRateBonus : AbilityBase
    {
        private int lootBonus;
        private int abilityBonus;

        public override void Prepare()
        {
            lootBonus = GetOption<int>("lootBonus");
            abilityBonus = GetOption<int>("abilityBonus");
        }

        public override void Activate()
        {
            Paladin.Model.BuffModule.LootDropRateBonus += lootBonus;
            Paladin.Model.BuffModule.LootDropRateBonus += abilityBonus;
        }

        /*public override void Deactivate()
        {
            Paladin.Model.BuffModule.LootDropRateBonus -= lootBonus
            Paladin.Model.BuffModule.LootDropRateBonus -= abilityBonus;
        }*/
    }
}
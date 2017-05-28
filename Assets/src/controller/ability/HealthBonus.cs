namespace Paladin.Controller.Ability
{
    public class HealthBonus : AbilityBase
    {
        private float multiplier;
        //private float oldHP;

        public override void Prepare()
        {
            multiplier = GetOption<float>("multiplier");
            //oldHP = PaladinModel.Config.Health;
        }

        public override void Activate()
        {
            Paladin.Model.BuffModule.HealthMultiplier += multiplier;
            Paladin.Model.UpdateSpecs();

            //PaladinModel.Specs.Health *= GetOption<float>("multiplier");
            //PaladinModel.Config.Health *= GetOption<float>("multiplier");
        }

        /*public override void Deactivate() // TODO: возможно я работаю с Health неправильно
        {
            Paladin.Model.BuffModule.HealthMultiplier -= multiplier;
            Paladin.Model.UpdateSpecs();
            //PaladinModel.Specs.Health = oldHP;
            //PaladinModel.Config.Health = oldHP;
        }*/
    }
}
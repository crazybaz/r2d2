namespace Paladin.Controller.Ability
{
    public class SpeedBonus : AbilityBase
    {
        private float multiplier;

        public override void Prepare()
        {
            multiplier = GetOption<float>("multiplier");
            /*vo = new BuffVO
            {
                Type = BuffType.SpeedRelative,
                Duration = -1f, // GetOption<float>("duration"),
                Multiplier = GetOption<float>("multiplier"),
                Options = Config.Options
            };*/
        }

        public override void Activate()
        {
            // Dispatcher.Dispatch(BattleEvent.APPLY_BUFF, vo);
            //buffId = PaladinModel.BuffModule.RegisterBuff(vo);
            Paladin.Model.BuffModule.SpeedMultiplier += multiplier;
            Paladin.Model.UpdateSpecs();
        }

        /*public override void Deactivate()
        {
            // Dispatcher.Dispatch(BattleEvent.REMOVE_BUFF, vo);
            Paladin.Model.BuffModule.SpeedMultiplier -= multiplier;
            Paladin.Model.UpdateSpecs();
        }*/
    }
}
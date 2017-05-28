namespace Paladin.Controller.Ability
{
    public class Vampirism : AbilityBase
    {
        private float multiplier;
//        private BuffVO vo;
//        private uint buffId;

        public override void Prepare()
        {
            multiplier = GetOption<float>("multiplier");
            /*vo = new BuffVO
            {
                Type = BuffType.VampirismRelative,
                Duration = -1f, // GetOption<float>("duration"),
                Multiplier = GetOption<float>("multiplier"),
                Options = Config.Options
            };*/
        }

        public override void Activate()
        {
            // Dispatcher.Dispatch(BattleEvent.APPLY_BUFF, vo);
            //buffId = PaladinModel.BuffModule.RegisterBuff(vo);
            Paladin.Model.BuffModule.VampirismMultiplier += multiplier;
        }

        /*public override void Deactivate()
        {
            // Dispatcher.Dispatch(BattleEvent.REMOVE_BUFF, vo);
            //PaladinModel.BuffModule.UnregisterBuff(buffId);
            Paladin.Model.BuffModule.VampirismMultiplier -= multiplier;
        }*/
    }
}
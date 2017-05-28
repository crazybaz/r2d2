using Paladin.View;

namespace Paladin.Controller.Ability
{
    public class Torn : AbilityBase
    {
        private TornBuff buff;

        public override void Prepare()
        {
            buff = Paladin.View.gameObject.AddComponent<TornBuff>();

            buff.FullValue = GetOption<int>("value");
            buff.Multiplier = GetOption<float>("multiplier");
            buff.Distance = GetOption<int>("distance");
        }

        public override void Activate()
        {
            Paladin.Model.BuffModule.ActivateBuff(buff);
        }

        /*public override void Deactivate()
        {
            PaladinModel.BuffModule.UnregisterBuff(buffId);
        }*/
    }
}
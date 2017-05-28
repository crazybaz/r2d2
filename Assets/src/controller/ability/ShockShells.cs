using Paladin.View;

namespace Paladin.Controller.Ability
{
    public class ShockShells : AbilityBase
    {
        private StunBuff buff;

        public override void Prepare()
        {
            buff = Paladin.View.gameObject.AddComponent<StunBuff>();
            buff.StunTime = GetOption<float>("stun");
        }

        public override void Activate()
        {
            Paladin.Model.BuffModule.ActivateBuff(buff, GetOption<int>("duration"));
        }
    }
}
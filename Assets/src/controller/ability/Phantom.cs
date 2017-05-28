using Paladin.View;

namespace Paladin.Controller.Ability
{
    public class Phantom : AbilityBase
    {
        private IgnoreObstacleBuff buff;

        public override void Prepare()
        {
            buff = Paladin.View.gameObject.AddComponent<IgnoreObstacleBuff>();
        }

        public override void Activate()
        {
            Paladin.Model.BuffModule.ActivateBuff(buff, GetOption<int>("duration"));
        }
    }
}
using System;

namespace Paladin.Controller.Ability
{
    public class AbilityBase
    {
        public AbilityType Type;
        public uint Level = 1;

        public AbilityConfig Config;
        public CurrentPaladin Paladin;
        public DefinitionManager Defs;

        public virtual void Prepare()
        {
            throw new NotImplementedException(this + " do not implement Prepare method");
        }

        public virtual void Activate()
        {
            throw new NotImplementedException(this + " do not implement Activate method");
        }

        /*public virtual void Deactivate()
        {
            throw new NotImplementedException(this + " do not implement Deactivate method");
        }*/

        protected T GetOption<T>(string key)
        {
            return Utils.GetParam<T>(Config.Options, key);
        }
    }
}
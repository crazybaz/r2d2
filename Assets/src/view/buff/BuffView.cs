using strange.extensions.mediation.impl;

namespace Paladin.View
{
    public class BuffView : EventView
    {
        public BuffType Type;
        protected bool activated;

        public CurrentPaladin Paladin;

        protected override void Start()
        {
            base.Start();
            enabled = activated;
        }

        public virtual void Activate()
        {
            enabled = activated = true;
        }

        public virtual void Deactivate()
        {
            activated = false;
            Destroy(this, 1);
        }
    }
}
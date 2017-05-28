using strange.extensions.mediation.impl;

namespace Paladin.View
{
    public class ScrollablePatternMediator : EventMediator
    {
        [Inject]
        public Logic Logic { get; set; }

        [Inject]
        public ScrollablePattern View { get; set; }

        public override void OnRegister()
        {
            View.Init(Logic.Defs.Config);
        }
    }
}
using strange.extensions.context.impl;

namespace Paladin.View
{
    public class HangarRoot : ContextView
    {
        private void Awake()
        {
            context = new HangarContext(this);
        }
    }
}
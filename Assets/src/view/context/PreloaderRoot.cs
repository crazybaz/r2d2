using strange.extensions.context.impl;

namespace Paladin.View
{
    public class PreloaderRoot : ContextView
    {
        private void Awake()
        {
            context = new PreloaderContext(this);
        }
    }
}
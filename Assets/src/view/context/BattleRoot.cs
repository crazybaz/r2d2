using strange.extensions.context.impl;

public class BattleRoot : ContextView
{
    private void Awake()
    {
        context = new BattleContext(this);
    }
}
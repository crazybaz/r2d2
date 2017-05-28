using strange.extensions.mediation.impl;

public class MasterPatternMediator : EventMediator
{
    [Inject]
    public Logic Logic { get; set; }

    [Inject]
    public MasterPattern View { get; set; }

    public override void OnRegister()
    {
        StartCoroutine(View.Init(Logic.Defs.Config));
    }
}
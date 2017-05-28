using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class PreloaderProvisorCommand : BaseProvisorCommand
{
    private float defsProgress;
    private float assetsProgress;
    private float stateProgress;
    private float hangarProgress;


    public override void Execute()
    {
        maxProgress = 4;

        dispatcher.AddListener(GameEvent.LOAD_DEFS_PROGRESS, LoadDefsProgressHandler);
        dispatcher.AddListener(GameEvent.LOAD_ASSETS_PROGRESS, LoadAssetsProgressHandler);
        dispatcher.AddListener(GameEvent.LOAD_STATE_PROGRESS, LoadStateProgressHandler);
        dispatcher.AddListener(GameEvent.LOAD_HANGAR_PROGRESS, LoadHangarProgressHandler);

        base.Execute();
    }

    private void LoadDefsProgressHandler(IEvent e)
    {
        defsProgress = Mathf.Clamp01((float) e.data);
        Update();
    }

    private void LoadAssetsProgressHandler(IEvent e)
    {
        assetsProgress = Mathf.Clamp01((float) e.data);
        Update();
    }

    private void LoadStateProgressHandler(IEvent e)
    {
        stateProgress = Mathf.Clamp01((float) e.data);
        Update();
    }

    private void LoadHangarProgressHandler(IEvent e)
    {
        hangarProgress = Mathf.Clamp01((float) e.data);
        Update();
    }

    private void Update()
    {
        currentProgress = defsProgress + assetsProgress + stateProgress + hangarProgress;
        if (currentProgress > 0)
            UpdatePredictionTime();
    }

    protected override void Finish()
    {
        dispatcher.RemoveListener(GameEvent.LOAD_DEFS_PROGRESS, LoadDefsProgressHandler);
        dispatcher.RemoveListener(GameEvent.LOAD_ASSETS_PROGRESS, LoadAssetsProgressHandler);
        dispatcher.RemoveListener(GameEvent.LOAD_STATE_PROGRESS, LoadStateProgressHandler);
        dispatcher.RemoveListener(GameEvent.LOAD_HANGAR_PROGRESS, LoadHangarProgressHandler);
    }
}
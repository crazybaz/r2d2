using Assets.src.core;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

public class LoadHangarCommand : EventCommand
{
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }

    public override void Execute()
    {
        dispatcher.AddListener(GameEvent.ACTIVATE_HANGAR, Activate);

        SceneHelper.Load(SceneHelper.HANGAR_SCENE, progress =>
        {
            if (progress <= 0.9)
                dispatcher.Dispatch(GameEvent.LOAD_HANGAR_PROGRESS, progress / 0.9f);

            if (progress == 1)
                Complete();
        });
    }

    private void Activate()
    {
        SceneHelper.Activate();
        dispatcher.RemoveListener(GameEvent.ACTIVATE_HANGAR, Activate);
    }

    private void Complete()
    {
        // destroy all root scene components
        var childCount = ContextView.transform.childCount;
        for (var i = childCount - 1; i >= 0; i--)
            GameObject.Destroy(ContextView.transform.GetChild(i).gameObject);

        dispatcher.Dispatch(CommandEvent.SHOW_HANGAR);
    }
}
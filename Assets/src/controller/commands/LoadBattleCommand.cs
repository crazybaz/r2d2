using Assets.src.core;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

public class LoadBattleCommand : EventCommand
{
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }

    public override void Execute()
    {
        dispatcher.AddListener(GameEvent.ACTIVATE_BATTLE, Activate);

        SceneHelper.Load(SceneHelper.BATTLE_SCENE, progress =>
        {
            if (progress <= 0.9)
                dispatcher.Dispatch(GameEvent.LOAD_BATTLE_PROGRESS, progress / 0.9f);

            if (progress == 1)
                dispatcher.Dispatch(CommandEvent.START_BATTLE);
        });
    }

    private void Activate()
    {
        SceneHelper.Activate();
        dispatcher.RemoveListener(GameEvent.ACTIVATE_BATTLE, Activate);
    }
}
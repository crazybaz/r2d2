using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

public class LoadDefsCommand : EventCommand
{
    [Inject]
    public DefinitionManager Defs { get; set; }

    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }

    public override void Execute()
    {
        Defs.Init();
        // TODO: load defs
        ContextView.GetComponent<ContextView>().StartCoroutine(MockLoadingProgress());
    }

    private IEnumerator MockLoadingProgress()
    {
        var p = 0f;
        while (p < 1)
        {
            dispatcher.Dispatch(GameEvent.LOAD_DEFS_PROGRESS, p);
            p += Random.Range(0.01f, 0.1f);
            yield return new WaitForSeconds(0.05f);
        }
        dispatcher.Dispatch(GameEvent.LOAD_DEFS_PROGRESS, 1f);
    }
}
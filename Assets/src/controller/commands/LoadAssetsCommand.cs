using System.Collections;
using Paladin.UI;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

public class LoadAssetsCommand : EventCommand
{
    [Inject]
    public IconManager IconManager { get; set; }

    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }

    public override void Execute()
    {
        IconManager.Init();
        // TODO: сделать систему загрузки ассетов и связку прогресса с UI
        ContextView.GetComponent<ContextView>().StartCoroutine(MockLoadingProgress());
    }

    private IEnumerator MockLoadingProgress()
    {
        var p = 0f;
        while (p < 1)
        {
            dispatcher.Dispatch(GameEvent.LOAD_ASSETS_PROGRESS, p);
            p += Random.Range(0.01f, 0.1f);
            yield return new WaitForSeconds(0.05f);
        }
        dispatcher.Dispatch(GameEvent.LOAD_ASSETS_PROGRESS, 1f);
    }
}
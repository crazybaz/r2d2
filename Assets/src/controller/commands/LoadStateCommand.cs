using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

public class LoadStateCommand : EventCommand
{
    [Inject]
    public Logic Logic { get; set; }

    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }

    public override void Execute()
    {
        // TODO: get original state data
        Logic.UpdateState(Resources.Load<TextAsset>("definitions/state").text);

        // TODO: load paladin state data, somewere
        /*Logic.Paladins = new Dictionary<PaladinType, PaladinModel>();

        var paladinData = stateData.GetField("paladin");
        foreach (var type in paladinData.keys)
        {
            var paladinType = DefinitionManager.EnumValue<PaladinType>(type);
            var paladinModel = injectionBinder.GetInstance<PaladinModel>().Init(paladinType, paladinData[type]);
            Logic.Paladins.Add(paladinType, paladinModel);
        }*/

        ContextView.GetComponent<ContextView>().StartCoroutine(MockLoadingProgress());
    }

    private IEnumerator MockLoadingProgress()
    {
        var p = 0f;
        while (p < 1)
        {
            dispatcher.Dispatch(GameEvent.LOAD_STATE_PROGRESS, p);
            p += Random.Range(0.01f, 0.1f);
            yield return new WaitForSeconds(0.05f);
        }
        dispatcher.Dispatch(GameEvent.LOAD_STATE_PROGRESS, 1f);
    }
}
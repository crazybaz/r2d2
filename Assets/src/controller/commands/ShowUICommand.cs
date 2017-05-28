using System;
using Paladin.UI;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

public class ShowUICommand : EventCommand
{
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }

    public override void Execute()
    {
        //Debug.Log(">> SHOW UI COMMAND " + ContextView);
        UIMediator.Show((String)evt.data).transform.SetParent(ContextView.transform);
    }
}

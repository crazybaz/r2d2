using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

public class UpgradePaladinCommand : EventCommand
{
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }

    [Inject]
    public Logic Logic { get; set; }

    public override void Execute()
    {
        // TODO: общаемся с сервером, запускаем процесс апгрейда узла паладина
       // Hashtable hash = (Hashtable) evt.data;
       // PaladinType type = (PaladinType) hash["paladin"];
       // NodeType node = (NodeType) hash["node"];
    }
}
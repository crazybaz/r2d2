using strange.extensions.command.impl;
using UnityEngine;

public class PrepareGameCommand : EventCommand
{
    public override void Execute()
    {
        Time.timeScale = 1.0f;
        iTween.Defaults.easeType = iTween.EaseType.easeInOutQuad;
    }
}
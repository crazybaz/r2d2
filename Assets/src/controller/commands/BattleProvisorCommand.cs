using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class BattleProvisorCommand : BaseProvisorCommand
{
    private float battleProgress;

    public override void Execute()
    {
        maxProgress = 1;
        dispatcher.AddListener(GameEvent.LOAD_BATTLE_PROGRESS, LoadBattleProgressHandler);
        base.Execute();
    }

    private void LoadBattleProgressHandler(IEvent e)
    {
        battleProgress = Mathf.Clamp01((float) e.data);
        //Debug.Log("BATTLE PROGRESS " + battleProgress);
        Update();
    }

    private void Update()
    {
        currentProgress = battleProgress;
        if (currentProgress > 0)
            UpdatePredictionTime();
    }

    protected override void Finish()
    {
        dispatcher.RemoveListener(GameEvent.LOAD_BATTLE_PROGRESS, LoadBattleProgressHandler);
    }
}
using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

public class BaseProvisorCommand : EventCommand
{
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }

    protected uint maxProgress = 1;

    protected float currentProgress;
    private float currentMockProgress;

    private double startMs;
    private double predictionMs = 2000;
    private const float updateProgressMs = 20;

    public override void Execute()
    {
        startMs = TimeHelper.GetCurrentMilliseconds;
        ContextView.GetComponent<ContextView>().StartCoroutine(UpdateProgress());
    }

    protected void UpdatePredictionTime()
    {
        if (currentProgress == maxProgress)
        {
            predictionMs = updateProgressMs * 20; // max 20 updates to complete mock progress
        }
        else
        {
            var deltaMs = TimeHelper.GetCurrentMilliseconds - startMs;
            predictionMs = maxProgress / currentProgress * deltaMs;
        }
    }

    protected IEnumerator UpdateProgress()
    {
        while (currentMockProgress < 1)
        {
            Debug.Log(">> PRELOADER PROGRESS " + currentMockProgress);
            dispatcher.Dispatch(GameEvent.PROVISOR_PROGRESS, currentMockProgress);

            currentMockProgress += (float)(updateProgressMs / predictionMs);

            yield return new WaitForSeconds(updateProgressMs / 1000f);
        }

        while (currentProgress != maxProgress)
            yield return null;

        Debug.Log(">> PRELOADER PROGRESS DONE");
        dispatcher.Dispatch(GameEvent.PROVISOR_PROGRESS, 1f);

        Finish();
    }

    protected virtual void Finish()
    {
    }
}
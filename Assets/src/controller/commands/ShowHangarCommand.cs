using Paladin.UI;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class ShowHangarCommand : EventCommand
{
    [Inject(ContextKeys.CROSS_CONTEXT_DISPATCHER)]
    public IEventDispatcher dispatcher{ get; set;}

    public override void Execute()
    {
        //ContextView.GetComponent<BattleRoot>().StartCoroutine(WaitGameScene(dispatcher));
        dispatcher.Dispatch(CommandEvent.SHOW_UI, UIPath.ExpScreen);
        dispatcher.Dispatch(CommandEvent.SHOW_UI, UIPath.HangarDialog);
    }

//    private IEnumerator WaitGameScene(IEventDispatcher eventDispatcher)
//    {
//        yield return new WaitForEndOfFrame();
//
//    }
}
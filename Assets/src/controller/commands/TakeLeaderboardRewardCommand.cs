using core;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

public class TakeLeaderboardRewardCommand : EventCommand
{
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }

    [Inject]
    public Logic Logic { get; set; }

    public override void Execute()
    {
        // TopVO topVO = (TopVO) evt.data;
        // Loot reward = topVO.Reward;
        // TODO: отправить с сообщением старт батл на сервер. Видимо перезапросить стейт придется, чтобы увидеть зачисленную награду
    }
}
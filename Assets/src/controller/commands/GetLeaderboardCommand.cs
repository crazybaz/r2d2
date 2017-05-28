using System.Collections.Generic;
using core;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

public class GetLeaderboardCommand : EventCommand
{
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }

    [Inject]
    public Logic Logic { get; set; }

    public override void Execute()
    {
        // TODO: отправить с сообщением старт батл на сервер.
        List<TopVO> friendTop;
        List<TopVO> localTop;
        List<TopVO> worldTop;

        Loot loot = new Loot();
        loot[LootType.Hard] = 25;
        friendTop = new List<TopVO>
        {
            new TopVO("Петух Петрович",100500,1),
            new TopVO("Джигурда Мукомол", 99995, 2),
            new TopVO("Плексиглаз Кирогазов", 90500, 3, loot),
            new TopVO("Дым без огня", 90000, 4),
            new TopVO("Толстый Фраер", 87877, 5),
            new TopVO("Красный Партизан", 86866, 6),
            new TopVO("Сладкий Пушкин", 55000, 7),
            new TopVO("Голый Пистолет", 1, 8)
        };
        localTop = new List<TopVO>
        {
            new TopVO("Чепушила Горьков", 100001, 1),
            new TopVO("Славный Малый", 99999, 2),
            new TopVO("Атас Бабаев", 89898, 3),
            new TopVO("Забой Кодоев", 75757, 4),
            new TopVO("Рулон Обоев", 70000, 5),
            new TopVO("Парад Евреев", 65666, 6),
            new TopVO("Осел Козлодоев", 50000, 7),
            new TopVO("Главный Врач", 45000, 8),
            new TopVO("Бледный Орк", 20000, 9)
        };
        worldTop = new List<TopVO>
        {
            new TopVO("Нет Друзей", 50000, 1),
            new TopVO("Хватит Терпеть", 45000, 2),
            new TopVO("Сладкий Хлеб", 40000, 3),
            new TopVO("Милый Друг", 37000, 4),
            new TopVO("Мокренькая Кисонька", 35000, 5),
            new TopVO("Паукан", 30000, 6),
            new TopVO("Пукан", 25000, 7),
            new TopVO("Бабай", 20000, 8),
            new TopVO("Мага Опасный", 5000, 9)
        };
        Logic.Leaderboard.WorldTop = worldTop;
        Logic.Leaderboard.LocalTop = localTop;
        Logic.Leaderboard.FriendTop = friendTop;
    }
}
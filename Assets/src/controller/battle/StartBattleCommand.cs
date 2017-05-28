using Paladin.UI;
using Paladin.View;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

namespace Paladin.Controller
{
    public class StartBattleCommand : EventCommand
    {
        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject ContextView { get; set; }

        [Inject]
        public GameLogic GameLogic { get; set; }

        public override void Execute()
        {
            var paladinPrefab = Resources.Load<GameObject>(StringEnum.GetStringValue(GameLogic.CurrentSession.Type));

            var paladin = Object.Instantiate(
                paladinPrefab,
                new Vector3(0, 0, Config.I.PLAYER_MOVEMENT_BOUNDS.zMin + 50),
                Quaternion.identity, ContextView.transform) as GameObject;

            // freeze paladin controlling
            paladin.GetComponent<PlayerInputController>().enabled = false;

            // start cutscene
            ContextView.GetComponentInChildren<StartCameraScene>().Play();

            // show top ui
            dispatcher.Dispatch(CommandEvent.SHOW_UI, UIPath.MainScreen);
        }
    }
}
using Assets.src.core;
using Paladin.UI;
using strange.extensions.command.impl;

namespace Paladin.Controller
{
    public class FinishBattleCommand : EventCommand
    {
        [Inject]
        public GameLogic GameLogic { get; set; }

        public override void Execute()
        {
            var isVictory = (bool)evt.data;

            GameLogic.Paladin.View.GetComponent<PlayerInputController>().enabled = false;

            UIMediator.Hide(UIPath.MainScreen);
            UIMediator.Show(UIPath.ResultDialog, FadeEffectType.Fade);
        }
    }
}
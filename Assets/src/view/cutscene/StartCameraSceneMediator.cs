using strange.extensions.context.api;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Paladin.View
{
    public class StartCameraSceneMediator : EventMediator
    {
        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject ContextView { get; set; }

        [Inject]
        public GameLogic GameLogic { get; set; }

        [Inject]
        public StartCameraScene View { get; set; }

        public override void OnRegister()
        {
            View.dispatcher.AddListener(ViewEvent.START, StartHandler);
            View.dispatcher.AddListener(ViewEvent.FINISH, FinishHandler);
            View.dispatcher.AddListener(ViewEvent.COMPLETE, CompleteHandler);
            View.Init(GameLogic);
        }

        public override void OnRemove()
        {
            View.dispatcher.RemoveListener(ViewEvent.START, StartHandler);
            View.dispatcher.RemoveListener(ViewEvent.FINISH, FinishHandler);
            View.dispatcher.RemoveListener(ViewEvent.COMPLETE, CompleteHandler);
        }

        private void StartHandler()
        {
            // стартуем картогенерацию
            ContextView.GetComponentInChildren<LevelGenerator>().enabled = true;
        }

        private void FinishHandler()
        {
            // открываем огонь
            dispatcher.Dispatch(GameEvent.START_SHOOTING);
        }

        private void CompleteHandler()
        {
            View.MainCamera.transform.parent = ContextView.transform;
            // теперь можно измерить размеры игрового пространства
            GameController.CalculateGameplayBounds();
            // и разрешить управление паладином
            GameLogic.Paladin.View.GetComponent<PlayerInputController>().enabled = true;
            Destroy(View.gameObject);
        }
    }
}
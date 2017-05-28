using strange.extensions.context.api;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Paladin.View
{
    public class LevelGeneratorMediator : EventMediator
    {
        [Inject]
        public GameLogic GameLogic { get; set; }

        [Inject]
        public LevelGenerator View { get; set; }

        [Inject]
        public LevelFactory Factory { get; set; }

        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject ContextView { get; set; }

        private bool freezeField;

        public override void OnRegister()
        {
            Factory.Init();
            View.Init(GameLogic.Defs.Config, Factory);
            View.dispatcher.AddListener(ViewEvent.BOSS_PATTERN_ARRIVED, BossPatternArrived);
        }

        public override void OnRemove()
        {
            View.dispatcher.RemoveListener(ViewEvent.BOSS_PATTERN_ARRIVED, BossPatternArrived);
        }

        private void BossPatternArrived()
        {
            freezeField = true;
            PlayerInputController.DefaultDirection = Vector3.zero;
        }

        private void Update()
        {
            // Slow down field movement
            if (freezeField && GameLogic.CurrentSession.FieldMoveSpeed > 0)
                GameLogic.ChangeFieldMoveSpeed(Mathf.Max(0, GameLogic.CurrentSession.FieldMoveSpeed - Time.deltaTime * 20));
        }
    }
}
using strange.extensions.mediation.impl;
using UnityEngine;
using SWS;

namespace Paladin.View
{
    public class StartCameraScene : EventView
    {
        public int MaxSpeed;
        public splineMove Mover;
        public Camera MainCamera;
        public float FinishRotationTime = 0.5f;

        private Quaternion startCameraRotation;
        private readonly Quaternion originalCameraRotation = Quaternion.Euler(47.0f, 0, 0);
        private float rotationTime;

        private CutsceneState state;
        private GameLogic gameLogic;

        public void Init(GameLogic gameLogic)
        {
            this.gameLogic = gameLogic;
            state = CutsceneState.Prepare;
        }

        public void Play()
        {
            // запускаем сплайн
            Mover.ChangeSpeed(50f);
            Mover.StartMove();

            // повернем камеру сразу на палыча
            MainCamera.transform.rotation = PaladinRotation;

            // переходим в состояние движения
            state = CutsceneState.Running;

            // сообщаем, что мы начали
            dispatcher.Dispatch(ViewEvent.START);

#if UNITY_EDITOR
            if (gameLogic.LocalConfig.SkipStartCameraScene)
            {
                Mover.GoToWaypoint(Mover.waypoints.Length);
                Complete();
            }
#endif
        }

        // called by sws callback
        public void Finish()
        {
            startCameraRotation = MainCamera.transform.rotation;
            state = CutsceneState.Targeting;
            dispatcher.Dispatch(ViewEvent.FINISH);
        }

        private void Complete()
        {
            Debug.Log(">> START CAMERA CUTSCENE COMPLETED");
            MainCamera.transform.rotation = originalCameraRotation;
            state = CutsceneState.Complete;
            dispatcher.Dispatch(ViewEvent.COMPLETE);
        }

        private void Update()
        {
            switch (state)
            {
                case CutsceneState.Running:
                    SpeedUp();
                    break;
                case CutsceneState.Targeting:
                    TakeOriginalPosition();
                    break;
            }
        }

        // Спецом для плавного движения камеры безо всяких Lerp
        private void LateUpdate()
        {
            switch (state)
            {
                case CutsceneState.Running:
                    LookAtPaladin();
                    break;
            }
        }

        private Quaternion PaladinRotation
        {
            get
            {
                return Quaternion.LookRotation(
                    gameLogic.Paladin.View.transform.position - MainCamera.transform.position + Config.I.PALADIN_HALF_HEIGHT_VECTOR
                );
            }
        }

        private void SpeedUp()
        {
            if (Mover.speed < MaxSpeed)
                Mover.ChangeSpeed(Mover.speed + Time.deltaTime * 20);
        }

        private void LookAtPaladin()
        {
            MainCamera.transform.rotation = PaladinRotation;
        }

        private void TakeOriginalPosition()
        {
            rotationTime += Time.deltaTime;
            MainCamera.transform.rotation = Quaternion.Slerp(startCameraRotation, originalCameraRotation, rotationTime / FinishRotationTime);

            if (rotationTime > FinishRotationTime)
                Complete();
        }
    }
}
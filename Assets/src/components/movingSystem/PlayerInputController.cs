using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    //public PaladinView Paladin;
    public ControlType controlType = ControlType.Absolute;

    private Vector3? targetPosition;
    private Vector3? deltaPosition;

    private PaladinView paladinView;
    private PaladinModel paladinModel;

    private static bool lastUpdate = false; // Необходим для синхронизации процесса фэйда на разных девайсах (в разной послеовательности MB приходят события Unity)
    private static bool isFirstTouch = true; // флаг для того, чтобы пока пальцом не нажал на экран, автоматически игра в паузу не переходила
    public static bool IsFirstTouch
    {
        set { isFirstTouch = value; lastUpdate = true; }
        get { return isFirstTouch; }
    }

    public static Vector3 CurrentVelocity { get { return currentVelocity; }}

    private static readonly Vector3 zeroVector = Vector3.zero;

    private static Vector3 currentVelocity = zeroVector;
    private static Vector3 targetVector;
    private static Camera mainCamera;

    public void Init(PaladinModel model)
    {
        paladinModel = model;
        paladinView = GetComponent<PaladinView>();
        mainCamera = Camera.main;
    }

    /*private void Start()
    {
//#if UNITY_EDITOR
//        var go = new GameObject();
//        var box = go.AddComponent<BoxCollider>();
//        box.size = new Vector3(Specs.I.GAMEPLAY_BOUNDS.width, 0, Specs.I.GAMEPLAY_BOUNDS.height);
//#endif
    }*/

    /*private void Awake() // TODO: ФПС ограничение (можно добавить в настройки, можно не добавлять)
    {
        QualitySettings.vSyncCount = 0; // VSync must be disabled
        Application.targetFrameRate = 15;
    }*/

    private void Update()
    {
        if (paladinModel == null)
            return;

        Process();
        ProcessPaladinRotation();

        lastUpdate = false;

        if (targetPosition == null) return;
        var pos = targetPosition.Value;

        pos.Set(
            Mathf.Clamp(pos.x, Config.I.PLAYER_MOVEMENT_BOUNDS.xMin, Config.I.PLAYER_MOVEMENT_BOUNDS.xMax),
            targetPosition.Value.y,
            Mathf.Clamp(pos.z, Config.I.PLAYER_MOVEMENT_BOUNDS.zMin, Config.I.PLAYER_MOVEMENT_BOUNDS.zMax)
        );

        if (!targetPosition.Value.Equals(transform.position))
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref currentVelocity, 0.05f, paladinModel.Speed);

        ProcessCamera();
    }

    private void Process()
    {
        Vector3? target = null;

        if (!lastUpdate)
            target = GetPosition();

        if (target == null)
        {
            targetPosition = null;
            deltaPosition = null;
            return;
        }

        if (controlType == ControlType.Absolute)
        {
            targetPosition = target;
        }
        else if (controlType == ControlType.Relative)
        {
            if (deltaPosition == null)
                deltaPosition = target - transform.position;

            targetPosition = target - deltaPosition;
        }
    }

    private static Vector3? GetPosition()
    {
        targetVector = zeroVector;

#if UNITY_EDITOR

        if (Input.GetMouseButton(0))
            targetVector = Input.mousePosition;
#else

        if (Input.touchCount > 0)
            targetVector = Input.GetTouch(0).position;

#endif
        if (!targetVector.Equals(zeroVector))
        {
            if (Config.I.TOUCH_RELEASE_MODE)
                IsFirstTouch = false;

            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(targetVector);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Config.I.FIELD_MASK))
            {
                targetVector.Set(hit.point.x, 0, hit.point.z);
                return targetVector;
            }
        }
        else if (Config.I.TOUCH_RELEASE_MODE)
        {
            if (!IsFirstTouch)
                GameController.FadeIn();
        }

        return null;
    }

    public static Vector3 DefaultDirection = Vector3.forward;
    private static readonly Quaternion zeroRotation = Quaternion.identity;
    private static readonly Vector3 forwardPosition = Vector3.forward;

    private void ProcessPaladinRotation()
    {
        var direction = targetPosition == null ? DefaultDirection : targetPosition.Value - transform.position;

        if (direction.magnitude < 0.1)
        {
            if (DefaultDirection == zeroVector)
                paladinView.TriggerAnimState(BattleState.Stay);

            return;
        }
        var angle = Vector3.Angle(forwardPosition, direction);

        if (paladinView.AnimState == BattleState.RunReverce)
        {
            direction.x *= -1;
            direction.z *= -1;
        }

        if (angle > 100 && paladinView.AnimState == BattleState.Run)
        {
            paladinView.TriggerAnimState(BattleState.RunReverce);
        }

        if (angle < 95 && (paladinView.AnimState == BattleState.RunReverce || paladinView.AnimState == BattleState.Stay))
        {
            paladinView.TriggerAnimState(BattleState.Run);
        }

        var targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 4f);

        paladinView.AnimTop.transform.rotation = zeroRotation;
    }

    private void ProcessCamera()
    {
        var cameraPos = mainCamera.transform.position;
        var playerPosX = transform.position.x;

        var cameraLimitX = playerPosX > 0 ? Config.I.CAMERA_MOVEMENT_BOUNDS.xMax : Config.I.CAMERA_MOVEMENT_BOUNDS.xMin;
        var playerLimitX = playerPosX > 0 ? Config.I.PLAYER_MOVEMENT_BOUNDS.xMax : Config.I.PLAYER_MOVEMENT_BOUNDS.xMin;

        cameraPos.Set(
            playerPosX * cameraLimitX / playerLimitX,
            cameraPos.y,
            cameraPos.z
        );

        mainCamera.transform.position = cameraPos;
    }
}
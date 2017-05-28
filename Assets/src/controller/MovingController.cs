using System;
using System.Collections.Generic;
using UnityEngine;

public class MovingController : MonoBehaviour
{
    private GameLogic gameLogic;
    private MovingConfig movingConfig;
    private BulletConfig bulletConfig;

    private Vector3? targetPosition; // позиция куда стреляем для снарядов, летящих навесом
    private Vector3 targetHalfPosition; // середина позиции куда стреляем для снарядов, летящих навесом
    private float maxHeight; // максимальная высота на которую поднимаются гранаты, летящие навесом
    private float startY; // изначальная высота эмиттера гранаты

    private Vector3? targetDirection; // нормализованый вектор направления стрельбы

    private GameObject targetObject; // целевоя объект (для наводящихся снарядов)
    private float lastTargetingTime;

    private Space space;

    private const float NORMALIZE_LEVEL_FLIGHT_SPEED = 0.2f;
    private const float LEVEL_FLIGHT_ERROR = 1.0f;

    public void Init(GameLogic gameLogic, MovingConfig movingConfig, BulletConfig bulletConfig = null)
    {
        Vector3 playerPosition;

        this.gameLogic = gameLogic;
        this.movingConfig = movingConfig;
        this.bulletConfig = bulletConfig;

        switch (this.movingConfig.MovingType)
        {
            case MovingType.Forward:
            case MovingType.Arc:
                targetDirection = Vector3.forward;
                space = Space.Self;
                break;
            case MovingType.PlayerTarget:
                playerPosition = gameLogic.Paladin.View.transform.position;
                playerPosition.y = transform.position.y;
                targetDirection = Vector3.Normalize(playerPosition - transform.position);
                space = Space.World;
                break;
            case MovingType.Prediction:
                // актуально для врагов, стреляющих по игроку. Вычисляется время полета пули до тек. положения игрока,
                // затем берется вектор его скорости и вычисляется в какой точке он будет через это время. Туда и летит пуля.
                playerPosition = gameLogic.Paladin.View.transform.position;
                float timeToPlayer = (playerPosition - transform.position).magnitude / this.movingConfig.Speed;
                Vector3 playerVelocity = PlayerInputController.CurrentVelocity;
                Vector3 playerEndPosition = playerPosition + playerVelocity * timeToPlayer;
                space = Space.World;
                targetDirection = Vector3.Normalize(playerEndPosition - transform.position);
                break;
            case MovingType.Parabolic:
                // пуля перекидывается псевдоанимацией через припятствия, в момент приземления у нее активируется коллайдер и скроллабл обжект.
                // будет ли она обязательно взрываться при приземлении - это галочка. Таким образом мы реализуем режим мины и режим гранаты.
                startY = transform.position.y;
                targetDirection = Vector3.forward;
                Vector3 tempPosition = transform.position;
                tempPosition.y = 0;
                targetPosition = tempPosition + transform.forward * this.movingConfig.Distance;
                targetHalfPosition = transform.position + (targetPosition.Value - transform.position) * 0.5f;
                targetHalfPosition.y = 0;
                maxHeight = (targetHalfPosition - tempPosition).magnitude;
                var explosionCollider = gameObject.GetComponent<CapsuleCollider>();
                if (explosionCollider != null) explosionCollider.enabled = false;
                space = Space.Self;
                break;
            case MovingType.Aiming:
                // если испускает враг, то цель ищется по овнер тайп - игрок, если испускаем мы, то цель ищется по овнер тайп - враг.
                // в заданном углу поиска ближайшую цель. Если угол пуска 360, то ближайшая может быть и сзади.
                targetObject = this.movingConfig.TargetObject ?? findTarget();
                lastTargetingTime = 0;
                targetDirection = Vector3.forward;
                space = Space.Self;
                break;
            default:
                throw new ArgumentException("Bullet moving type not configured");
        }
    }

    public void ChangeSpeed(float value)
    {
        movingConfig.Speed = value;
    }

    private void Update()
    {
        if (targetDirection != null)
        {
            transform.Translate(targetDirection.Value * movingConfig.Speed * Time.deltaTime, space);

            float distanceToFlightLevel = Config.I.PALADIN_HALF_HEIGHT - transform.position.y;
            if (Math.Abs(distanceToFlightLevel) > LEVEL_FLIGHT_ERROR)
                transform.Translate(0, distanceToFlightLevel * NORMALIZE_LEVEL_FLIGHT_SPEED, 0);
        }

        if (targetPosition != null)
        {
            Vector3 position = transform.position;
            position.y = 0;
            position.y = startY + (float)(maxHeight - Math.Pow((targetHalfPosition - position).magnitude / maxHeight, 2.0) * maxHeight);
            if (position.y < 0)
            {
                position.y = 0;
                targetDirection = null;
                targetPosition = null;
            }
            transform.position = position;
        }

        if (targetObject != null)
        {
            Quaternion temp = transform.rotation;
            Quaternion temp2 = transform.rotation;
            var position = targetObject.transform.position;
            position.y = Config.I.PALADIN_HALF_HEIGHT;
            temp2.SetLookRotation(position - transform.position);
            float delta = (temp2.eulerAngles.y - temp.eulerAngles.y);
            if (Math.Abs(delta) <= movingConfig.AngleSpeed * Time.deltaTime)
            {
                transform.LookAt(position);
            }
            else
            {
                int sign = 1;
                if (temp2.eulerAngles.y - temp.eulerAngles.y > 180.0f)
                    sign = -1;
                if (temp2.eulerAngles.y - temp.eulerAngles.y < 0 && temp2.eulerAngles.y - temp.eulerAngles.y > -180.0f)
                    sign = -1;

                transform.Rotate(0, sign * movingConfig.AngleSpeed * Time.deltaTime, 0);
            }
        }
        else if (movingConfig.MovingType == MovingType.Aiming)
        {
            lastTargetingTime += Time.deltaTime;
            if (lastTargetingTime >= movingConfig.AimingCooldown)
            {
                targetObject = findTarget();
                lastTargetingTime = 0;
            }
        }

        if (movingConfig.MovingType == MovingType.Arc && movingConfig.AngleSpeed != 0)
        {
            transform.Rotate(0, movingConfig.AngleSpeed * Time.deltaTime, 0);
        }
    }

    private GameObject findTarget()
    {
        Quaternion temp = transform.rotation;
        if (movingConfig.TargetKind == PawnKind.Paladin)
        {
            var player = gameLogic.Paladin.View.gameObject;

            var position = player.transform.position;
            position.y = Config.I.PALADIN_HALF_HEIGHT;
            temp.SetLookRotation(position - transform.position);

            if (temp.eulerAngles.y > 180.0f - movingConfig.AimingAngle * 0.5f && temp.eulerAngles.y < 180.0f + movingConfig.AimingAngle * 0.5f
                && movingConfig.DetectingRadius >= (position - transform.position).magnitude || movingConfig.DetectingRadius <= 0)
                return gameLogic.Paladin.View.gameObject;
            else
                return null;
        }
        else if (movingConfig.TargetKind == PawnKind.Enemy)
        {
            List<Pawn> pawnsInAngle = new List<Pawn>();
            // выберем все по углу
            foreach (var pawn in gameLogic.Pawns)
            {
                // не учитываем припятствия, если наша ракета их не должна аффектить
                if (pawn.Kind == PawnKind.Obstacle && bulletConfig != null && bulletConfig.ObstacleCollisionType == ObstacleCollisionType.Ignore)
                    continue;

                temp.SetLookRotation(pawn.transform.position - transform.position);

                if (Quaternion.Angle(temp, transform.rotation) <= movingConfig.AimingAngle)
                    pawnsInAngle.Add(pawn);
            }
            // отсортируем по дистанции
            if (pawnsInAngle.Count > 0)
            {
                Pawn nearestPawn = pawnsInAngle[0];
                for (int i = 1; i < pawnsInAngle.Count; i++)
                {
                    if ((pawnsInAngle[i].transform.position - transform.position).magnitude < (nearestPawn.transform.position - transform.position).magnitude)
                        nearestPawn = pawnsInAngle[i];
                }

                if (movingConfig.DetectingRadius >= (nearestPawn.transform.position - transform.position).magnitude)
                    return nearestPawn.gameObject;
                else
                    return null;
            }
            else return null;
        }
        return null;
    }

    private void OnDestroy()
    {
        gameLogic = null;
        movingConfig = null;
        bulletConfig = null;
        targetPosition = null;
        targetDirection = null;
        targetObject = null;
    }
}
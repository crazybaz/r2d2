using System;
using System.Collections.Generic;
using strange.extensions.context.impl;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.Events;

public enum WeaponState
{
    Reloading,
    Preparing,
    Shooting,
    Idle,
    None
}

public class BulletCreationOptions
{
    public GameObject BulletPrefab;
    public Vector3 Position;
    public Quaternion Rotation;
    public MovingConfig MovingConfig;
    public WeaponOwnerType OwnerType;
    public float Multiplier;
}

public class Weapon : EventView, IWeapon
{
    [Inject]
    public GameLogic GameLogic { get; set; }

    public WeaponInfo Info;
    public WeaponType Type { get; protected set; }
    public WeaponOwnerType OwnerType { get; private set; }

    [NonSerialized] public SuspensionType SuspensionType;

    public bool IsAutomatic = true;
    public List<Transform> Origins;
    public UnityEvent OnShot;
    public UnityEvent OnPrepare;
    public UnityEvent OnReload;
    [Range(0, 1)] public float StartShootDelay;

    public WeaponConfig Config;

    private WeaponState weaponState = WeaponState.None;
    private WeaponState lastState = WeaponState.None;
    private float delta;
    private int remainShots;
    private float configTime;

    public bool IsFreezed()
    {
        return weaponState == WeaponState.Idle;
    }

    public void Freeze()
    {
        if (IsFreezed()) return;

        lastState = weaponState;
        State = WeaponState.Idle;
    }

    public void Unfreeze()
    {
        if (lastState != WeaponState.None)
            State = lastState;
    }

    /// <summary>
    /// Инициализация. Приходит от медиатора. Необходимо держать в уме тот факт, что код, следующий за ассайном оружия (промисы), исполнится раньше, чем исполнится метод Init.
    /// При этом стейтом WeaponState.Reloading перезатрется то, что вы могли выставить раньше.
    /// </summary>
    public void Init(WeaponInfo info, WeaponConfig config, WeaponOwnerType ownerType)
    {
        Info = info;
        Config = config;
        OwnerType = ownerType;

        delta = Config.ReloadTime;
        delta -= StartShootDelay; // first reload increased by start shoot delay

        State = WeaponState.Reloading;
    }

    public void MakeShot()
    {
        foreach (var origin in Origins)
        {
            foreach (var shotData in Config.ShotConfig)
            {
                var rotation = origin.transform.rotation;
                rotation = Quaternion.Euler(0, rotation.eulerAngles.y + shotData.Orientation, 0);
                var bulletObject = Instantiate(shotData.Bullet, origin.position, rotation) as GameObject;

                var bullet = bulletObject.GetComponent<Bullet>();
                if (bullet == null)
                    throw new MissingComponentException("Bullet is missing");

                bullet.WeaponOwnerType = OwnerType;
                bullet.WeaponMultiplier = Config.Multiplier;
                bullet.MovingConfig = shotData.MovingConfig;

                bullet.transform.parent = (Context.firstContext.GetContextView() as GameObject).transform;

                if (OwnerType == WeaponOwnerType.Paladin)
                    GameLogic.Paladin.View.dispatcher.Dispatch(ViewEvent.BULLET_CREATED, bullet);
            }
        }

        if (OnShot != null)
            OnShot.Invoke();

        if (remainShots-- == 0)
            State = WeaponState.Reloading;
        else
            State = WeaponState.Shooting;

        delta = 0;
    }

    private void FixedUpdate()
    {
        if (IsFreezed()) return;
        if (Config == null) return;
        if (GameLogic.Paladin == null || GameLogic.Paladin.View.Disabled) return;
        if (global::Config.I.GAMEPLAY_BOUNDS.IsOutside(transform.position)) return;

        if (Info.AttackRadius > 0 && OwnerType == WeaponOwnerType.Enemy)
        {
            var playerPosition = GameLogic.Paladin.View.transform.position;
            if ((playerPosition - transform.position).magnitude > Info.AttackRadius)
                return;
        }

        delta += Time.deltaTime;
        if (delta >= configTime && weaponState != WeaponState.Preparing)
        {
            State = WeaponState.Preparing;
            if (IsAutomatic)
                MakeShot();
        }
    }

    private WeaponState State
    {
        set
        {
            if (weaponState == value)
                return;

            weaponState = value;

            var fireRateMultiplier = OwnerType == WeaponOwnerType.Paladin ? GameLogic.BuffModule.FireRateMultiplier : 1;

            switch (value)
            {
                case WeaponState.Reloading:
                    if (OnReload != null)
                        OnReload.Invoke();

                    configTime = Config.ReloadTime; // INFO: у паладина нет ReloadTime * fireRateMultiplier;
                    remainShots = Config.ShotCount - 1;
                    break;
                case WeaponState.Preparing:
                    if (OnPrepare != null)
                        OnPrepare.Invoke();
                    break;
                case WeaponState.Shooting:
                    configTime = Config.FireCooldown * fireRateMultiplier;
                    break;
            }
        }
    }
}
using System;
using UnityEngine;

public enum MovingType
{
    None, // не двигается, используется для мин
    Forward, // тупо летит вперед относительно своего контейнера
    PlayerTarget, // летит в точку где был игрок на момент выстрела
    Aiming, // наведение на игрока с инерцией
    Prediction, // прямой полет с предопределением, где будет находтся цель в момент подлета снаряда
    Parabolic, // миноArcментый навес (снаряд активирует свой колайдер при ударении о землю)
    Arc
}

[Serializable]
public class MovingConfig
{
    public float Speed;
    public uint Distance = 0;
    public PawnKind TargetKind;
    public GameObject TargetObject;
    public MovingType MovingType = MovingType.Forward;

    // если снаряд наводящийся, укажем характеристику наведения.
    [RangeAttribute(-180f, 180f)]
    public int AngleSpeed = 0;  // К примеру 10 - снаряд чуть накланяется за целью и пролетает мимо, 180 - снаряд следует за целью.
    public int DetectingRadius = 160;
    public float AimingAngle = 45f;
    public float AimingCooldown = 1f;
}
using System;
using strange.extensions.mediation.impl;
using UnityEngine;

public class WeaponMediator : EventMediator
{
    [Inject]
    public Logic Logic { get; set; }

    [Inject]
    public IWeapon View { get; set; }

    public override void OnRegister()
    {
        var ownerType = GetOwnerType();
        GetComponent<IWeaponInfo>().WeaponInfoPromise.Register(View.Type, weaponInfo =>
        {
            var config = Logic.Defs.GetWeapon(weaponInfo.Type, weaponInfo.Level, ownerType);
            (View as Weapon).Init(weaponInfo, config, ownerType);
        });
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }

    private WeaponOwnerType GetOwnerType()
    {
        var pawn = GetComponent<IPawn>();
        switch (pawn.Kind)
        {
            case PawnKind.FlyingEnemy:
            case PawnKind.Enemy:
                return WeaponOwnerType.Enemy;
            case PawnKind.Paladin:
                return WeaponOwnerType.Paladin;
            case PawnKind.Obstacle:
                throw new ArgumentException("Obstacle with weapon is forbidden");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
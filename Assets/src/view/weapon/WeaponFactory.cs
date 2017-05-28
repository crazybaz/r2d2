using System;

public class WeaponFactory
{
    public static Type GetWeapon(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Machinegun:
                return typeof(Machinegun);
            case WeaponType.Laser:
                return typeof(Laser);
            case WeaponType.Cannon:
                return typeof(Cannon);
            case WeaponType.PulseCannon:
                return typeof(PulseCannon);
            case WeaponType.ShockCannon:
                return typeof(ShockCannon);
            case WeaponType.Missile:
                return typeof(Missile);
            case WeaponType.GrenadeLauncher:
                return typeof(GrenadeLauncher);
            case WeaponType.MissileSystem:
                return typeof(MissileSystem);
            case WeaponType.RocketLauncher:
                return typeof(RocketLauncher);
            case WeaponType.Miner:
                return typeof(Miner);

            // not implemented
            case WeaponType.WaveCannon:
            case WeaponType.PlasmaCannon:
            case WeaponType.BoloCannon:
            case WeaponType.HeavyPlasma:
            case WeaponType.Electra:
                return null;

            default:
                throw new ArgumentOutOfRangeException("Weapon not implemented");
        }
    }
}
public interface IWeaponInfo
{
    Promise<WeaponType, WeaponInfo> WeaponInfoPromise { get; }
}
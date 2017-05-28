using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class WeaponInfo
{
    public WeaponType Type;
    public uint Level;
    public uint AttackRadius;
}

/**
 * Промежуточная сущность для ребят с оружием
 */
public class BattlePawn : Pawn, IBattlePawn, IWeaponInfo
{
    protected Weapon[] weapons;

    public Promise<WeaponType, WeaponInfo> WeaponInfoPromise { get; private set; }

    private float prepareAnimLength = 0;
    public float PrepareAnimLength
    {
        set { prepareAnimLength = value; }
        get { return prepareAnimLength; }
    }

    public virtual bool WeaponsEnabled
    {
        get { return weapons.First().enabled; }
        set
        {
            foreach (var weapon in weapons)
                weapon.enabled = value;
        }
    }

    public BattlePawn()
    {
        WeaponInfoPromise = new Promise<WeaponType, WeaponInfo>();
    }

    protected override void Init()
    {
        base.Init();

        MoverRequired = true;
        AnimatorRequired = true;

        weapons = gameObject.GetComponents<Weapon>();
        if (weapons == null || weapons.Length == 0)
            throw new MissingComponentException("No one weapon found for BattlePawn");

        WeaponInfoPromise.Assign(GetWeaponInfo);
    }

    public virtual void StartAttack()
    {
        throw new NotImplementedException(this + " do not implement StartAttack handler, plz change enemy type or path config");
    }

    public virtual void StopAttack()
    {
        throw new NotImplementedException(this + " do not implement StopAttack handler, plz change enemy type or path config");
    }

    protected void ProcessShootingCoroutine(string prepareAnim = BattleStateMachine.STATE_PREPARE,
        string shotAnim = BattleStateMachine.STATE_ATTACK, Weapon weapon = null, int animLevel = 0)
    {
        Anim.Play(prepareAnim);
        StartCoroutine(WaitForShooting(shotAnim, weapon, animLevel));
    }

    private IEnumerator WaitForShooting(string shotAnim, Weapon weapon, int animLevel)
    {
        yield return WaitForEndCurrentAnim(animLevel);

        if (weapon == null)
            weapons[0].MakeShot();
         else
            weapon.MakeShot();

        Anim.Play(shotAnim);
    }

    protected IEnumerator WaitForEndCurrentAnim(int animLevel = 0)
    {
        yield return new WaitForEndOfFrame();

        var length = Anim.GetCurrentAnimatorStateInfo(animLevel).length;

        // TODO: Внимание! В режиме Scene корутина не работает

        yield return new WaitForSeconds(length);
    }

    private WeaponInfo GetWeaponInfo(WeaponType weaponType)
    {
        foreach (var weaponInfo in Config.WeaponInfo)
        {
            if (weaponInfo.Type == weaponType)
                return weaponInfo;
        }

        throw new Exception("Config not found for weapon " + weaponType + " in " + this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        weapons = null;
    }
}
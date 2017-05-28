using System.Collections.Generic;
using Paladin.View;
using UnityEngine;

public class LightTank : BattlePawn
{
    [SerializeField] private List<Transform> specialOrigins;

    private bool lastRocketShotLeft = true;
    private Weapon rocketLauncher;

    protected override void Init()
    {
        base.Init();
        foreach (var weapon in weapons)
            if (weapon.Type == WeaponType.RocketLauncher)
                rocketLauncher = weapon;

        if (immovable)
        {
            ScrollableTexture track = GetComponentInChildren<ScrollableTexture>();
            if (track != null)
                track.enabled = false;
        }
    }

    public void ShotHandler()
    {
        switch (Type)
        {
            case PawnType.Tank1:
                Anim.Play(BattleStateMachine.STATE_ATTACK);
                break;
            case PawnType.Tank2:
                Anim.Play(BattleStateMachine.STATE_ATTACK);
                break;
            case PawnType.Tank3:
                Anim.Play(BattleStateMachine.STATE_SHOT3);
                break;
            case PawnType.Tank4:
                Anim.Play(BattleStateMachine.STATE_SHOT4);
                break;
        }
    }

    public void PrepareRocketHandler()
    {
        if (lastRocketShotLeft)
        {
            foreach (var weapon in weapons)
            {
                if (weapon.Type == WeaponType.RocketLauncher)
                {
                    weapon.Origins.Clear();
                    weapon.Origins.Add(specialOrigins[0]);
                    break;
                }
            }
            ProcessShootingCoroutine(BattleStateMachine.STATE_PREPARE_SPECIAL_LEFT, BattleStateMachine.STATE_SHOT_SPECIAL_LEFT, rocketLauncher);
            lastRocketShotLeft = false;
        }
        else
        {
            foreach (var weapon in weapons)
            {
                if (weapon.Type == WeaponType.RocketLauncher)
                {
                    weapon.Origins.Clear();
                    weapon.Origins.Add(specialOrigins[1]);
                    break;
                }
            }
            ProcessShootingCoroutine(BattleStateMachine.STATE_PREPARE_SPECIAL_RIGHT, BattleStateMachine.STATE_SHOT_SPECIAL_RIGHT, rocketLauncher);
            lastRocketShotLeft = true;
        }
    }
}
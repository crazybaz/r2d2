using UnityEngine;

public class EnemyBunker : BattlePawn
{
    [SerializeField] protected GameObject fireEffect;

    protected override void Init()
    {
        base.Init();
        MoverRequired = false;
    }

    public void ShotHandler()
    {
        Anim.Play(BattleStateMachine.STATE_ATTACK);
        Instantiate(fireEffect, weapons[0].Origins[0], false);
    }

    public void ShotSpecialLeftHandler()
    {
        Anim.Play(BattleStateMachine.STATE_SHOT_SPECIAL_LEFT);
    }

    public void ShotSpecialRightHandler()
    {
        Anim.Play(BattleStateMachine.STATE_SHOT_SPECIAL_RIGHT);
    }

    public void PrepareBunker4Handler()
    {
        ProcessShootingCoroutine(BattleStateMachine.STATE_PREPARE_SHOT4, BattleStateMachine.STATE_SHOT4);
    }
}
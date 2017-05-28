using UnityEngine;

public class MammothTank : HeavyTank
{
    [SerializeField] private Weapon missileLeft;
    [SerializeField] private Weapon missileRight;

    private bool isAlreadyPoint = false;
    private bool isAlreadyPreparingLeft = false;
    private bool isAlreadyPreparingRight = false;

    public override void StartAttack()
    {
        isAlreadyPoint = true;

        Mover.Pause();

        if (isAlreadyPreparingLeft) // если мы ходили и как бы пришло время стрелять, но заморозили оружие, чтобы стрелять именно с спец. позиции
        {
            missileLeft.Unfreeze();
            ProcessShootingCoroutine(BattleStateMachine.STATE_PREPARE_SPECIAL_LEFT, BattleStateMachine.STATE_SHOT_SPECIAL_LEFT, missileLeft);
        }
        if (isAlreadyPreparingRight)
        {
            missileRight.Unfreeze();
            ProcessShootingCoroutine(BattleStateMachine.STATE_PREPARE_SPECIAL_RIGHT, BattleStateMachine.STATE_SHOT_SPECIAL_RIGHT, missileRight);
        }
    }

    public override void StopAttack()
    {
        isAlreadyPreparingLeft = isAlreadyPreparingRight = isAlreadyPoint = false;
        Mover.Resume();
    }

    public void SpecialPrepareLeftHandler()
    {
        isAlreadyPreparingLeft = true;

        if (!isAlreadyPoint)
            missileLeft.Freeze();
        else
            ProcessShootingCoroutine(BattleStateMachine.STATE_PREPARE_SPECIAL_LEFT, BattleStateMachine.STATE_SHOT_SPECIAL_LEFT, missileLeft);
    }

    public void SpecialPrepareRightHandler()
    {
        isAlreadyPreparingRight = true;
        if (!isAlreadyPoint)
            missileRight.Freeze();
        else
            ProcessShootingCoroutine(BattleStateMachine.STATE_PREPARE_SPECIAL_RIGHT, BattleStateMachine.STATE_SHOT_SPECIAL_RIGHT, missileRight);
    }
}
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiner : BattlePawn
{
    [Header("Минимальное время между расстановкой мин")]
    [SerializeField] private float minMinePeriod = 1.0f;

    [Header("Максимальное время между расстановкой мин")]
    [SerializeField] private float maxMinePeriod = 3.0f;

    private float minePeriod;
    private float mineCooldown;

    private bool isAlreadyAtPoint;
    private bool isAlreadyPrepared;

    protected override void Init()
    {
        base.Init();

        transitions.AddRange(new List<BattleStateTransition>
        {
            new BattleStateTransition(BattleState.Run, BattleState.Prepare, RunMineHandler),
            new BattleStateTransition(BattleState.Fire, BattleState.Run, MineRunHandler),
            new BattleStateTransition(BattleState.Fire, BattleState.Fire, MineMineHandler)
        });

        minePeriod = CalcMinePeriod();
    }

    private float CalcMinePeriod()
    {
        return Random.Range(minMinePeriod, maxMinePeriod);
    }

    protected override void Update()
    {
        base.Update();

        if (nextState == BattleState.Run)
            mineCooldown += Time.deltaTime;

        if (mineCooldown >= minePeriod)
        {
            mineCooldown = 0;
            minePeriod = CalcMinePeriod();

            Mover.Pause();
            isAlreadyAtPoint = true;
            Anim.SetTrigger(BattleStateMachine.TRIGGER_RUN_PREPARE);
            nextState = BattleState.Prepare;
        }
    }

    // ============= transitions =============

    private void MineRunHandler()
    {
        Mover.Resume();
        isAlreadyAtPoint = false;
        isAlreadyPrepared = false;
    }

    private void RunMineHandler()
    {
        if (weapons[0].IsFreezed())
            weapons[0].Unfreeze();
        nextState = BattleState.Fire;
    }

    private void MineMineHandler()
    {
        if (isAlreadyPrepared)
        {
            weapons[0].MakeShot();
            isAlreadyPrepared = false;
        }
        nextState = BattleState.Run;
    }

    // ============= from weapon =============

    public void PrepareHandler()
    {
        isAlreadyPrepared = true;
        if (!isAlreadyAtPoint)
        {
            weapons[0].Freeze();
        }
    }

    public void ReloadHandler()
    {
        nextState = BattleState.Run;
        Anim.ResetTrigger(BattleStateMachine.TRIGGER_RUN_PREPARE);
    }
}
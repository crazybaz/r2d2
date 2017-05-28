using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMite : Pawn, IMeleePawn
{
    [Header("Величина, на которую увеличивается скорость при анимации roll")]
    [SerializeField] private float rollSpeedBoost = 15;

    [Header("Время нахождения в закопанном состоянии, секунды")]
    [SerializeField] private float digTime = 1;

    protected override void Init()
    {
        base.Init();

        transitions.AddRange(new List<BattleStateTransition>
        {
            new BattleStateTransition(BattleState.Run, BattleState.DigIn, RunDigInHandler),
            new BattleStateTransition(BattleState.DigIn, BattleState.DigRun, DigInRunHandler),
            new BattleStateTransition(BattleState.DigIn, BattleState.DigOut, DigInOutHandler),
            new BattleStateTransition(BattleState.Run, BattleState.Run, RunHandler),
            new BattleStateTransition(BattleState.Run, BattleState.Roll, RunRollHandler),
            new BattleStateTransition(BattleState.Roll, BattleState.Run, RollRunHandler)
        });
    }

    public void DiggIn()
    {
        nextState = BattleState.DigIn;
    }

    public void DiggOut()
    {
        nextState = BattleState.DigOut;
    }

    public void StartRoll()
    {
        nextState = BattleState.Roll;
    }

    public void EndRoll()
    {
        nextState = BattleState.Run;
    }

    private void RunDigInHandler()
    {
        Anim.SetTrigger(BattleStateMachine.TRIGGER_RUN_DIG);
        nextState = BattleState.DigRun;
    }

    private void DigInRunHandler()
    {
        Mover.Pause();
        GetComponent<Collider>().enabled = false;
        StartCoroutine(DigInWait());
        nextState = BattleState.None;
    }

    private IEnumerator DigInWait()
    {
        yield return new WaitForSeconds(digTime);
        Mover.Resume();
    }

    private void DigInOutHandler()
    {
        Mover.Pause();
        GetComponent<Collider>().enabled = true;
        Anim.ResetTrigger(BattleStateMachine.TRIGGER_RUN_DIG);
        Anim.Play(BattleStateMachine.STATE_DIG_OUT);
        nextState = BattleState.Run;
    }

    private void RunHandler()
    {
        Mover.Resume();
        Anim.ResetTrigger(BattleStateMachine.TRIGGER_ROLL_END);
        nextState = BattleState.None;
    }

    private void RunRollHandler()
    {
        dispatcher.Dispatch(ViewEvent.BOOST_SPEED, rollSpeedBoost);
        Anim.SetTrigger(BattleStateMachine.TRIGGER_RUN_ROLL);
        nextState = BattleState.None;
    }

    private void RollRunHandler()
    {
        dispatcher.Dispatch(ViewEvent.BOOST_SPEED, -rollSpeedBoost);
        Anim.ResetTrigger(BattleStateMachine.TRIGGER_RUN_ROLL);
        Anim.SetTrigger(BattleStateMachine.TRIGGER_ROLL_END);
        nextState = BattleState.None;
    }
}
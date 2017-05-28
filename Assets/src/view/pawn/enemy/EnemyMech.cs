using System.Collections;
using System.Collections.Generic;
using Paladin.View;
using UnityEngine;

public class EnemyMech : HumanoidEnemy
{
    [Inject]
    public GameLogic GameLogc { get; set; }

    public Transform top;
    private bool isAlreadyPoint = false;
    [SerializeField] private float targetingSpeed = 8;
    [SerializeField] private float targetingAngle = 1; // Угол наведения, достаточный для перехода к BattleState.Fire
    [SerializeField] private PlayerTargeting targeting;

    protected override void Init()
    {
        base.Init();

        transitions.AddRange(new List<BattleStateTransition>
        {
            new BattleStateTransition(BattleState.Run, BattleState.Run, RunRunHandler),
            new BattleStateTransition(BattleState.Run, BattleState.Stay, RunStayHandler),
            new BattleStateTransition(BattleState.Stay, BattleState.Targeting, StayTargetingHandler),
            new BattleStateTransition(BattleState.Stay, BattleState.Fire, StayFireHandler),
            new BattleStateTransition(BattleState.Stay, BattleState.Run, StayRunHandler),
            new BattleStateTransition(BattleState.Death, BattleState.Disappear, DeathDisappearHandler)
        });

        targeting.enabled = false;
    }

    public void PrepareHandler()
    {
        if (isAlreadyPoint)
            weapons[0].MakeShot();
        else
            weapons[0].Freeze();
    }

    public override void ShotHandler()
    {
        nextState = BattleState.Fire;
    }

    public void ReloadHandler()
    {
        nextState = BattleState.Run;
    }

    public override void StartAttack()
    {
        nextState = BattleState.Stay;
    }

    public override void DestroyAnim()
    {
        nextState = BattleState.Death;
    }

    private void RunRunHandler()
    {
        // move head by movement direction
        top.transform.rotation = Quaternion.Slerp(top.transform.rotation, transform.rotation, targetingSpeed * Time.deltaTime * .5f);
        targeting.enabled = false;
    }

    private void RunStayHandler()
    {
        StartCoroutine(MoverPause());
        if (nextState == BattleState.Stay) // чтобы отработало лишь единожды
        {
            Anim.SetTrigger(BattleStateMachine.TRIGGER_RUN_STAY);
            AnimTop.SetTrigger(BattleStateMachine.TRIGGER_RUN_STAY);
            Anim.ResetTrigger(BattleStateMachine.TRIGGER_STAY_RUN);
            AnimTop.ResetTrigger(BattleStateMachine.TRIGGER_STAY_RUN);
            nextState = BattleState.Targeting;
        }
    }

    private void StayTargetingHandler()
    {
        // targeting
        if (GameLogc.Paladin.View.Disabled)
            return;

        var targetRotation = Quaternion.LookRotation(GameLogc.Paladin.View.transform.position - top.transform.position);
        targeting.enabled = true;

        if (Quaternion.Angle(targetRotation, top.transform.rotation) < targetingAngle)
        {
            if (weapons[0].IsFreezed())
                weapons[0].Unfreeze();

            isAlreadyPoint = true;
        }
    }

    private void StayFireHandler()
    {
        Anim.Play(BattleStateMachine.STATE_ATTACK);
        AnimTop.Play(BattleStateMachine.STATE_ATTACK);
        nextState = BattleState.Targeting;
    }

    private void StayRunHandler()
    {
        if (isAlreadyPoint) // чтобы отработало лишь единожды
        {
            isAlreadyPoint = false;
            Anim.SetTrigger(BattleStateMachine.TRIGGER_STAY_RUN);
            Anim.ResetTrigger(BattleStateMachine.TRIGGER_RUN_STAY);
            AnimTop.SetTrigger(BattleStateMachine.TRIGGER_STAY_RUN);
            AnimTop.SetTrigger(BattleStateMachine.TRIGGER_RUN_STAY);
            StartCoroutine(MoverResume());
        }
    }

    /// <summary>
    /// это для более адекватного перехода от стояния к ходьбе, так как без этого по логике аниматора пока не отыграется анимация стояния
    /// переход к анимации ходьбя не будет осуществлен, таким образом, если без корутины выставить Mover.Resume, то чувак начинает двигаться, хотя
    /// к анимации движения он перейдет только через пару секунд. Выглядит плохо.
    /// </summary>
    private IEnumerator MoverResume()
    {
        yield return WaitForEndCurrentAnim();
        Mover.Resume();
    }

    // см. предыдущее описание метода
    private IEnumerator MoverPause()
    {
        yield return WaitForEndCurrentAnim();
        Mover.Pause();
    }

    private void DeathDisappearHandler()
    {
        StartCoroutine(WaitAndFadeOut());
        nextState = BattleState.Disabled;
    }

    private IEnumerator WaitAndFadeOut()
    {
        yield return new WaitForSeconds(0.6f);
        FadeOut();
    }

    protected override void OnDestroy()
    {
        top = null;
        targeting = null;
        base.OnDestroy();
    }
}
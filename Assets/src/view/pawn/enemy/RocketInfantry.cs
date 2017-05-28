using System.Collections.Generic;

public class RocketInfantry : HumanoidEnemy
{
    private bool isAlreadyPoint = false;
    private bool isAlreadyPreparing = false;

    protected override void Init()
    {
        base.Init();

        transitions.AddRange(new List<BattleStateTransition>
        {
            new BattleStateTransition(BattleState.Shot3, BattleState.Run, Shot3RunHandler)
        });
    }

    public override void StartAttack()
    {
        isAlreadyPoint = true;

        Mover.Pause();

        if (isAlreadyPreparing) // если мы ходили и как бы пришло время стрелять, но заморозили оружие, чтобы стрелять именно отсюда
        {
            weapons[0].Unfreeze();
            Shoot();
        }
        else Prepare(); // разрешаем зациклить анимацию подготовки как только подошли
        // иначе стоим и ждем пока можно будет стрелять
    }

    public void PrepareHandler()
    {
        isAlreadyPreparing = true;

        if (isAlreadyPoint) // если я уже стоял, то могу показывать анимацию длиной PrepareAnimLength с самого начала и попаду как раз к моменту выстрела
            Shoot();
        else
            weapons[0].Freeze();
    }

    private void Prepare()
    {
        Anim.SetTrigger(BattleStateMachine.TRIGGER_RUN_PREPARE3);
        AnimTop.SetTrigger(BattleStateMachine.TRIGGER_RUN_PREPARE3);
    }

    private void Shoot()
    {
        Anim.Play(BattleStateMachine.STATE_PREPARE_SHOT3, -1, 0);
        AnimTop.Play(BattleStateMachine.STATE_PREPARE_SHOT3, -1, 0);
        Anim.SetTrigger(BattleStateMachine.TRIGGER_PREPARE_SHOT3);
        AnimTop.SetTrigger(BattleStateMachine.TRIGGER_PREPARE_SHOT3);
        nextState = BattleState.Run;
    }

    private void Shot3RunHandler()
    {
        nextState = BattleState.None; // для того, чтобы не сыпалось постоянно Shot3RunHandler
        Mover.Resume();
        Anim.ResetTrigger(BattleStateMachine.TRIGGER_RUN_PREPARE3);
        AnimTop.ResetTrigger(BattleStateMachine.TRIGGER_RUN_PREPARE3);
        Anim.ResetTrigger(BattleStateMachine.TRIGGER_PREPARE_SHOT3);
        AnimTop.ResetTrigger(BattleStateMachine.TRIGGER_PREPARE_SHOT3);
        isAlreadyPreparing = false;
        isAlreadyPoint = false;
    }
}
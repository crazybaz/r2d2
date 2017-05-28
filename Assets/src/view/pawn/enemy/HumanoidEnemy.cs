using UnityEngine;

public class HumanoidEnemy : BattlePawn
{
    [SerializeField] public Animator AnimTop;

    protected override void Init()
    {
        base.Init();
        if (immovable)
        {
            for (int i = 0; i < AnimTop.layerCount; i++)
                AnimTop.Play(BattleStateMachine.STATE_STAY, i);
        }
    }

    public virtual void ShotHandler()
    {
        switch (Type)
        {
            case PawnType.Trooper1:
                AnimTop.Play(BattleStateMachine.STATE_SHOT1);
                break;
            case PawnType.Trooper2:
                AnimTop.Play(BattleStateMachine.STATE_SHOT2);
                break;
            case PawnType.Mech1:
                AnimTop.Play(BattleStateMachine.STATE_SHOT1);
                AnimTop.Play(BattleStateMachine.STATE_SHOT2, -1, -0.10f);
                AnimTop.Play(BattleStateMachine.STATE_SHOT1, -1, -0.20f);
                AnimTop.Play(BattleStateMachine.STATE_SHOT2, -1, -0.30f);
                break;
            default:
                AnimTop.Play(BattleStateMachine.STATE_ATTACK);
                break;
        }
    }

    protected override void AnyDeathHandler()
    {
        base.AnyDeathHandler();
        AnimTop.SetTrigger(BattleStateMachine.TRIGGER_ANY_DEATH);
    }

    protected override void OnDestroy()
    {
        AnimTop = null;
        base.OnDestroy();
    }
}
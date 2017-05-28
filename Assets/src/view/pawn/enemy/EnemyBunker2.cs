using UnityEngine;

public class EnemyBunker2 : EnemyBunker
{
    [SerializeField] private Weapon weapon1;
    [SerializeField] private Weapon weapon2;
    [SerializeField] private Weapon weapon3;
    [SerializeField] private Weapon weapon4;

    public void Shot1Handler()
    {
        Anim.Play(BattleStateMachine.STATE_SHOT1);
        Instantiate(fireEffect, weapon1.Origins[0], false);
    }

    public void Shot2Handler()
    {
        Anim.Play(BattleStateMachine.STATE_SHOT22);
        Instantiate(fireEffect, weapon2.Origins[0], false);
    }

    public void Shot3Handler()
    {
        Anim.Play(BattleStateMachine.STATE_SHOT23);
        Instantiate(fireEffect, weapon3.Origins[0], false);
    }

    public void Shot4Handler()
    {
        Anim.Play(BattleStateMachine.STATE_SHOT24);
        Instantiate(fireEffect, weapon4.Origins[0], false);
    }
}
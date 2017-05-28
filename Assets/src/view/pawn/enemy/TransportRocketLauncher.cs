using System.Collections.Generic;
using UnityEngine;

public class TransportRocketLauncher : Transport
{
    [SerializeField] private List<Transform> specialOrigins;

    private int rocketNum = 0;

    public void PrepareRocketHandler()
    {
        weapons[0].Origins.Clear();
        weapons[0].Origins.Add(specialOrigins[rocketNum]);

        rocketNum ++;
        switch (rocketNum)
        {
            case 1:
                Anim.Play(BattleStateMachine.STATE_SHOT3);
                break;
            case 2:
            case 3:
                Anim.Play("shot3"+rocketNum);
                break;
            case 4:
                Anim.Play("shot3"+rocketNum);
                rocketNum = 0;
            break;
        }

        weapons[0].MakeShot();
    }
}
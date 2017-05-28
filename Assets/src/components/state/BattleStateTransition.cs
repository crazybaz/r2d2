using UnityEngine;

public class BattleStateTransition
{
    public delegate void Handler();

    private readonly BattleState inputState;
    private readonly BattleState outputState;
    private readonly Handler handler;

    public BattleStateTransition(BattleState inputState, BattleState outputState, Handler handler)
    {
        this.inputState = inputState;
        this.outputState = outputState;
        this.handler = handler;
    }

    public bool Match(BattleState inState, BattleState outState)
    {
        if ((inputState == BattleState.Any || inState == inputState) && (outputState == BattleState.Any || outState == outputState))
        {
            // Debug.Log(handler.Method.Name);
            handler();
            return true;
        }
        return false;
    }
}
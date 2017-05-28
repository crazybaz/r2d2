using Paladin.View;

public class HeavyTank : BattlePawn
{
    protected override void Init()
    {
        base.Init();
        if (immovable)
        {
            ScrollableTexture track = GetComponentInChildren<ScrollableTexture>();
            if (track != null)
                track.enabled = false;
        }
    }

    public void ShotHandler()
    {
        Anim.Play(BattleStateMachine.STATE_ATTACK);
    }

    public void ShotLeftHandler()
    {
        Anim.Play(BattleStateMachine.STATE_SHOT13LEFT);
    }

    public void ShotRightHandler()
    {
        Anim.Play(BattleStateMachine.STATE_SHOT13RIGHT);
    }
}
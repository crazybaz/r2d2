public class SuicideEnemy : Pawn
{
    private MovingController mover;

    protected override void Init()
    {
        base.Init();

        mover = GetComponent<MovingController>();
        mover.Init(GameLogic, new MovingConfig
        {
            MovingType = MovingType.Aiming,
            AngleSpeed = 80,
            AimingAngle = 360,
            DetectingRadius = 0,
            Speed = Config.MaxSpeed,
            TargetKind = PawnKind.Paladin
        });
    }

    public override void ChangeSpeed(float value)
    {
        base.ChangeSpeed(value);
        if (mover != null)
            mover.ChangeSpeed(value);
    }

    /*public override void ResetSpeed()
    {
        base.ResetSpeed();
        if (mover != null)
            mover.MovingConfig.Speed = Speed;
    }*/
}
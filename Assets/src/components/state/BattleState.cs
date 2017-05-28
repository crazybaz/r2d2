public enum BattleState
{
    // особые стейты
    Any,
    None,

    // анимационные стейты
    Appear,
    Death,
    DigIn, DigOut,
    Fire,
    Landing, LandingEnd,
    Prepare, PrepareShot3, PrepareShot4, PrepareSpecialLeft, PrepareSpecialRight,
    Reload,
    Roll, RollEnd,
    Run, RunReverce,
    Shot1, Shot3, Shot4, ShotSpecialLeft, ShotSpecialRight,
    Stay,

    // не анимационные стейты
    DigRun,
    Disappear,
    Disabled,
    Targeting
}
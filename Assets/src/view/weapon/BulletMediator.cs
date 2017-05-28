using strange.extensions.mediation.impl;

public class BulletMediator : EventMediator
{
    [Inject]
    public Logic Logic { get; set; }

    [Inject]
    public GameLogic GameLogic { get; set; }

    [Inject]
    public Bullet View { get; set; }

    public override void OnRegister()
    {
        var damageMultiplier = View.WeaponOwnerType == WeaponOwnerType.Paladin ? GameLogic.BuffModule.DamageMultiplier : 1;
        View.Init(Logic.Defs.GetBullet(View.Type), damageMultiplier);
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }
}
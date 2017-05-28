using Paladin.View;
using UnityEngine;

public class TankPhantom : BattlePawn
{
    private bool isStealthMode;
    private float stealthTime = -1;
    private bool isAlreadyPoint = false;
    [SerializeField] private ScrollableTexture track;

    public override void HitHandler()
    {
        base.HitHandler();
        SetStealth(false);
    }

    // если готовность к выстрелу приходит раньше, нежели чувак дошел до точки атаки
    public void PrepareHandler()
    {
        if (isAlreadyPoint)
            ProcessShootingCoroutine(BattleStateMachine.STATE_PREPARE_SHOT4, BattleStateMachine.STATE_SHOT4);
        else
            weapons[0].Freeze();
    }

    public void ReloadHandler()
    {
        // скрываемся в стелсе, фризим оружие и идем дальше
        SetStealth(true);
        Mover.Resume();
        isAlreadyPoint = false;
        track.enabled = true;
    }

    public override void StartAttack()
    {
        // останавливаемся, вылазием из стелса, анфризим оружие.
        isAlreadyPoint = true;
        Mover.Pause();
        track.enabled = false;
        SetStealth(false);
        weapons[0].Unfreeze();
    }

    private void SetStealth(bool isStealth)
    {
        // если хотим встать в стелс, но у нас на это кулдаун, то не встаем, покуда кулдаун не пройдет
        // назначаем время входа в стелс на текущее время, если оно меньше, а если больше, то не переназначаем
        if (stealthTime < Time.time && isStealth)
            stealthTime = Time.time;
        else if (!isStealth)  // если же нужно выйти из стелса - то нет на это ограничений
            FadeInAnim();
    }

    private void FadeInAnim()
    {
        // делаем как в FadeOutSettings. подменяем материал и поперли
        FadeOutSettings.FadeIn();
    }

    private void FadeOutAnim()
    {
        // делаем как в FadeOutSettings. подменяем материал и поперли
        FadeOutSettings.FadeOut();
    }

    protected override void Update()
    {
        // встаем в стелс, если пришло ВремяВходаВСтелс
        base.Update();

        if (Time.time >= stealthTime && stealthTime > 0)
        {
            FadeOutAnim();
            stealthTime = -1;
        }
    }
}
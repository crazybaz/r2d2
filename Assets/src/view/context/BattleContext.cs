using Paladin.Controller;
using Paladin.Model;
using Paladin.Tools;
using Paladin.UI;
using Paladin.View;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleContext : MVCSContext
{
    private GameObject rootObject;
    public BattleContext(MonoBehaviour view) : base(view)
    {
        rootObject = view.gameObject;
    }

    public BattleContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) {}

    protected override void mapBindings()
    {
        MapViews();
        MapModels();
        MapCommands();
    }

    private void MapModels()
    {
        // singletones
        injectionBinder.Bind<LevelFactory>().ToSingleton();
        injectionBinder.Bind<AbilityFactory>().ToSingleton();

        // models
        injectionBinder.Bind<PawnModel>().To<PawnModel>();
        injectionBinder.Bind<PaladinModel>().To<PaladinModel>();
        injectionBinder.Bind<BossModel>().To<BossModel>();
        injectionBinder.Bind<BuffModule>().To<BuffModule>();
    }

    private void MapViews()
    {
        // UI Battle Scene
        mediationBinder.Bind<AbilityDialog>().ToMediator<AbilityDialogMediator>();
        mediationBinder.Bind<ResultDialog>().ToMediator<ResultDialogMediator>();
        mediationBinder.Bind<MainScreen>().ToMediator<MainScreenMediator>();

        // UI Common
        mediationBinder.Bind<OptionsDialog>().ToMediator<OptionsDialogMediator>();
        mediationBinder.Bind<LocaleText>().ToMediator<LocaleTextMediator>();

        // scenes
        mediationBinder.Bind<StartCameraScene>().ToMediator<StartCameraSceneMediator>();

        // pawns
        mediationBinder.Bind<EnemyMech>().ToAbstraction<IPawn>().To<PawnMediator>();
        mediationBinder.Bind<EnemyBunker>().ToAbstraction<IPawn>().To<PawnMediator>();
        mediationBinder.Bind<EnemyBunker2>().ToAbstraction<IPawn>().To<PawnMediator>();
        mediationBinder.Bind<EnemyMite>().ToAbstraction<IPawn>().To<PawnMediator>();
        mediationBinder.Bind<EnemyMiner>().ToAbstraction<IPawn>().To<PawnMediator>();
        mediationBinder.Bind<HumanoidEnemy>().ToAbstraction<IPawn>().To<PawnMediator>();
        mediationBinder.Bind<RocketInfantry>().ToAbstraction<IPawn>().To<PawnMediator>();
        mediationBinder.Bind<Transport>().ToAbstraction<IPawn>().To<PawnMediator>();
        mediationBinder.Bind<TransportRocketLauncher>().ToAbstraction<IPawn>().To<PawnMediator>();
        mediationBinder.Bind<SuicideEnemy>().ToAbstraction<IPawn>().To<PawnMediator>();
        mediationBinder.Bind<BattlePawn>().ToAbstraction<IPawn>().To<PawnMediator>();
        mediationBinder.Bind<Pawn>().ToAbstraction<IPawn>().To<PawnMediator>();
        mediationBinder.Bind<LightTank>().ToAbstraction<IPawn>().To<PawnMediator>();
        mediationBinder.Bind<TankPhantom>().ToAbstraction<IPawn>().To<PawnMediator>();
        mediationBinder.Bind<HeavyTank>().ToAbstraction<IPawn>().To<PawnMediator>();
        mediationBinder.Bind<MammothTank>().ToAbstraction<IPawn>().To<PawnMediator>();

        // weapons
        mediationBinder.Bind<Cannon>().ToAbstraction<IWeapon>().To<WeaponMediator>();
        mediationBinder.Bind<GrenadeLauncher>().ToAbstraction<IWeapon>().To<WeaponMediator>();
        mediationBinder.Bind<Laser>().ToAbstraction<IWeapon>().To<WeaponMediator>();
        mediationBinder.Bind<Machinegun>().ToAbstraction<IWeapon>().To<WeaponMediator>();
        mediationBinder.Bind<Miner>().ToAbstraction<IWeapon>().To<WeaponMediator>();
        mediationBinder.Bind<Missile>().ToAbstraction<IWeapon>().To<WeaponMediator>();
        mediationBinder.Bind<MissileSystem>().ToAbstraction<IWeapon>().To<WeaponMediator>();
        mediationBinder.Bind<PulseCannon>().ToAbstraction<IWeapon>().To<WeaponMediator>();
        mediationBinder.Bind<RocketLauncher>().ToAbstraction<IWeapon>().To<WeaponMediator>();
        mediationBinder.Bind<ShockCannon>().ToAbstraction<IWeapon>().To<WeaponMediator>();
        // boss weapons
        mediationBinder.Bind<BossCannon>().ToAbstraction<IWeapon>().To<WeaponMediator>();
        mediationBinder.Bind<BossLaser>().ToAbstraction<IWeapon>().To<WeaponMediator>();
        mediationBinder.Bind<BossMachinegun>().ToAbstraction<IWeapon>().To<WeaponMediator>();
        mediationBinder.Bind<BossPulseCannon>().ToAbstraction<IWeapon>().To<WeaponMediator>();
        mediationBinder.Bind<BossRocketLauncher>().ToAbstraction<IWeapon>().To<WeaponMediator>();

        // bullet
        mediationBinder.Bind<Bullet>().To<BulletMediator>();

        // bosses
        mediationBinder.Bind<Helicopter>().ToAbstraction<Boss>().To<BossMediator>();
        mediationBinder.Bind<TankBoss>().ToAbstraction<Boss>().To<BossMediator>();
        mediationBinder.Bind<TrainBoss>().ToAbstraction<Boss>().To<TrainBossMediator>();
        mediationBinder.Bind<BossPawn>().ToAbstraction<IPawn>().To<BossPawnMediator>();

        // other
        mediationBinder.Bind<ScrollableObject>().ToMediator<ScrollableObjectMediator>();
        mediationBinder.Bind<PaladinView>().ToMediator<PaladinMediator>();
        mediationBinder.Bind<PlayerTargeting>().ToMediator<PlayerTargetingMediator>();
        mediationBinder.Bind<LevelGenerator>().ToMediator<LevelGeneratorMediator>();
        mediationBinder.Bind<MasterPattern>().ToMediator<MasterPatternMediator>();
        mediationBinder.Bind<ScrollablePattern>().ToMediator<ScrollablePatternMediator>();
    }

    private void MapCommands()
    {
        commandBinder.Bind(CommandEvent.SHOW_UI).To<ShowUICommand>();
        commandBinder.Bind(CommandEvent.START_BATTLE).To<StartBattleCommand>();
        commandBinder.Bind(CommandEvent.FINISH_BATTLE).To<FinishBattleCommand>();
        commandBinder.Bind(CommandEvent.UPGRADE_PALADIN).To<UpgradePaladinCommand>();
        commandBinder.Bind(CommandEvent.TAKE_LEADERBOARD_REWARD).To<TakeLeaderboardRewardCommand>();

        /*commandBinder.Bind(BattleEvent.APPLY_BUFF)
            .To<ApplyBuffCommand>();

        commandBinder.Bind(BattleEvent.REMOVE_BUFF)
            .To<RemoveBuffCommand>();*/
    }

    public override void AddView(object view)
    {
        GameObject go = ((Component)view).gameObject;

        if (go.transform.parent == null)
            go.transform.SetParent(rootObject.transform);

        base.AddView(view);
    }
}
using Paladin.UI;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

namespace Paladin.View
{
    public class HangarContext : MVCSContext
    {
        private GameObject rootObject;
        public HangarContext(MonoBehaviour view) : base(view)
        {
            rootObject = view.gameObject;
        }

        public HangarContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) {}

        protected override void mapBindings()
        {
            MapViews();
            MapModels();
            MapCommands();
        }

        private void MapViews()
        {
            // UI Hangar Scene
            mediationBinder.Bind<HangarDialog>().ToMediator<HangarDialogMediator>();
            mediationBinder.Bind<StartBattle>().ToMediator<StartBattleMediator>();
            mediationBinder.Bind<ConfigurationDialog>().ToMediator<ConfigurationDialogMediator>();
            mediationBinder.Bind<PaladinConfigurationElement>().ToMediator<PaladinConfigurationElementMediator>();
            mediationBinder.Bind<WeaponConfigurationElement>().ToAbstraction<IConfigurationElement>().ToMediator<WeaponConfigurationElementMediator>();
            mediationBinder.Bind<AbilityConfigurationElement>().ToAbstraction<IConfigurationElement>().ToMediator<AbilityConfigurationElementMediator>();
            mediationBinder.Bind<BankDialog>().ToMediator<BankDialogMediator>();
            mediationBinder.Bind<EnergyDialog>().ToMediator<EnergyDialogMediator>();
            mediationBinder.Bind<LeaderboardDialog>().ToMediator<LeaderboardDialogMediator>();
            mediationBinder.Bind<LeaderboardElement>().ToMediator<LeaderboardElementMediator>();
            mediationBinder.Bind<ExpBarScreen>().ToMediator<ExpBarScreenMediator>();
            mediationBinder.Bind<QuestDialog>().ToMediator<QuestDialogMediator>();

            // UI Common
            mediationBinder.Bind<OptionsDialog>().ToMediator<OptionsDialogMediator>();
            mediationBinder.Bind<LocaleText>().ToMediator<LocaleTextMediator>();
        }

        private void MapModels()
        {
            injectionBinder.Bind<ReqsChecker>().ToSingleton();
            injectionBinder.Bind<AbilityModule>().ToSingleton();
        }

        private void MapCommands()
        {
            commandBinder.Bind(CommandEvent.SHOW_UI).To<ShowUICommand>();
            commandBinder.Bind(CommandEvent.SHOW_HANGAR).To<ShowHangarCommand>();
            commandBinder.Bind(CommandEvent.GET_LEADERBOARD).To<GetLeaderboardCommand>();

            commandBinder.Bind(CommandEvent.LOAD_BATTLE)
                .To<BattleProvisorCommand>()
                //.To<LoadAssetsCommand>() // TODO: сделать предзагрузку ассетов битвы
                .To<LoadBattleCommand>()
                .Once().InParallel();
        }

        public override void AddView(object view)
        {
            GameObject go = ((Component)view).gameObject;

            if (go.transform.parent == null)
                go.transform.SetParent(rootObject.transform);

            base.AddView(view);
        }
    }
}
using Paladin.Tools;
using Paladin.UI;
using strange.extensions.context.impl;
using strange.extensions.context.api;
using UnityEngine;

namespace Paladin.View
{
    public class PreloaderContext : MVCSContext
    {
        public PreloaderContext(MonoBehaviour view) : base(view) {}
        public PreloaderContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags) {}

        /*protected override void addCoreComponents()
        {
            base.addCoreComponents();
            injectionBinder.Unbind<ICommandBinder>();
            injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
        }*/

        protected override void mapBindings()
        {
            MapCrossMagic();

            // views
            mediationBinder.Bind<PreloaderDialog>().ToMediator<PreloaderDialogMediator>();

            // commands
            commandBinder.Bind(CommandEvent.SHOW_UI).To<ShowUICommand>();

            commandBinder.Bind(ContextEvent.START)
                .To<PrepareGameCommand>()
                .To<LoadLocaleCommand>()
                .To<ShowLogoCommand>()
                .Once().InSequence();

            commandBinder.Bind(CommandEvent.START_COMPLETE)
                .To<PreloaderProvisorCommand>()

                .To<ServerInitCommand>()
                //.To<ServerAuth>()
                .To<LoadStateCommand>()
                .To<LoadDefsCommand>()

                .To<LoadAssetsCommand>()
                .To<LoadHangarCommand>()
                .Once().InParallel();
        }

        private void MapCrossMagic()
        {
            // cross models
            injectionBinder.Bind<ILocale>().ToValue(new Locale()).ToSingleton().CrossContext();
            injectionBinder.Bind<DefinitionManager>().ToSingleton().CrossContext();
            injectionBinder.Bind<Logic>().ToSingleton().CrossContext();
            injectionBinder.Bind<GameLogic>().ToSingleton().CrossContext();

            injectionBinder.Bind<LocalConfig>().ToSingleton().CrossContext();
            injectionBinder.Bind<IconManager>().ToSingleton().CrossContext();

            // cross commands
            crossContextBridge.Bind(CommandEvent.SHOW_HANGAR);
            crossContextBridge.Bind(CommandEvent.START_BATTLE);
        }
    }
}
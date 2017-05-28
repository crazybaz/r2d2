using System.Collections;
using Paladin.Tools;
using Paladin.UI;
using Paladin.View;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

public class ShowLogoCommand : EventCommand
{
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }

    [Inject]
    public ILocale Locale { get; set; }

    private bool localeComplete;
    private bool cooldownComplete;

    public override void Execute()
    {
        // show logo
        dispatcher.Dispatch(CommandEvent.SHOW_UI, UIPath.StartLogoDialog);

        // locale handler
        localeComplete = Locale.IsLoaded;
        if (!localeComplete)
            dispatcher.AddListener(CommandEvent.LOCALE_COMPLETE, LocaleCompleteHandler);

        ContextView.GetComponent<PreloaderRoot>().StartCoroutine(WaitForLogo());
    }

    private IEnumerator WaitForLogo()
    {
        yield return new WaitForSecondsRealtime(0.3f /*GameLogic.Defs.Config.START_LOGO_TIMEOUT*/);

        cooldownComplete = true;
        if (localeComplete)
            Complete();
    }

    private void LocaleCompleteHandler()
    {
        localeComplete = true;
        if (cooldownComplete)
            Complete();
    }

    private void Complete()
    {
        dispatcher.RemoveListener(CommandEvent.LOCALE_COMPLETE, LocaleCompleteHandler);
        dispatcher.Dispatch(CommandEvent.SHOW_UI, UIPath.PreloaderDialog);
        dispatcher.Dispatch(CommandEvent.START_COMPLETE);
    }
}
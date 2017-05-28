using Paladin.Tools;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

public class LoadLocaleCommand : EventCommand
{
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject ContextView { get; set; }

    [Inject]
    public ILocale Locale { get; set; }

    public override void Execute()
    {
        Locale.Load(CompleteHandler);
    }

    private void CompleteHandler()
    {
        dispatcher.Dispatch(CommandEvent.LOCALE_COMPLETE);
    }
}
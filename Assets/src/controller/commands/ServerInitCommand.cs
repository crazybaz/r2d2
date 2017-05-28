using System;
using Paladin.Tools;
using strange.extensions.command.impl;
using UnityEngine;

public class Chainer
{
    public void Next()
    {
    };
}

public class ServerInitCommand : EventCommand
{
    [Inject]
    public ILocale Locale { get; set; }

    public override void Execute()
    {
        Link();
    }

    private void checkStatus<T>(Response<T> result)
    {
        //result.Status
    }

    private void /*Chainer*/ Link()
    {
        Api.Link(linkResponse =>
        {
            //if (checkStatus(linkResponse))
        });
    }

    private void Auth()
    {

    }
}
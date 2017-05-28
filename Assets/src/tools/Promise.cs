using System;
using System.Collections.Generic;

class PromisData<TParam, TValue>
{
    public TParam Param;
    public Promise<TParam, TValue>.PromiseCallback Callback;
}

enum PromisState
{
    Await,
    Ready
}

public class Promise<TValue>
{
    private PromisState state = PromisState.Await;

    public delegate void PromiseCallback(TValue param);
    private List<PromiseCallback> promises = new List<PromiseCallback>();

    protected TValue assignValue;
    public void Assign(TValue value)
    {
        assignValue = value;
        Process();
    }

    public void Register(PromiseCallback callback)
    {
        if (state == PromisState.Ready)
        {
            Process(callback);
        }
        else
        {
            promises.Add(callback);
        }
    }

    private void Process()
    {
        state = PromisState.Ready;

        foreach (var promise in promises)
            Process(promise);

        promises = null;
    }

    private void Process(PromiseCallback callback)
    {
        if (assignValue != null)
            callback(assignValue);

        else
            throw new Exception("Inconsistent state, assign data not found");
    }
}

public class Promise<TParam, TValue> : Promise<TValue>
{
    private PromisState state = PromisState.Await;

    public delegate TValue AssignCallback(TParam param);
    private List<PromisData<TParam, TValue>> promises = new List<PromisData<TParam, TValue>>();

    private AssignCallback assignCallback;
    public void Assign(AssignCallback assignCallback)
    {
        this.assignCallback = assignCallback;
        Process();
    }

    public void Register(TParam param, PromiseCallback callback)
    {
        if (state == PromisState.Ready)
        {
            Process(param, callback);
        }
        else
        {
            promises.Add(new PromisData<TParam, TValue>
            {
                Param = param,
                Callback = callback
            });
        }
    }

    private void Process()
    {
        state = PromisState.Ready;

        foreach (var promise in promises)
            Process(promise.Param, promise.Callback);

        promises = null;
    }

    private void Process(TParam param, PromiseCallback callback)
    {
        if (assignValue != null)
            callback(assignValue);

        else if (assignCallback != null)
            callback(assignCallback(param));

        else
            throw new Exception("Inconsistent state, assign data not found");
    }
}
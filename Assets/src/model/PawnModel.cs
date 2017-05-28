using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class PawnModel
{
    [Inject]
    public IEventDispatcher Dispatcher { get; set; }

    public PawnConfig Config;

    public void Init(PawnConfig config)
    {
        Config = config;

        health = Config.MaxHealth;
        ResetSpeed();
    }

    // >>>>>>>>>>>>>>>>>>>>>>>> SPEED >>>>>>>>>>>>>>>>>>>>>>>>

    private float speedBoost;
    public float Speed { get; private set; }

    public void StunSpeed(bool isStun)
    {
        if (isStun)
            ChangeSpeed(0);
        else
            ResetSpeed();
    }

    public void BoostSpeed(float value)
    {
        speedBoost += value;
        if (Speed > 0) // if not stun
            ChangeSpeed(Speed + speedBoost);
    }

    public void ResetSpeed()
    {
        ChangeSpeed(Config.MaxSpeed + speedBoost);
    }

    // strictly private method
    private void ChangeSpeed(float value)
    {
        Speed = value;
        Dispatcher.Dispatch(ModelEvent.SPEED_CHANGED, Speed);
    }

    // >>>>>>>>>>>>>>>>>>>>>>>> HEALTH >>>>>>>>>>>>>>>>>>>>>>>>

    private float health;

    public float Health
    {
        get { return health; }
        private set
        {
            // only decrease
            health = Mathf.Clamp(value, 0, health);

            if (health <= 0)
                Dispatcher.Dispatch(ModelEvent.DESTROY, Config.Type);
        }
    }

    public void Hit(float value)
    {
        Health -= value;
    }
}
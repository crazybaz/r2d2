using System;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class PaladinModel
{
    [Inject]
    public IEventDispatcher Dispatcher { get; set; }

    [Inject]
    public BuffModule BuffModule { get; set; }

    [Inject]
    public Logic Logic { get; set; }

    public IEventDispatcher View;
    public PaladinConfig Config;

    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> PARAMS >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    public float MaxHealth { get; private set; }

    private float health;
    public float Health
    {
        get { return health; }

        private set
        {
            var oldValue = health;

            if (value > health) // healing
            {
                health = Math.Min(MaxHealth, value);
            }
            else // damaging
            {
                if (!BuffModule.IsUnderShield)
                {
                    //Dispatcher.Dispatch(GameEvent.HIT, health - value);

                    health = value;

                    if (health <= 0 /*&& !Disabled*/)
                        Dispatcher.Dispatch(ModelEvent.DESTROY);
                }
            }

            if (oldValue != health)
                Dispatcher.Dispatch(ModelEvent.HEALTH_CHANGED, health);
        }
    }

    public void Hit(float value)
    {
        // процент, на который уменьшается входящий урон
        var armorRatio = Mathf.Min(Armor * 0.001f, 100f) / 100f;

        Health -= (value - value * armorRatio);
    }

    public void Heal(float value)
    {
        Health += value;
    }

    public float HealthProgress { get { return Health / MaxHealth; }} // sugar

    public float Speed;
    public float Armor;

    public float Resist; // { get { return Config.Resist; }}
    public float CollisionDamage; // { get { return Config.CollisionDamage; }}

    public PaladinModel Init(PaladinConfig config, IEventDispatcher view)
    {
        View = view;
        Config = config;

        UpdateSpecs();
        Health = MaxHealth;

        return this;
    }

    public void UpdateSpecs()
    {
        var paladinState = Logic.GetPaladin(Config.Type);

        var headLevel = paladinState.Upgrades[NodeType.Head];
        var frameLevel = paladinState.Upgrades[NodeType.Frame];
        var chassisLevel = paladinState.Upgrades[NodeType.Chassis];

        var headMultipliers = Config.HeadUpgrades[headLevel].ParamMultipliers;
        var frameMultipliers = Config.FrameUpgrades[frameLevel].ParamMultipliers;
        var chassisMultipliers = Config.ChasisUpgrades[chassisLevel].ParamMultipliers;

        MaxHealth = Config.Health * BuffModule.HealthMultiplier * headMultipliers.Health * frameMultipliers.Health * chassisMultipliers.Health;
        Speed = Config.Speed * BuffModule.SpeedMultiplier * headMultipliers.Speed * frameMultipliers.Speed * chassisMultipliers.Speed;
        Armor = Config.Armor * BuffModule.ArmorMultiplier * headMultipliers.Armor * frameMultipliers.Armor * chassisMultipliers.Armor;
        Resist = Config.Resist * headMultipliers.Resist * frameMultipliers.Resist * chassisMultipliers.Resist;
        CollisionDamage = Config.CollisionDamage * headMultipliers.CollisionDamage * frameMultipliers.CollisionDamage * chassisMultipliers.CollisionDamage;
    }
}

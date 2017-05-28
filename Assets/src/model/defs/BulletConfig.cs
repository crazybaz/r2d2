using System;

[Serializable]
public enum ObstacleCollisionType
{
    Ignore,
    Collision,
    ReduceDamage
}

public enum BulletType
{
    Bullet1, Bullet2, Bullet3,
    CruiseMissile1,
    Grenade1, Grenade2,
    Laser1, Laser2, Laser3,
    Mine1, Mine2, Mine3,
    Missile1,
    PulseShell1, PulseShell2,
    Shell1, Shell2, Shell3,
    VacuumBomb1, VacuumBomb2, VacuumBomb3, VacuumBomb4, VacuumBomb5,
    ArtilleryBomb1, ArtilleryBomb2, ArtilleryBomb3, ArtilleryBomb4, ArtilleryBomb5,
    RocketMissile1, RocketMissile2,

    // INFO: post-release
    PulseBeam1, PulseBeam2,
    PulseBall1,
    PulseBolo1,
    ShockWaves1,
    Electra1
}

public class BulletConfig
{
    public uint BaseSpeed;
    public uint BaseDamage;
    public uint DamageRadius;
    public uint ActivationRadius;
    public uint LifeTime; // <= 0 живет бесконечно, больше нуля - самоуничтожается через LifeTime секунд
    public uint Distance;
    public ObstacleCollisionType ObstacleCollisionType = ObstacleCollisionType.Ignore;
    public bool Destroyable;
    public bool ExplodeOnAnyCollision;

    public BulletConfig(JSONObject def)
    {
        if (def.HasField("speed"))
            BaseSpeed = (uint)def["speed"].n;
        if (def.HasField("damage"))
            BaseDamage = (uint)def["damage"].n;
        if (def.HasField("damageRadius"))
            DamageRadius = (uint)def["damageRadius"].n;
        if (def.HasField("activationRadius"))
            ActivationRadius = (uint)def["activationRadius"].n;
        if (def.HasField("lifeTime"))
            LifeTime = (uint)def["lifeTime"].n;
        if (def.HasField("distance"))
            Distance = (uint)def["distance"].n;
        if (def.HasField("obstacleCollisionType"))
            ObstacleCollisionType = DefinitionManager.EnumValue<ObstacleCollisionType>(def["obstacleCollisionType"].str);
        if (def.HasField("destroyable"))
            Destroyable = def["destroyable"].b;
        if (def.HasField("explodeOnAnyCollision"))
            ExplodeOnAnyCollision = def["explodeOnAnyCollision"].b;

    }
}
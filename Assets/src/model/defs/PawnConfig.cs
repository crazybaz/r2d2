using System;
using System.Collections.Generic;

public enum PawnKind
{
    Enemy,
    Paladin,
    Obstacle,
    FlyingEnemy // пометить высоко летящие pawn, специально, чтобы обстреливать их не коллайдером пули, а рейкастом из камеры
}

[Flags]
public enum PawnGroupType
{
    Trooper,
    Mite,
    LightMech,
    Exoskeleton,
    Buggy,
    Bmp,
    Tank,
    Mech,
    HeavyTank,
    Drone,
    Helicopter,
    Bunker,

    BossTank,
    BossTrain,
    BossHelicopter,

    Ambience
}

[Flags]
public enum PawnType
{
    Trooper1, Trooper2, Trooper3, Trooper4,
    Mite1, Mite2, Mite3, Mite4,
    LightMech1, LightMech2, LightMech3, LightMech4,
    Exoskeleton1, Exoskeleton2, Exoskeleton3, Exoskeleton4,
    Buggy1, Buggy2, Buggy3, Buggy4,
    Bmp1, Bmp2, Bmp3, Bmp4,
    Tank1, Tank2, Tank3, Tank4,
    Mech1, Mech2, Mech3, Mech4,
    HeavyTank1, HeavyTank2, HeavyTank3, HeavyTank4,
    Drone1, Drone2, Drone3, Drone4,
    Helicopter1, Helicopter2, Helicopter3, Helicopter4,
    Bunker1, Bunker2, Bunker3, Bunker4,

    // Ambience
    // Desert
    Bags, BarrelDown, Barrels, Box, Bunker, Bus1, Bus2, Bus3, Bus4, Cart,
    ConcreteWall, Conteiner, Fence, Grid, Hatch1,
    Hatch2, Home1, Home2, IronWall, LitWall, MetallWall,
    Most, MountSmall, Road1, Road2, Rock3, Rock5, Rock6,
    Rock7, Rock8, Rock9, Rock10, RockArch, Stoun, StounWall1, StounWall2,
    StounWall3, OilTank, ThreeBarrelDown, Tower, Trash, Tray, Tube,
    Wheel, WoodWall1, WoodBox, Radar, Antena,
    Boxtex, Budka, Dom, Pipe,

    // Bosses
    BossTankStage1Machinegun, BossTankStage1Cannon, BossTankStage2Rocket, BossTankStage2Cannon, BossTankStage3,
    BossHelicopterStage1Machinegun, BossHelicopterStage1Gun, BossHelicopterStage2Machinegun, BossHelicopterStage2RocketLauncher,
    BossHelicopterStage3Machinegun, BossHelicopterStage3Gun, BossHelicopterStage4,
    BossTrainStage1Machinegun, BossTrainStage1Cannon, BossTrainStage2Cannon, BossTrainStage2Missile, BossTrainStage2BoloCannon,
    BossTrainStage3Machinegun, BossTrainStage3BoloCannon, BossTrainStage3Missile, BossTrainStage4
}

public class PawnConfig : IPawnConfig
{
    public PawnType Type;
    public PawnGroupType GroupType;
    public uint MaxSpeed;
    public uint MaxHealth;
    public uint CollisionDamage;
    public uint RewardPoints;
    public uint AttackRadius;
    public List<WeaponInfo> WeaponInfo; // [{type, level}]

    public PawnConfig(JSONObject item)
    {
        // обязательные поля
        Type = DefinitionManager.EnumValue<PawnType>(item["type"].str);
        GroupType = DefinitionManager.EnumValue<PawnGroupType>(item["group"].str);

        if (item.HasField("maxSpeed"))
            MaxSpeed = (uint)item["maxSpeed"].i;

        if (item.HasField("maxHealth"))
            MaxHealth = (uint)item["maxHealth"].i;

        if (item.HasField("collisionDamage"))
            CollisionDamage = (uint)item["collisionDamage"].i;

        if (item.HasField("rewardPoints"))
            RewardPoints = (uint)item["rewardPoints"].i;

        if (item.HasField("weaponInfo"))
        {
            var weaponInfo = new List<WeaponInfo>();
            foreach (var info in item["weaponInfo"].list)
            {
                weaponInfo.Add(new WeaponInfo
                {
                    Type = DefinitionManager.EnumValue<WeaponType>(info["type"].str),
                    Level = (uint)info["level"].n,
                    AttackRadius = info.HasField("attackRadius") ? (uint)info["attackRadius"].n : 0
                });
            }

            WeaponInfo = weaponInfo;
        }
    }
}
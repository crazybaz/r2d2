using System.Collections.Generic;
using UnityEngine;

public class ShotData
{
    public GameObject Bullet;
    [Range(-90, 90)] public int Orientation;
    public MovingConfig MovingConfig;
}

public enum WeaponOwnerType
{
    Enemy,
    Paladin
}

public enum WeaponType
{
    Machinegun,
    Laser,
    Cannon,
    PulseCannon,
    ShockCannon,
    WaveCannon,
    PlasmaCannon,
    BoloCannon,
    HeavyPlasma,
    Electra,
    Missile,
    GrenadeLauncher,
    MissileSystem,
    RocketLauncher,
    Miner,

    // Boss weapons
    BossMachinegun, BossLaser, BossCannon, BossPulseCannon, BossRocketLauncher
}

public class WeaponConfig
{
    public List<ShotData> ShotConfig; // параметры выстрела
    public float FireCooldown = 1.0f; // задержка между выстрелами
    public int ShotCount = 1; // число пуль в обойме
    public float ReloadTime = 0; // время перезарядки обоймы в секундах
    public float Multiplier = 1.0f; // множитель для параметров пули
    public Loot UpgradeCost; // стоимость грейда на следующий уровень
    public Reqs Reqs; // требования грейда на следующий уровень

    public WeaponConfig(JSONObject def)
    {
        if (def.HasField("shotConfig"))
        {
            ShotConfig = new List<ShotData>();
            foreach (var rawData in def["shotConfig"].list)
            {
                var shotData = new ShotData
                {
                    Bullet = Resources.Load("prefabs/bullets/" + rawData["bullet"].str) as GameObject,
                    Orientation = (int)rawData["orientation"].n
                };

                shotData.MovingConfig = new MovingConfig
                {
                    MovingType = DefinitionManager.EnumValue<MovingType>(rawData["movingType"].str)
                };

                if (rawData.HasField("angleSpeed"))
                    shotData.MovingConfig.AngleSpeed = (int)rawData["angleSpeed"].n;
                if (rawData.HasField("detectingRadius"))
                    shotData.MovingConfig.DetectingRadius = (int)rawData["detectingRadius"].n;
                if (rawData.HasField("aimingAngle"))
                    shotData.MovingConfig.AimingAngle = rawData["aimingAngle"].f;
                if (rawData.HasField("aimingCooldown"))
                    shotData.MovingConfig.AimingCooldown = rawData["aimingCooldown"].f;

                ShotConfig.Add(shotData);
            }
        }

        if (def.HasField("fireCooldown"))
            FireCooldown = def["fireCooldown"].f;

        if (def.HasField("shotCount"))
            ShotCount = (int)def["shotCount"].n;

        if (def.HasField("reloadTime"))
            ReloadTime = def["reloadTime"].f;

        if (def.HasField("multiplier"))
            Multiplier = def["multiplier"].f;

        if (def.HasField("upgradeCost"))
            UpgradeCost = DefinitionManager.GetLoot(def["upgradeCost"]);

        if (def.HasField("upgradeReqs"))
            Reqs = DefinitionManager.GetReqs(def["upgradeReqs"]);
    }
}
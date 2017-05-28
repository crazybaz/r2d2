using System;
using System.Collections.Generic;
using Paladin.Model;
using UnityEngine;

public class DefinitionManager
{
    public Dictionary<string, PawnConfig> PawnConfig;
    public Dictionary<PaladinType, PaladinConfig> PaladinConfig;

    public Dictionary<string, BossConfig> BossConfig;
    public Dictionary<BulletType, BulletConfig> BulletConfig;

    public Dictionary<string, WeaponConfig> WeaponEnemyConfig;
    public Dictionary<string, WeaponConfig> WeaponPaladinConfig;
    public Dictionary<WeaponType, SuspensionType> WeaponSuspensionConfig;

    public Dictionary<string, AbilityConfig> AbilityConfig;
    public Dictionary<AbilityType, Reqs> AbilityReqsConfig;

    public Dictionary<uint, LevelConfig> LevelConfig;

    // ГД уровень соответсвует позиции для удобства
    // Пример: сколько нужно exp для достижения 17го уровня? ExpLevelRequirement[17]
    public uint[] ExpLevelRequirement;

    public Dictionary<ProductType, ProductConfig> ProductConfig;
    public Dictionary<ProductGroup, List<ProductType>> ProductGroupConfig;

    public List<QuestConfig> QuestConfig;
    public Dictionary<uint, ChestConfig> ChestConfig;

    public DropConfig DropConfig;
    public GameConfig Config;

    public DefinitionManager Init()
    {
        InitPawnConfig();
        InitAmbience();
        InitPaladinConfig();
        InitBulletConfig();
        InitWeaponConfig();
        InitSuspensionsConfig();
        InitBossConfig();
        InitAbilityConfig();
        InitAbilityReqs();
        InitDropConfig();
        InitGameConfig();
        InitLevelConfig();
        InitExpConfig();
        InitProductConfig();
        InitQuestConfig();
        InitChestConfig();

        return this;
    }

    // >>>>>>>>>>>>>>>>>>>>>>>> HELPERS >>>>>>>>>>>>>>>>>>>>>>>>

    public static T EnumValue<T>(string item) //where T : Enum
    {
        var enumValues = Enum.GetNames(typeof(T));
        for (int i = 0; i < enumValues.Length; i++)
        {
            var value = enumValues[i];
            if (value == item)
                return (T)Enum.Parse(typeof(T), value);
        }
        throw new ArgumentException("Value \"" + item + "\" not found in " + typeof(T));
    }

    public static Loot GetLoot(JSONObject def)
    {
        if (!def.IsObject)
            throw new ArgumentException("Loot def is not an object");

        var loot = new Loot();
        foreach (var key in def.keys)
            loot.Add(EnumValue<LootType>(key), (uint)def[key].i);

        return loot;
    }

    public static Reqs GetReqs(JSONObject def)
    {
        var reqs = new Reqs();

        if (def.HasField("playerLevel"))
            reqs.Level = (ushort)def["playerLevel"].n;

        if (def.HasField("paladinType"))
            reqs.PaladinType = EnumValue<PaladinType>(def["paladinType"].str);

        return reqs;
    }

    public static uint GetUint(JSONObject def, string fieldName)
    {
        return (uint)def.GetField(fieldName).i;
    }

    public static string GetString(JSONObject def, string fieldName)
    {
        return def.GetField(fieldName).str;
    }

    public static bool TryGet(JSONObject def, string fieldName, ref uint result)
    {
        return def.GetField(ref result, fieldName);
    }

    public static bool TryGet(JSONObject def, string fieldName, ref string result)
    {
        return def.GetField(ref result, fieldName);
    }

    public static bool TryGet(JSONObject def, string fieldName, ref Loot result)
    {
        var hasResult = def.HasField(fieldName);
        if (hasResult)
            result = GetLoot(def.GetField(fieldName));
        return hasResult;
    }

    // >>>>>>>>>>>>>>>>>>>>>>>> INIT >>>>>>>>>>>>>>>>>>>>>>>>

    private void InitPawnConfig()
    {
        PawnConfig = new Dictionary<string, PawnConfig>();

        var jsonString = Resources.Load<TextAsset>("definitions/enemy").text;
        var jsonObject = new JSONObject(jsonString);

        foreach (var def in jsonObject.list)
            PawnConfig[def["type"].str + def["level"].n] = new PawnConfig(def);
    }

    private void InitAmbience()
    {
        var jsonString = Resources.Load<TextAsset>("definitions/ambience").text;
        var jsonObject = new JSONObject(jsonString);

        foreach (var def in jsonObject.list)
            PawnConfig[def["type"].str + def["level"].n] = new PawnConfig(def);
    }

    private void InitPaladinConfig()
    {
        PaladinConfig = new Dictionary<PaladinType, PaladinConfig>();

        var jsonString = Resources.Load<TextAsset>("definitions/paladin").text;
        var jsonObject = new JSONObject(jsonString);

        foreach (var def in jsonObject.list)
        {
            var paladinType = EnumValue<PaladinType>(def.GetField("id").str);

            if (PaladinConfig.ContainsKey(paladinType))
                PaladinConfig[paladinType].Init(def);
            else
                PaladinConfig[paladinType] = new PaladinConfig().Init(def);
        }
    }

    private void InitBulletConfig()
    {
        BulletConfig = new Dictionary<BulletType, BulletConfig>();

        var jsonString = Resources.Load<TextAsset>("definitions/bullets").text;
        var jsonObject = new JSONObject(jsonString);

        foreach (var def in jsonObject.list)
            BulletConfig[EnumValue<BulletType>(def["id"].str)] = new BulletConfig(def);
    }

    private void InitWeaponConfig()
    {
        WeaponEnemyConfig = new Dictionary<string, WeaponConfig>();
        WeaponPaladinConfig = new Dictionary<string, WeaponConfig>();
        InitWeaponConfig(WeaponEnemyConfig, "definitions/weapon_enemy");
        InitWeaponConfig(WeaponPaladinConfig, "definitions/weapon_paladin");
    }

    private void InitWeaponConfig(Dictionary<string, WeaponConfig> config, string configPath)
    {
        var jsonString = Resources.Load<TextAsset>(configPath).text;
        var jsonObject = new JSONObject(jsonString);

        foreach (var def in jsonObject.list)
            config[def["id"].str + def["level"].n] = new WeaponConfig(def);
    }

    private void InitSuspensionsConfig()
    {
        WeaponSuspensionConfig = new Dictionary<WeaponType, SuspensionType>();

        var jsonString = Resources.Load<TextAsset>("definitions/suspensions").text;
        var jsonObject = new JSONObject(jsonString);

        foreach (var weapon in jsonObject.keys)
            WeaponSuspensionConfig[EnumValue<WeaponType>(weapon)] = EnumValue<SuspensionType>(jsonObject.GetField(weapon).str);
    }

    private void InitAbilityConfig()
    {
        AbilityConfig = new Dictionary<string, AbilityConfig>();

        var jsonString = Resources.Load<TextAsset>("definitions/ability").text;
        var jsonObject = new JSONObject(jsonString);

        foreach (var def in jsonObject.list)
            AbilityConfig[def["id"].str + def["level"].n] = new AbilityConfig(def);
    }

    private void InitAbilityReqs()
    {
        AbilityReqsConfig = new Dictionary<AbilityType, Reqs>();

        var jsonString = Resources.Load<TextAsset>("definitions/ability_reqs").text;
        var jsonObject = new JSONObject(jsonString);

        foreach (var item in jsonObject.list)
            AbilityReqsConfig[EnumValue<AbilityType>(item["id"].str)] = GetReqs(item["reqs"]);
    }

    private void InitDropConfig()
    {
        DropConfig = new DropConfig();

        var jsonString = Resources.Load<TextAsset>("definitions/drop").text;
        var jsonObject = new JSONObject(jsonString);

        foreach (var item in jsonObject.list)
            DropConfig.AddItem(item);
    }

    private void InitGameConfig()
    {
        var jsonString = Resources.Load<TextAsset>("definitions/config").text;
        Config = new GameConfig(new JSONObject(jsonString));
    }

    private void InitBossConfig()
    {
        BossConfig = new Dictionary<string, BossConfig>();

        var jsonString = Resources.Load<TextAsset>("definitions/bosses").text;
        var jsonObject = new JSONObject(jsonString);

        foreach (var item in jsonObject.list)
        {
            var key = item["id"].str + item["level"].n;

            if (!BossConfig.ContainsKey(key))
                BossConfig[key] = new BossConfig(item);

            if (item.HasField("pawns"))
                BossConfig[key].UpdatePawns(item["pawns"].list);
        }
    }

    private void InitLevelConfig()
    {
        LevelConfig = new Dictionary<uint, LevelConfig>();

        var jsonString = Resources.Load<TextAsset>("definitions/level").text;
        var jsonObject = new JSONObject(jsonString);

        foreach (var item in jsonObject.list)
            LevelConfig[(uint)item.GetField("playerLevel").i] = new LevelConfig(item);
    }

    private void InitExpConfig()
    {
        var jsonString = Resources.Load<TextAsset>("definitions/experience").text;
        var jsonObject = new JSONObject(jsonString);
        ExpLevelRequirement = new uint[jsonObject.list.Count + 1];
        foreach (var item in jsonObject.list)
            ExpLevelRequirement[item.GetField("level").i] = (uint) item.GetField("ammountExp").i;
    }

    private void InitProductConfig()
    {
        ProductConfig = new Dictionary<ProductType, ProductConfig>();
        ProductGroupConfig = new Dictionary<ProductGroup, List<ProductType>>();

        var jsonString = Resources.Load<TextAsset>("definitions/products").text;
        var jsonObject = new JSONObject(jsonString);

        foreach (var item in jsonObject.list)
        {
            var product = ProductConfig[EnumValue<ProductType>(item.GetField("id").str)] = new ProductConfig(item);

            if (!ProductGroupConfig.ContainsKey(product.Group))
                ProductGroupConfig[product.Group] = new List<ProductType>();

            ProductGroupConfig[product.Group].Add(product.Type);
        }
    }

    private void InitQuestConfig()
    {
        QuestConfig = new List<QuestConfig>();

        var jsonString = Resources.Load<TextAsset>("definitions/quests").text;
        var jsonObject = new JSONObject(jsonString);

        foreach (var item in jsonObject.list)
        {
            QuestConfig.Add(new QuestConfig(item));
        }
    }

    private void InitChestConfig()
    {
        ChestConfig = new Dictionary<uint, ChestConfig>();

        var jsonString = Resources.Load<TextAsset>("definitions/chests").text;
        var jsonObject = new JSONObject(jsonString);

        foreach (var item in jsonObject.list)
            ChestConfig.Add(GetUint(item, "id"), new ChestConfig(item));
    }

    // >>>>>>>>>>>>>>>>>>>>>>>> DATA >>>>>>>>>>>>>>>>>>>>>>>>

    public PawnConfig GetPawn(PawnType type, uint level)
    {
        PawnConfig config;
        var id = type.ToString() + level;

        if (!PawnConfig.TryGetValue(id, out config))
            throw new ArgumentException("Configuration for " + id + " not found");

        return config;
    }

    public PaladinConfig GetPaladin(PaladinType type)
    {
        PaladinConfig config;

        if (!PaladinConfig.TryGetValue(type, out config))
            throw new ArgumentException("Configuration for " + type + " not found");

        return config;
    }

    public BulletConfig GetBullet(BulletType type)
    {
        BulletConfig config;

        if (!BulletConfig.TryGetValue(type, out config))
            throw new ArgumentException("Configuration for " + type + " not found");

        return config;
    }

    public bool HasWeapon(WeaponType type, uint level, WeaponOwnerType ownerType)
    {
        var id = type.ToString() + level;
        return (ownerType == WeaponOwnerType.Paladin ? WeaponPaladinConfig : WeaponEnemyConfig).ContainsKey(id);
    }

    public WeaponConfig GetWeapon(WeaponType type, uint level, WeaponOwnerType ownerType)
    {
        WeaponConfig config;
        var id = type.ToString() + level;

        if (!(ownerType == WeaponOwnerType.Paladin ? WeaponPaladinConfig : WeaponEnemyConfig).TryGetValue(id, out config))
            throw new ArgumentException("Configuration for " + id + " not found");

        return config;
    }

    public bool HasAbility(AbilityType type, uint level)
    {
        var id = type.ToString() + level;
        return AbilityConfig.ContainsKey(id);
    }

    public AbilityConfig GetAbility(AbilityType type, uint level)
    {
        AbilityConfig config;
        var id = type.ToString() + level;

        if (!AbilityConfig.TryGetValue(id, out config))
            throw new ArgumentException("Configuration for " + id + " not found");

        return config;
    }

    public Reqs GetAbilityReqs(AbilityType abilityType)
    {
        Reqs reqs;
        if (!AbilityReqsConfig.TryGetValue(abilityType, out reqs))
            throw new ArgumentException("Configuration for " + abilityType + " not found");
        return reqs;
    }

    public Dictionary<DropType, List<DropItem>> GetDrop(PawnGroupType groupType)
    {
        Dictionary<DropType, List<DropItem>> dropItems;
        if (!DropConfig.TryGetValue(groupType, out dropItems))
            throw new ArgumentException("Configuration for " + groupType + " not found");

        return dropItems;
    }

    public bool TryGetDrop(PawnGroupType groupType, out Dictionary<DropType, List<DropItem>> dropItems)
    {
        return DropConfig.TryGetValue(groupType, out dropItems);
    }

    public BossConfig GetBoss(PawnGroupType type, uint level)
    {
        BossConfig config;
        var id = type.ToString() + level;

        if (!BossConfig.TryGetValue(id, out config))
            throw new ArgumentException("Configuration for " + id + " not found");

        return config;
    }

    public LevelConfig GetLevel(uint level)
    {
        LevelConfig config;
        if (!LevelConfig.TryGetValue(level, out config))
            throw new ArgumentException("Configuration LevelConfig for player level " + level + " not found");
        return config;
    }

    public ProductConfig GetProduct(ProductType type)
    {
        ProductConfig config;
        if (!ProductConfig.TryGetValue(type, out config))
            throw new ArgumentException("ProductConfig definition not found: " + type);
        return config;
    }

    public List<ProductType>GetProductGroup(ProductGroup group)
    {
        List<ProductType> items;
        if (!ProductGroupConfig.TryGetValue(group, out items))
            throw new ArgumentException("ProductGroup definition items not found: " + group);
        return items;
    }

    public bool TryGetProductGroup(ProductGroup group, out List<ProductType> products)
    {
        return ProductGroupConfig.TryGetValue(group, out products);
    }

    public QuestConfig GetQuestConfig(int index)
    {
        return QuestConfig[index];
    }

    public ChestConfig GetChest(uint chestId)
    {
        ChestConfig config;
        if (!ChestConfig.TryGetValue(chestId, out config))
            throw new ArgumentException("ChestConfig definition not found: ID=" + chestId);
        return config;
    }

    public bool TryGetChest(uint chestId, out ChestConfig config)
    {
        return ChestConfig.TryGetValue(chestId, out config);
    }
}
public enum ProductGroup
{
    Chest,
    Soft,
    Hard
}

public enum ProductType
{
    SoftTier1,
    SoftTier2,
    SoftTier3,
    SoftTier4,
    SoftTier5,
    SoftTier6,
    SoftTier7,
    SoftTier8,

    HardTier1,
    HardTier2,
    HardTier3,
    HardTier4,
    HardTier5,
    HardTier6,
    HardTier7,
    HardTier8,

    ChestTier1,
    ChestTier2,
    ChestTier3,
    ChestTier4,
    ChestTier5,
    ChestTier6,
    ChestTier7,
    ChestTier8
}

public enum ProductKind
{
    Regular,
    Sales,
    Popular,
    Offer
}

public class ProductConfig
{
    public ProductType Type;
    public ProductGroup Group;
    public ProductKind Kind;
    public string Percent;
    public string Tier;
    public bool Active;
    public Loot Reward;
    public Loot HardPrice;
    public string DummyPrice;
    public string OriginalPrice;

    public ProductConfig(JSONObject def)
    {
        Type = DefinitionManager.EnumValue<ProductType>(def.GetField("id").str);
        Group = DefinitionManager.EnumValue<ProductGroup>(def.GetField("group").str);
        Kind = DefinitionManager.EnumValue<ProductKind>(def.GetField("kind").str);
        Tier = def.GetField("tier").str;
        Active = def.GetField("active").b;
        Reward = DefinitionManager.GetLoot(def.GetField("reward"));

        // optional
        DefinitionManager.TryGet(def, "percent", ref Percent);
        DefinitionManager.TryGet(def, "hardPrice", ref HardPrice);
        DefinitionManager.TryGet(def, "dummyPrice", ref DummyPrice);
        DefinitionManager.TryGet(def, "originalPrice", ref OriginalPrice);
    }
}
using System.Collections.Generic;

public class Session
{
    public PaladinType Type;

    public int Exp { get; private set; }
    public int Score { get; private set; }
    public Dictionary<LootType, uint> LootItems = new Dictionary<LootType, uint>();
    public uint[] AbilityUseCount;
    public float FieldMoveSpeed { get; private set; } // чтобы изменить, испольуй метод GameLogic ChangeFieldMoveSpeed

    public Session()
    {
        AbilityUseCount = new uint[]{ 0, 0, 0};
    }

    public void ChangeFieldMoveSpeed(float value)
    {
        FieldMoveSpeed = value;
    }

    public void AddExp(int value)
    {
        Exp += value;
    }

    public void AddScore(int value)
    {
        Score += value;
    }

    public uint GetLoot(LootType type)
    {
        uint value;
        LootItems.TryGetValue(type, out value);
        return value;
    }
}
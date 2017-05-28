public interface IBattlePawn : IPawn
{
    void StartAttack();
    void StopAttack();
    float PrepareAnimLength { get; }
    bool MoverRequired { get; }
    bool AnimatorRequired { get; }
}
using System.Collections.Generic;

public interface IPattern : IDestroyable
{
    void AddPawn(Pawn pawn);
    List<Pawn> PawnList { get; }
}
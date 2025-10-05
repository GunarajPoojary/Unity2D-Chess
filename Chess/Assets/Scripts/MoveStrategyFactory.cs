using System;

public interface IMoveStrategyFactory
{
    IMoveStrategy GetStrategy(BoardSide boardSide, PieceType type);
}

public class MoveStrategyFactory : IMoveStrategyFactory
{
    public IMoveStrategy GetStrategy(BoardSide boardSide, PieceType type) =>
        type switch
        {
            PieceType.Rook => new RookMove(ServiceLocator.Get<IBoardQuery>()),
            PieceType.Bishop => new BishopMove(ServiceLocator.Get<IBoardQuery>()),
            PieceType.Knight => new KnightMove(ServiceLocator.Get<IBoardQuery>()),
            PieceType.King => new KingMove(ServiceLocator.Get<IBoardQuery>()),
            PieceType.Queen => new QueenMove(ServiceLocator.Get<IBoardQuery>()),
            PieceType.Pawn => new PawnMove(ServiceLocator.Get<IBoardQuery>(), boardSide),
            _ => throw new ArgumentException($"Unsupported piece type: {type}")
        };
}
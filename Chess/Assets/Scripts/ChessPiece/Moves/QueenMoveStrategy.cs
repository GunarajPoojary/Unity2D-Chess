using System;

public class QueenMoveStrategy : IMoveStrategy
{
    private readonly RookMoveStrategy _rook = new RookMoveStrategy();
    private readonly BishopMoveStrategy _bishop = new BishopMoveStrategy();

    public bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onGetLegalMoveAction)
    {
        bool rookMoves = _rook.TryGetMoves(board, color, currentTile, onGetLegalMoveAction);
        bool bishopMoves = _bishop.TryGetMoves(board, color, currentTile, onGetLegalMoveAction);

        return rookMoves || bishopMoves;
    }
}
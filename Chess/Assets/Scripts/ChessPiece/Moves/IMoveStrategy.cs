using System;

public interface IMoveStrategy
{
    bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onGetLegalMoveAction);
}
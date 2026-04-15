using System;

public class King : ChessPiece
{
    private readonly IMoveStrategy _moveStrategy = new KingMoveStrategy();

    public override void CalculatePossibleMoves(IBoardService board, TileData currentTile, Action<Move> onPossibleMoveFound)
    {
        _moveStrategy.TryGetMoves(board, pieceData.Color, currentTile, onPossibleMoveFound);
    }
}
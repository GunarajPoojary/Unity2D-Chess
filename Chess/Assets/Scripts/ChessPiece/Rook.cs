using System;

public class Rook : ChessPiece
{
    private readonly IMoveStrategy _moveStrategy = new RookMoveStrategy();

    public override void CalculatePossibleMoves(IBoardService board, TileData currentTile, Action<Move> onPossibleMoveFound)
    {
        _moveStrategy.TryGetMoves(board, pieceData.Color, currentTile, onPossibleMoveFound);
    }
}
using System;

public class Bishop : ChessPiece
{
    private readonly IMoveStrategy _moveStrategy = new BishopMoveStrategy();

    public override void CalculatePossibleMoves(IBoardService board, TileData currentTile, Action<Move> onPossibleMoveFound)
    {
        _moveStrategy.TryGetMoves(board, pieceData.Color, currentTile, onPossibleMoveFound);
    }
}
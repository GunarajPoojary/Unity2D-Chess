using System;

public class Queen : ChessPiece
{
    private readonly IMoveStrategy _moveStrategy = new QueenMoveStrategy();

    public override void CalculatePossibleMoves(IBoardService board, TileData currentTile, Action<Move> onPossibleMoveFound)
    {
        _moveStrategy.TryGetMoves(board, pieceData.Color, currentTile, onPossibleMoveFound);
    }
}
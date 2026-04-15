using System;

public class Knight : ChessPiece
{
    private readonly IMoveStrategy _moveStrategy = new KnightMoveStrategy();

    public override void CalculatePossibleMoves(IBoardService board, TileData currentTile, Action<Move> onPossibleMoveFound)
    {
        _moveStrategy.TryGetMoves(board, pieceData.Color, currentTile, onPossibleMoveFound);
    }
}
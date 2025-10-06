using System;
using UnityEngine;

public class King : ChessPiece
{
    public override void CalculateLegalMoves(Action<Vector2Int, bool> onLegalMoveFound)
    {
        foreach (Vector2Int dir in DirectionUtility.KingMoves)
        {
            Vector2Int target = CurrentTile + dir;

            if (!ChessBoard.IsInsideBoard(target)) continue;

            if (ChessBoard.IsTileEmpty(target))
            {
                onLegalMoveFound?.Invoke(target, false);
            }
            else if (ChessBoard.TryGetOccupiedPiece(target, out ChessPiece piece) && piece.Color != Color)
            {
                onLegalMoveFound?.Invoke(target, true);
            }
        }
    }
}
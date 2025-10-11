using System;
using UnityEngine;

public class King : ChessPiece
{
    public override void CalculatePossibleMoves(Action<Vector2Int, ChessPiece> onPossibleMoveFound)
    {
        foreach (Vector2Int dir in DirectionUtility.KingMoves)
        {
            Vector2Int target = CurrentTile + dir;

            if (!ChessBoard.IsInsideBoard(target)) continue;

            if (ChessBoard.IsTileEmpty(target))
            {
                onPossibleMoveFound?.Invoke(target, null);
            }
            else if (ChessBoard.TryGetOccupiedPiece(target, out ChessPiece piece) && piece.Color != Color)
            {
                onPossibleMoveFound?.Invoke(target, piece);
            }
        }
    }
}
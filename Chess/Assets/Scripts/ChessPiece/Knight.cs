using System;
using UnityEngine;

public class Knight : ChessPiece
{
    public override void CalculatePossibleMoves(Action<Vector2Int, ChessPiece> onPossibleMoveFound)
    {
        foreach (Vector2Int jump in DirectionUtility.KnightMoves)
        {
            Vector2Int to = CurrentTile + jump;

            if (!ChessBoard.IsInsideBoard(to)) continue;

            if (ChessBoard.IsTileEmpty(to))
            {
                onPossibleMoveFound?.Invoke(to, null);
            }
            else if (ChessBoard.TryGetOccupiedPiece(to, out ChessPiece piece) && piece.Color != Color)
            {
                onPossibleMoveFound?.Invoke(to, piece);
            }
        }
    }
}
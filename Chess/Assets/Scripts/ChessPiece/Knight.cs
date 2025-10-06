using System;
using UnityEngine;

public class Knight : ChessPiece
{
    public override void CalculateLegalMoves(Action<Vector2Int, ChessPiece> onLegalMoveFound)
    {
        foreach (Vector2Int jump in DirectionUtility.KnightMoves)
        {
            Vector2Int to = CurrentTile + jump;

            if (!ChessBoard.IsInsideBoard(to)) continue;

            if (ChessBoard.IsTileEmpty(to))
            {
                onLegalMoveFound?.Invoke(to, null);
            }
            else if (ChessBoard.TryGetOccupiedPiece(to, out ChessPiece piece) && piece.Color != Color)
            {
                onLegalMoveFound?.Invoke(to, piece);
            }
        }
    }
}
using System;
using UnityEngine;

public abstract class SlidingChessPiece : ChessPiece
{
    protected abstract Vector2Int[] Directions { get; }

    public override void CalculatePossibleMoves(Action<Vector2Int, bool> onPossibleMoveFound)
    {
        foreach (Vector2Int dir in Directions)
        {
            Vector2Int to = CurrentTile + dir;

            while (ChessBoard.IsInsideBoard(to))
            {
                if (ChessBoard.IsTileEmpty(to))
                {
                    onPossibleMoveFound?.Invoke(to, false);
                    to += dir;
                }
                else if (ChessBoard.TryGetOccupiedPiece(to, out ChessPiece piece) && piece.Color != Color)
                {
                    onPossibleMoveFound?.Invoke(to, true);
                    break;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
using System;
using UnityEngine;

public class Pawn : ChessPiece
{
    [SerializeField] private MovementDirection _movementDirection;
    private bool _hasMoved = false;

    public override void CalculatePossibleMoves(Action<Vector2Int, ChessPiece> onPossibleMoveFound)
    {
        int direction = (int)_movementDirection;
        Vector2Int forwardOne = CurrentTile + DirectionUtility.Up * direction;

        if (!ChessBoard.IsInsideBoard(forwardOne)) return;

        // Forward move
        if (ChessBoard.IsTileEmpty(forwardOne))
        {
            onPossibleMoveFound?.Invoke(forwardOne, null);

            // Two-step move if pawn hasn't moved
            if (!_hasMoved)
            {
                Vector2Int forwardTwo = CurrentTile + DirectionUtility.Up * direction * 2;
                if (ChessBoard.IsInsideBoard(forwardTwo) && ChessBoard.IsTileEmpty(forwardTwo))
                    onPossibleMoveFound?.Invoke(forwardTwo, null);
            }
        }

        // Diagonal captures
        Vector2Int[] diagonals = { DirectionUtility.UpLeft, DirectionUtility.UpRight };
        foreach (var diag in diagonals)
        {
            Vector2Int target = CurrentTile + diag * direction;
            if (ChessBoard.IsInsideBoard(target) &&
                ChessBoard.TryGetOccupiedPiece(target, out ChessPiece piece) &&
                piece != null && piece.Color != Color)
            {
                onPossibleMoveFound?.Invoke(target, piece);
            }
        }
    }

    public override void SetPiecePosition(Vector2Int position)
    {
        base.SetPiecePosition(position);
        _hasMoved = true;
    }
}
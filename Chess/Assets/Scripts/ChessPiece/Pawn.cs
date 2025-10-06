using System;
using UnityEngine;

public class Pawn : ChessPiece
{
    [SerializeField] private MovementDirection _movementDirection;
    private bool _hasMoved = false;

    public override void CalculateLegalMoves(Action<Vector2Int, bool> onLegalMoveFound)
    {
        int direction = (int)_movementDirection;
        Vector2Int forwardOne = CurrentTile + DirectionUtility.Up * direction;

        if (!ChessBoard.IsInsideBoard(forwardOne)) return;

        // Forward move
        if (ChessBoard.IsTileEmpty(forwardOne))
        {
            onLegalMoveFound?.Invoke(forwardOne, false);

            // Two-step move if pawn hasn't moved
            if (!_hasMoved)
            {
                Vector2Int forwardTwo = CurrentTile + DirectionUtility.Up * direction * 2;
                if (ChessBoard.IsInsideBoard(forwardTwo) && ChessBoard.IsTileEmpty(forwardTwo))
                    onLegalMoveFound?.Invoke(forwardTwo, false);
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
                onLegalMoveFound?.Invoke(target, true);
            }
        }
    }

    public override void SetPiecePosition(Vector2Int position)
    {
        base.SetPiecePosition(position);
        _hasMoved = true;
    }
}
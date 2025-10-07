using System;
using UnityEngine;

public class MoveValidator : MonoBehaviour
{
    [SerializeField] private ChessBoard _board;

    private void OnEnable()
    {
        GameEvents.OnPieceMovedEventRaised += OnPieceMoved;
    }

    private void OnDisable()
    {
        GameEvents.OnPieceMovedEventRaised -= OnPieceMoved;
    }

    private void OnPieceMoved(ChessPiece piece, Vector2Int to)
    {
        if (_board.IsKingInCheck(TeamColor.White))
        {
            Debug.Log($"King is in check after moving {piece.name} to {to}");
        }
        else if (_board.IsKingInCheck(TeamColor.Black))
        {
            Debug.Log($"King is in check after moving {piece.name} to {to}");
        }
    }

    public void GetLegalMoves(ChessPiece piece, Action<Vector2Int, bool> onLegalMoveFound)
    {
        piece.CalculatePossibleMoves((position, isOccupiedByOpponent) =>
        {
            if (piece != null && _board.IsKingInCheck(piece.Color))
            {
                Vector2Int previousPosition = piece.CurrentTile;

                ChessBoard.MovePiece(piece, position);

                if (_board.IsKingInCheck(ServiceLocator.Get<IPlayerContext>().Color))
                {
                    ChessBoard.MovePiece(piece, previousPosition);
                    return;
                }

                ChessBoard.MovePiece(piece, previousPosition);
            }

            onLegalMoveFound?.Invoke(position, isOccupiedByOpponent);
        });
    }
}
using System;
using UnityEngine;

public class MoveValidator : MonoBehaviour
{
    [SerializeField] private ChessBoard _board;
    private ChessPiece _selectedPiece = null;
    private Action<Vector2Int, bool> _onLegalMoveFound = delegate { };

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
        if (IsKingInCheck(TeamColor.White))
            GameEvents.RaiseHighlightEvent(ChessBoard.WhiteKing.CurrentTile, HighlightType.Check);
        else
            GameEvents.RaiseUnHighlightEvent(ChessBoard.WhiteKing.CurrentTile);

        if (IsKingInCheck(TeamColor.Black))
            GameEvents.RaiseHighlightEvent(ChessBoard.BlackKing.CurrentTile, HighlightType.Check);
        else
            GameEvents.RaiseUnHighlightEvent(ChessBoard.BlackKing.CurrentTile);
    }

    public void GetLegalMoves(ChessPiece piece, Action<Vector2Int, bool> onLegalMoveFound)
    {
        if (piece == null) return;

        TeamColor opponentColor = piece.Color == TeamColor.White ? TeamColor.Black : TeamColor.White;
        _selectedPiece = piece;
        _onLegalMoveFound = onLegalMoveFound;

        piece.CalculatePossibleMoves(OnPossibleMoveFound);
    }

    private void OnPossibleMoveFound(Vector2Int position, bool isOccupiedByOpponent)
    {
        Vector2Int previousPosition = _selectedPiece.CurrentTile;

        ChessBoard.MovePiece(_selectedPiece, position);

        if (IsKingInCheck(_selectedPiece.Color))
        {
            ChessBoard.MovePiece(_selectedPiece, previousPosition);
            return;
        }
        
        ChessBoard.MovePiece(_selectedPiece, previousPosition);

        _onLegalMoveFound?.Invoke(position, isOccupiedByOpponent);
    }

    private bool IsKingInCheck(TeamColor color)
    {
        ChessPiece targetKing = color == TeamColor.White ? ChessBoard.WhiteKing : ChessBoard.BlackKing;
        if (targetKing == null) return false;

        Vector2Int kingPos = targetKing.CurrentTile;
        ChessPiece[] opponentPieces = color == TeamColor.White ? _board.BlackPieces : _board.WhitePieces;

        foreach (ChessPiece piece in opponentPieces)
        {
            if (piece == null || !piece.gameObject.activeSelf) continue;

            bool isAttackingKing = false;

            piece.CalculatePossibleMoves((movePos, isOpponentTile) =>
            {
                if (movePos == kingPos) isAttackingKing = true;
            });

            if (isAttackingKing) return true;
        }

        return false;
    }

    private void CalculatePseudoLegalMoves()
    {

    }
}
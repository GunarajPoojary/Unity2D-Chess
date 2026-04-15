using System;
using UnityEngine;

public class MoveValidator : MonoBehaviour
{
    // [SerializeField] private ChessBoard _board;
    private ChessPiece _selectedPiece = null;
    private Action<Vector2Int, bool> _onLegalMoveFound = delegate { };

    private bool _isCheck;
    private bool _hasLegalMoves;
    private ChessPiece[] _attackingPieces;
    private King _opponentKing;
    private ChessPiece _currentCheckingPiece;
    private bool _validMoveFound;
    private TileData _targetTile;

    private void OnEnable()
    {
        // GameEvents.OnPieceMovedEventRaised += OnPieceMoved;
    }

    private void OnDisable()
    {
        // GameEvents.OnPieceMovedEventRaised -= OnPieceMoved;
    }

    private void OnPieceMoved(ChessPiece piece, Vector2Int to)
    {
        // _opponentKing = piece.Color == TeamColor.Light ? _board.BlackKing : _board.WhiteKing;
        // _isCheck = false;
        // _currentCheckingPiece = piece;

        // piece.CalculatePossibleMoves(HandlePossibleMove);

        // ChessPiece[] opponentPieces = piece.Color == TeamColor.Light ? _board.BlackPieces : _board.WhitePieces;
        // _attackingPieces = piece.Color == TeamColor.Light ? _board.WhitePieces : _board.BlackPieces;

        // _hasLegalMoves = false;

        // foreach (ChessPiece opponentPiece in opponentPieces)
        // {
        //     if (opponentPiece == null || !opponentPiece.gameObject.activeSelf)
        //         continue;

        //     SimulateOpponentMoves(opponentPiece);

        //     if (_hasLegalMoves)
        //         break;
        // }

        // if (_isCheck)
        // {
        //     if (!_hasLegalMoves)
        //     {
        //         Debug.Log("CHECKMATE – King has no legal moves!");
        //     }
        //     else
        //     {
        //         Debug.Log("CHECK – King is under attack!");
        //     }
        //     // GameEvents.RaiseHighlightEvent(_opponentKing.CurrentTile, HighlightType.Check);
        // }
        // else
        // {
        //     if (!_hasLegalMoves)
        //     {
        //         Debug.Log("STALEMATE – No legal moves but not in check.");
        //         GameEvents.RaiseDrawEvent();
        //     }
        // }
    }

    private void HandlePossibleMove(Vector2Int tile, ChessPiece opponentPiece)
    {
        // if (tile == _opponentKing.CurrentTile)
        // {
        //     _isCheck = true;
        // }
    }

    private void SimulateOpponentMoves(ChessPiece opponentPiece)
    {
        _currentCheckingPiece = opponentPiece;
        // opponentPiece.CalculatePossibleMoves(CheckIfLegalEscapeMove);
    }

    private void CheckIfLegalEscapeMove(Vector2Int movePos, ChessPiece opponentPiece)
    {
        // Vector2Int fromPos = _currentCheckingPiece.CurrentTile;
        // ChessBoard.PseudoMovePiece(_currentCheckingPiece, fromPos, movePos);

        // King opponentKing = _opponentKing;
        // Vector2Int kingPosToCheck = _currentCheckingPiece is King ? movePos : opponentKing.CurrentTile;

        // bool stillInCheck = IsUnderAttack(kingPosToCheck, _attackingPieces, opponentPiece);

        // ChessBoard.UndoPseudoMove(_currentCheckingPiece, fromPos, movePos, opponentPiece);

        // if (!stillInCheck)
        // {
        //     _hasLegalMoves = true;
        // }
    }

    public void GetLegalMoves(ChessPiece piece, Action<Vector2Int, bool> onLegalMoveFound)
    {
        if (piece == null) return;

        _selectedPiece = piece;
        _onLegalMoveFound = onLegalMoveFound;

        // piece.CalculatePossibleMoves(CheckLegalMove);

        _selectedPiece = null;
        _onLegalMoveFound = null;
    }

    private void CheckLegalMove(Vector2Int targetPosition, ChessPiece opponentPiece)
    {
        // Vector2Int currentPosition = _selectedPiece.CurrentTile;

        // ChessBoard.PseudoMovePiece(_selectedPiece, currentPosition, targetPosition);

        // King targetKing = _selectedPiece.Color == TeamColor.Light ? ChessBoard.WhiteKing : ChessBoard.BlackKing;
        // ChessPiece[] opponentPieces = _selectedPiece.Color == TeamColor.Light ? _board.BlackPieces : _board.WhitePieces;

        // Vector2Int kingPosToCheck = _selectedPiece == targetKing ? targetPosition : targetKing.CurrentTile;

        // bool isUnderAttack = IsUnderAttack(kingPosToCheck, opponentPieces, opponentPiece);

        // ChessBoard.UndoPseudoMove(_selectedPiece, currentPosition, targetPosition, opponentPiece);

        // if (!isUnderAttack)
        //     _onLegalMoveFound?.Invoke(targetPosition, opponentPiece);
    }

    private bool IsUnderAttack(Vector2Int position, ChessPiece[] attackingPieces, ChessPiece excludePiece = null)
    {
        bool isUnderAttack = false;

        foreach (ChessPiece piece in attackingPieces)
        {
            // if (piece == null || !piece.gameObject.activeSelf || piece == excludePiece)
            //     continue;

            // if (piece.CurrentTile == position)
            //     continue;

            // piece.CalculatePossibleMoves((movePos, isOpponentTile) =>
            // {
            //     if (movePos == position)
            //         isUnderAttack = true;
            // });

            if (isUnderAttack)
                break;
        }

        return isUnderAttack;
    }

    public bool IsValidMove(Collider2D hitCollider, ChessPiece selectedPiece)
    {
        // _board.TryGetTile(hitCollider, out _targetTile);
        // selectedPiece.CalculatePossibleMoves(OnPossibleMoveFound);

        return _validMoveFound;
    }

    private void OnPossibleMoveFound(Move move)
    {
        if (move.To == _targetTile)
        _validMoveFound = true;
    }
}
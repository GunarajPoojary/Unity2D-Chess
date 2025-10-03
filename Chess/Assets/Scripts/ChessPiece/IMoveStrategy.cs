using System;
using UnityEngine;

public interface IMoveStrategy
{
    void CalculateLegalMoves(
        TeamColor color,
        Vector2Int currentTile,
        Action<Vector2Int, ChessPiece> onLegalMoveFound);
}

public class PawnMove : IMoveStrategy
{
    private readonly IMoveStrategy _singleStepMove;
    private readonly IMoveStrategy _doubleStepMove;
    private readonly IMoveStrategy _captureMove;
    private IMoveStrategy _currentNonCaptureMove;

    private bool _hasMoved;

    public PawnMove(IBoardQuery board, BoardSide side)
    {
        // Calculate the forward direction based on the board side
        // side is either 1 (Up) or -1 (Down), so this correctly orients the pawn
        Vector2Int forwardDirection = Directions.Up * (int)side;

        // Non-capture moves
        _singleStepMove = new SingleStepMove(board, forwardDirection);
        _doubleStepMove = new DoubleStepMove(board, forwardDirection);

        _currentNonCaptureMove = _doubleStepMove;

        // Capture moves (diagonal left/right depending on side)
        Vector2Int[] captureDirs = new[]
        {
            Directions.UpLeft * (int)side,
            Directions.UpRight * (int)side
        };
        _captureMove = new DiagonalCaptureMove(board, captureDirs);
    }

    public void CalculateLegalMoves(
        TeamColor color,
        Vector2Int currentTile,
        Action<Vector2Int, ChessPiece> onLegalMoveFound)
    {
        // Forward (non-capture)
        _currentNonCaptureMove.CalculateLegalMoves(color, currentTile, onLegalMoveFound);

        // Diagonal captures
        _captureMove.CalculateLegalMoves(color, currentTile, onLegalMoveFound);
    }

    /// <summary>
    /// Call this after the pawn moves, so it can no longer do the double step.
    /// </summary>
    public void SwitchToSingleStep()
    {
        if (!_hasMoved)
        {
            _hasMoved = true;
            _currentNonCaptureMove = _singleStepMove;
        }
    }

    /// <summary>
    /// Returns whether this pawn has moved from its starting position.
    /// </summary>
    public bool HasMoved => _hasMoved;
}

public class QueenMove : IMoveStrategy
{
    private readonly IMoveStrategy _move;

    public QueenMove(IBoardQuery board)
    {
        _move = new MultiStepMove(board, Directions.EightDirections);
    }

    public void CalculateLegalMoves(
        TeamColor color,
        Vector2Int currentTile,
        Action<Vector2Int, ChessPiece> onLegalMoveFound) =>
        _move.CalculateLegalMoves(color, currentTile, onLegalMoveFound);
}

public class KnightMove : MoveStrategyBase
{
    public KnightMove(IBoardQuery board) : base(board) { }

    public override void CalculateLegalMoves(
        TeamColor color,
        Vector2Int currentTile,
        Action<Vector2Int, ChessPiece> onLegalMoveFound)
    {
        foreach (Vector2Int jump in Directions.KnightMoves)
        {
            Vector2Int to = currentTile + jump;
            if (IsInsideBoard(to) && !HasAlly(to, color))
            {
                if (TryGetOpponent(to, color, out ChessPiece piece))
                    onLegalMoveFound?.Invoke(to, piece);
                else
                    onLegalMoveFound?.Invoke(to, null);
            }
        }
    }
}

public class KingMove : MoveStrategyBase
{
    public KingMove(IBoardQuery board) : base(board) { }

    public override void CalculateLegalMoves(
        TeamColor color,
        Vector2Int currentTile,
        Action<Vector2Int, ChessPiece> onLegalMoveFound)
    {
        foreach (Vector2Int dir in Directions.KingMoves)
        {
            Vector2Int to = currentTile + dir;
            if (IsInsideBoard(to) && !HasAlly(to, color))
            {
                if (TryGetOpponent(to, color, out ChessPiece piece))
                    onLegalMoveFound?.Invoke(to, piece);
                else
                    onLegalMoveFound?.Invoke(to, null);
            }
        }
    }
}

public class RookMove : IMoveStrategy
{
    private readonly IMoveStrategy _move;

    public RookMove(IBoardQuery board)
    {
        _move = new MultiStepMove(board, Directions.Orthogonals);
    }

    public void CalculateLegalMoves(
        TeamColor color,
        Vector2Int currentTile,
        Action<Vector2Int, ChessPiece> onLegalMoveFound) =>
        _move.CalculateLegalMoves(color, currentTile, onLegalMoveFound);
}

public class BishopMove : IMoveStrategy
{
    private readonly IMoveStrategy _move;

    public BishopMove(IBoardQuery board)
    {
        _move = new MultiStepMove(board, Directions.Diagonals);
    }

    public void CalculateLegalMoves(
        TeamColor color,
        Vector2Int currentTile,
        Action<Vector2Int, ChessPiece> onLegalMoveFound) =>
        _move.CalculateLegalMoves(color, currentTile, onLegalMoveFound);
}

public class MultiStepMove : MoveStrategyBase
{
    private readonly Vector2Int[] _directions;

    public MultiStepMove(IBoardQuery board, Vector2Int[] directions) : base(board)
    {
        _directions = directions;
    }

    public override void CalculateLegalMoves(
        TeamColor color,
        Vector2Int currentTile,
        Action<Vector2Int, ChessPiece> onLegalMoveFound)
    {
        foreach (Vector2Int dir in _directions)
        {
            Vector2Int to = currentTile + dir;

            while (IsInsideBoard(to))
            {
                if (IsTileEmpty(to))
                {
                    onLegalMoveFound?.Invoke(to, null);
                    to += dir;
                }
                else if (TryGetOpponent(to, color, out ChessPiece piece))
                {
                    onLegalMoveFound?.Invoke(to, piece);
                    break;
                }
                else break;
            }
        }
    }
}

public class SingleStepMove : MoveStrategyBase
{
    private readonly Vector2Int _direction;

    public SingleStepMove(IBoardQuery board, Vector2Int direction) : base(board)
    {
        _direction = direction;
    }

    public override void CalculateLegalMoves(
        TeamColor color,
        Vector2Int currentTile,
        Action<Vector2Int, ChessPiece> onLegalMoveFound)
    {
        Vector2Int to = currentTile + _direction;

        if (IsInsideBoard(to) && IsTileEmpty(to))
            onLegalMoveFound?.Invoke(to, null);
    }
}

public class DoubleStepMove : MoveStrategyBase
{
    private readonly Vector2Int _direction;

    public DoubleStepMove(IBoardQuery board, Vector2Int direction) : base(board)
    {
        _direction = direction;
    }

    public override void CalculateLegalMoves(
        TeamColor color,
        Vector2Int currentTile,
        Action<Vector2Int, ChessPiece> onLegalMoveFound)
    {
        Vector2Int oneStep = currentTile + _direction;
        Vector2Int twoStep = currentTile + _direction * 2;

        if (IsInsideBoard(oneStep) && IsTileEmpty(oneStep))
        {
            onLegalMoveFound?.Invoke(oneStep, null);

            if (IsInsideBoard(twoStep) && IsTileEmpty(twoStep))
                onLegalMoveFound?.Invoke(twoStep, null);
        }
    }
}

public class DiagonalCaptureMove : MoveStrategyBase
{
    private readonly Vector2Int[] _captureDirections;

    public DiagonalCaptureMove(IBoardQuery board, Vector2Int[] captureDirections) : base(board)
    {
        _captureDirections = captureDirections;
    }

    public override void CalculateLegalMoves(
        TeamColor color,
        Vector2Int currentTile,
        Action<Vector2Int, ChessPiece> onLegalMoveFound)
    {
        foreach (Vector2Int dir in _captureDirections)
        {
            Vector2Int to = currentTile + dir;
            
            // Validate position is on the board before checking for opponent
            if (IsInsideBoard(to) && TryGetOpponent(to, color, out ChessPiece piece))
                onLegalMoveFound?.Invoke(to, piece);
        }
    }
}
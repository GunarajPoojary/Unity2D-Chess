using System;
using UnityEngine;

public abstract class MoveStrategyBase : IMoveStrategy
{
    protected readonly IBoardQuery Board;

    protected MoveStrategyBase(IBoardQuery board)
    {
        Board = board ?? throw new ArgumentNullException(nameof(board));
    }

    public abstract void CalculateLegalMoves(
        TeamColor color,
        Vector2Int currentTile,
        Action<Vector2Int, ChessPiece> onLegalMoveFound);

    protected bool IsInsideBoard(Vector2Int pos) => BoardUtilities.IsInsideBoard(pos);
    protected bool IsTileEmpty(Vector2Int pos) => Board.IsTileEmpty(pos);
    protected bool TryGetOpponent(Vector2Int pos, TeamColor color, out ChessPiece piece) => Board.TryGetOpponent(pos, color, out piece);
    protected bool HasAlly(Vector2Int pos, TeamColor color) => Board.HasAlly(pos, color);
}
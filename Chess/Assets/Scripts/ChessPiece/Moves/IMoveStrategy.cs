using System;
using UnityEngine;

public interface IMoveStrategy
{
    bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onMoveFoundAction);
}

public abstract class PawnMove
{
    protected readonly bool _isUpward;

    public PawnMove(bool isUpward)
    {
        _isUpward = isUpward;
    }
}

public class PawnDoubleStepMove : PawnMove, IMoveStrategy
{
    public PawnDoubleStepMove(bool isUpward) : base(isUpward) { }

    public bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onMoveFoundAction)
    {
        int dir = _isUpward ? 1 : -1;

        TileData oneStep = currentTile + new TileData(0, dir);
        TileData twoStep = currentTile + new TileData(0, dir * 2);

        if (!board.IsWithinBoard(twoStep)) return false;
        if (!board.IsTileEmptyAt(oneStep)) return false;
        if (!board.IsTileEmptyAt(twoStep)) return false;

        onMoveFoundAction?.Invoke(new Move(currentTile, twoStep, null));
        return true;
    }
}

public class PawnSingleStepMove : PawnMove, IMoveStrategy
{
    public PawnSingleStepMove(bool isUpward) : base(isUpward) { }

    public bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onMoveFoundAction)
    {
        int dir = _isUpward ? 1 : -1;

        TileData targetTile = currentTile + new TileData(0, dir);

        if (!board.IsWithinBoard(targetTile)) return false;
        if (!board.IsTileEmptyAt(targetTile)) return false;

        onMoveFoundAction?.Invoke(new Move(currentTile, targetTile, null));
        return true;
    }
}

public class PawnCaptureMove : PawnMove, IMoveStrategy
{
    public PawnCaptureMove(bool isUpward) : base(isUpward) { }

    public bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onMoveFoundAction)
    {
        int dir = _isUpward ? 1 : -1;

        TileData[] captureOffsets =
        {
            new TileData(1,dir),
            new TileData(-1,dir),
        };

        bool foundMove = false;

        foreach (TileData offset in captureOffsets)
        {
            TileData targetTile = currentTile + offset;

            if (!board.IsWithinBoard(targetTile))
                continue;

            if (board.TryGetOpponent(color, targetTile, out PieceData targetPiece))
            {
                onMoveFoundAction?.Invoke(new Move(currentTile, targetTile, targetPiece));
                foundMove = true;
            }
        }

        return foundMove;
    }
}

public class QueenMoveStrategy : IMoveStrategy
{
    private readonly RookMoveStrategy _rook = new RookMoveStrategy();
    private readonly BishopMoveStrategy _bishop = new BishopMoveStrategy();

    public bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onMoveFoundAction)
    {
        bool rookMoves = _rook.TryGetMoves(board, color, currentTile, onMoveFoundAction);
        bool bishopMoves = _bishop.TryGetMoves(board, color, currentTile, onMoveFoundAction);

        return rookMoves || bishopMoves;
    }
}

public class RookMoveStrategy : IMoveStrategy
{
    private static readonly TileData[] Directions =
    {
        TileData.Up, TileData.Down,
        TileData.Left, TileData.Right
    };

    public bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onMoveFoundAction)
    {
        bool foundMove = false;

        foreach (TileData dir in Directions)
        {
            TileData current = currentTile;

            while (true)
            {
                current += dir;

                if (!board.IsWithinBoard(current))
                    break;

                if (board.IsTileEmptyAt(current))
                {
                    onMoveFoundAction?.Invoke(new Move(currentTile, current, null));
                    foundMove = true;
                }
                else
                {
                    if (board.TryGetOpponent(color, current, out PieceData opponent))
                    {
                        onMoveFoundAction?.Invoke(new Move(currentTile, current, opponent));
                        foundMove = true;
                    }
                    break;
                }
            }
        }

        return foundMove;
    }
}

public class KnightMoveStrategy : IMoveStrategy
{
    private static readonly TileData[] Offsets =
    {
        new TileData(1,2), new TileData(2,1),
        new TileData(-1,2), new TileData(-2,1),
        new TileData(1,-2), new TileData(2,-1),
        new TileData(-1,-2), new TileData(-2,-1)
    };

    public bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onMoveFoundAction)
    {
        bool foundMove = false;

        foreach (TileData offset in Offsets)
        {
            TileData target = currentTile + offset;

            if (!board.IsWithinBoard(target))
                continue;

            if (board.IsTileEmptyAt(target))
            {
                onMoveFoundAction?.Invoke(new Move(currentTile, target, null));
                foundMove = true;
            }
            else
            {
                if (board.TryGetOpponent(color, target, out PieceData opponent))
                {
                    onMoveFoundAction?.Invoke(new Move(currentTile, target, opponent));
                    foundMove = true;
                }
            }
        }

        return foundMove;
    }
}

public class KingMoveStrategy : IMoveStrategy
{
    private static readonly TileData[] Directions =
    {
        TileData.Up, TileData.Down,
        TileData.Left, TileData.Right,
        TileData.UpRight, TileData.DownRight,
        TileData.DownLeft, TileData.UpLeft,
    };

    public bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onMoveFoundAction)
    {
        bool foundMove = false;

        foreach (TileData dir in Directions)
        {
            TileData target = currentTile + dir;

            if (!board.IsWithinBoard(target))
                continue;

            if (board.IsTileEmptyAt(target))
            {
                onMoveFoundAction?.Invoke(new Move(currentTile, target, null));
                foundMove = true;
            }
            else
            {
                if (board.TryGetOpponent(color, target, out PieceData opponent))
                {
                    onMoveFoundAction?.Invoke(new Move(currentTile, target, opponent));
                    foundMove = true;
                }
            }
        }

        return foundMove;
    }
}

public class BishopMoveStrategy : IMoveStrategy
{
    private static readonly TileData[] Directions =
    {
        TileData.UpRight, TileData.DownRight,
        TileData.DownLeft, TileData.UpLeft
    };

    public bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onMoveFoundAction)
    {
        bool foundMove = false;

        foreach (TileData dir in Directions)
        {
            TileData current = currentTile;

            while (true)
            {
                current += dir;

                if (!board.IsWithinBoard(current))
                    break;

                if (board.IsTileEmptyAt(current))
                {
                    onMoveFoundAction?.Invoke(new Move(currentTile, current, null));
                    foundMove = true;
                }
                else
                {
                    if (board.TryGetOpponent(color, current, out PieceData opponent))
                    {
                        onMoveFoundAction?.Invoke(new Move(currentTile, current, opponent));
                        foundMove = true;
                    }
                    break;
                }
            }
        }

        return foundMove;
    }
}
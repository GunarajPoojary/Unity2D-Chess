using System;
using UnityEngine;

public class PawnSingleStepMove : PawnMove, IMoveStrategy
{
    public PawnSingleStepMove(bool isUpward) : base(isUpward) { }

    public bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onGetLegalMoveAction)
    {
        int dir = _isUpward ? 1 : -1;
        TileData targetTile = currentTile + new TileData(dir, 0);

        if (!board.IsWithinBoard(targetTile))
        {
            Debug.Log($"[PawnSingleStep] Target {targetTile} is outside the board.");
            return false;
        }

        if (!board.IsTileEmptyAt(targetTile))
        {
            Debug.Log($"[PawnSingleStep] Target {targetTile} is blocked.");
            return false;
        }

        Debug.Log($"[PawnSingleStep] Valid single step from {currentTile} to {targetTile}");
        onGetLegalMoveAction?.Invoke(new Move(currentTile, targetTile, null));
        return true;
    }
}
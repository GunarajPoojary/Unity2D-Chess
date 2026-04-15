using System;
using UnityEngine;

public class PawnDoubleStepMove : PawnMove, IMoveStrategy
{
    public PawnDoubleStepMove(bool isUpward) : base(isUpward) { }

    public bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onGetLegalMoveAction)
    {
        int dir = _isUpward ? 1 : -1;

        TileData oneStep = currentTile + new TileData(dir, 0);
        TileData twoStep = currentTile + new TileData(dir * 2, 0);

        if (!board.IsWithinBoard(twoStep))
        {
            Debug.Log($"[PawnDoubleStep] Target {twoStep} is outside the board.");
            return false;
        }

        if (!board.IsTileEmptyAt(oneStep))
        {
            Debug.Log($"[PawnDoubleStep] Intermediate tile {oneStep} is blocked — cannot double step.");
            return false;
        }

        if (!board.IsTileEmptyAt(twoStep))
        {
            Debug.Log($"[PawnDoubleStep] Target {twoStep} is blocked.");
            return false;
        }

        Debug.Log($"[PawnDoubleStep] Valid double step from {currentTile} to {twoStep}");
        onGetLegalMoveAction?.Invoke(new Move(currentTile, twoStep, null));
        return true;
    }
}
using System;
using UnityEngine;

public class PawnCaptureMove : PawnMove, IMoveStrategy
{
    public PawnCaptureMove(bool isUpward) : base(isUpward) { }

    public bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onGetLegalMoveAction)
    {
        int dir = _isUpward ? 1 : -1;

        TileData[] captureOffsets =
        {
            new TileData(dir,  1),  
            new TileData(dir, -1),  
        };

        bool foundMove = false;

        foreach (TileData offset in captureOffsets)
        {
            TileData targetTile = currentTile + offset;

            if (!board.IsWithinBoard(targetTile))
                continue;

            if (board.TryGetPieceByColor(color==TeamColor.Light?TeamColor.Dark:TeamColor.Light, targetTile, out ChessPiece targetPiece))
            {
                Debug.Log($"[PawnCapture] Valid capture from {currentTile} to {targetTile} " +
                          $"(captures {targetPiece.name})");
                onGetLegalMoveAction?.Invoke(new Move(currentTile, targetTile, targetPiece));
                foundMove = true;
            }
        }

        return foundMove;
    }
}
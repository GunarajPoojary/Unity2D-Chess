using System;

public class KnightMoveStrategy : IMoveStrategy
{
    private static readonly TileData[] Offsets =
    {
        new TileData(1,2), new TileData(2,1),
        new TileData(-1,2), new TileData(-2,1),
        new TileData(1,-2), new TileData(2,-1),
        new TileData(-1,-2), new TileData(-2,-1)
    };

    public bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onGetLegalMoveAction)
    {
        bool foundMove = false;

        foreach (TileData offset in Offsets)
        {
            TileData target = currentTile + offset;

            if (!board.IsWithinBoard(target))
                continue;

            if (board.IsTileEmptyAt(target))
            {
                onGetLegalMoveAction?.Invoke(new Move(currentTile, target, null));
                foundMove = true;
            }
            else
            {
                if (board.TryGetPieceByColor(color==TeamColor.Light?TeamColor.Dark:TeamColor.Light, target, out ChessPiece opponent))
                {
                    onGetLegalMoveAction?.Invoke(new Move(currentTile, target, opponent));
                    foundMove = true;
                }
            }
        }

        return foundMove;
    }
}
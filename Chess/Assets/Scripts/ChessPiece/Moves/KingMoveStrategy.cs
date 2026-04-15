using System;

public class KingMoveStrategy : IMoveStrategy
{
    private static readonly TileData[] Directions =
    {
        TileData.Up, TileData.Down,
        TileData.Left, TileData.Right,
        TileData.UpRight, TileData.DownRight,
        TileData.DownLeft, TileData.UpLeft,
    };

    public bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onGetLegalMoveAction)
    {
        bool foundMove = false;

        foreach (TileData dir in Directions)
        {
            TileData target = currentTile + dir;

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
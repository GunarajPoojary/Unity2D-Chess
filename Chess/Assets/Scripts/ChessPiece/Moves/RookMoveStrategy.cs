using System;

public class RookMoveStrategy : IMoveStrategy
{
    private static readonly TileData[] Directions =
    {
        TileData.Up, TileData.Down,
        TileData.Left, TileData.Right
    };

    public bool TryGetMoves(IBoardService board, TeamColor color, TileData currentTile, Action<Move> onGetLegalMoveAction)
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
                    onGetLegalMoveAction?.Invoke(new Move(currentTile, current, null));
                    foundMove = true;
                }
                else
                {
                    if (board.TryGetPieceByColor(color==TeamColor.Light?TeamColor.Dark:TeamColor.Light, current, out ChessPiece opponent))
                    {
                        onGetLegalMoveAction?.Invoke(new Move(currentTile, current, opponent));
                        foundMove = true;
                    }
                    break;
                }
            }
        }

        return foundMove;
    }
}
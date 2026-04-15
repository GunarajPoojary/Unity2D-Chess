public interface IBoardService
{
    bool IsWithinBoard(TileData tile);
    bool TryGetPieceAt(TileData tile, out ChessPiece piece);
    bool TryGetPieceByColor(TeamColor color, TileData tile, out ChessPiece piece);
    bool IsTileEmptyAt(TileData tile);
}
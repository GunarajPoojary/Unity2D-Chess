public interface IBoardService
{
    bool IsWithinBoard(TileData tile);
    bool TryGetOpponent(TeamColor selectedPieceColor, TileData tile, out PieceData piece);
    bool IsTileEmptyAt(TileData tile);
}
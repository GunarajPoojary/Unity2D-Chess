public class BoardState : IBoardService
{
    private Grid<PieceData> _boardState;

    public BoardState(Grid<PieceData> boardState)
    {
        _boardState = boardState;
    }

    public void SetBoardState(Grid<PieceData> boardState) => _boardState = boardState;

    public void SetPieceAt(TileData tile, PieceData piece)
    {
        ValidateTile(tile);

        _boardState[tile.columnIndex, tile.rowIndex] = piece;
    }

    public void MovePiece(TileData currentTile, TileData targetTile, PieceData piece)
    {
        SetPieceAt(currentTile, null);
        SetPieceAt(targetTile, piece);
    }

    public bool IsTileEmptyAt(TileData tile)
    {
        ValidateTile(tile);

        return _boardState[tile.columnIndex, tile.rowIndex] == null;
    }

    public bool IsWithinBoard(TileData tile) => _boardState.IsInside(tile.columnIndex, tile.rowIndex);

    public bool TryGetPieceAt(TileData tile, out PieceData piece)
    {
        ValidateTile(tile);

        piece = _boardState[tile.columnIndex, tile.rowIndex];

        if (piece == null)
            return false;

        return true;
    }

    public bool TryGetOpponent(TeamColor selectedPieceColor, TileData tile, out PieceData piece)
    {
        if (TryGetPieceAt(tile, out piece))
        {
            TeamColor opponentColor = selectedPieceColor == TeamColor.Light ? TeamColor.Dark : TeamColor.Light;

            if (piece.Color == opponentColor)
                return true;
        }
        
        piece = null;

        return false;
    }

    private void ValidateTile(TileData tile)
    {
        if (!IsWithinBoard(tile)) throw new System.ArgumentException("Tile is out of board");
    }
}
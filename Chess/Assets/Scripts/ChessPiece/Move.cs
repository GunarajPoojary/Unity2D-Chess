public class Move
{
    public TileData From;
    public TileData To;
    public PieceData CapturedPiece;

    public Move(TileData from, TileData to, PieceData capturedPiece)
    {
        From = from;
        To = to;
        CapturedPiece = capturedPiece;
    }
}
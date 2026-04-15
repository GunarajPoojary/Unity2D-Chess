public class Move
{
    public TileData From;
    public TileData To;
    public ChessPiece CapturedPiece;

    public Move(TileData from, TileData to, ChessPiece capturedPiece)
    {
        From = from;
        To = to;
        CapturedPiece = capturedPiece;
    }
}
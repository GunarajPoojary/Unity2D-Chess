using UnityEngine;

public class MoveExecutor : MonoBehaviour
{
    public static void Execute(ChessPiece piece, Vector2Int targetTile)
    {
        if (ChessBoard.TryGetOccupiedPiece(targetTile, out ChessPiece capturedPiece)
            && capturedPiece.Color != piece.Color)
        {
            capturedPiece.gameObject.SetActive(false);
            GameEvents.RaisePieceCapturedEvent(capturedPiece, targetTile);
        }

        ChessBoard.MovePiece(piece, targetTile);
        GameEvents.RaisePieceMovedEvent(piece, targetTile);
    }
}
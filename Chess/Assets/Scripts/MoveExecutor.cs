using UnityEngine;

public class MoveExecutor : MonoBehaviour
{
    // private ChessBoard _board;

    public void Execute(ChessPiece piece, Vector2Int targetTile)
    {
        // if (_board.TryGetOccupiedPiece(targetTile, out ChessPiece capturedPiece)
        //     && capturedPiece.Color != piece.Color)
        // {
        //     capturedPiece.gameObject.SetActive(false);
        //     GameEvents.RaisePieceCapturedEvent(capturedPiece, targetTile);
        // }

        // _board.MovePiece(piece, targetTile);
        // GameEvents.RaisePieceMovedEvent(piece, targetTile);
    }
}
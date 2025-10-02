using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    [SerializeField] private ChessPiece[] _whitePieces;
    [SerializeField] private ChessPiece[] _blackPieces;

    private readonly ChessPiece[,] _boardState = new ChessPiece[BOARD_SIZE, BOARD_SIZE];
    private const int BOARD_SIZE = 8;

    private void Awake()
    {
        InitializeBoardState();
    }

    private void InitializeBoardState()
    {
        // Initialize board by registering all pieces
        RegisterPieces(_whitePieces);
        RegisterPieces(_blackPieces);
    }

    /// <summary>
    /// Registers pieces into the board state array based on their position.
    /// </summary>
    private void RegisterPieces(ChessPiece[] chessPieces)
    {
        for (int i = 0; i < chessPieces.Length; i++)
        {
            Vector2Int piecePosition = chessPieces[i].PiecePosition;

            _boardState[piecePosition.x, piecePosition.y] = chessPieces[i];
        }
    }
}
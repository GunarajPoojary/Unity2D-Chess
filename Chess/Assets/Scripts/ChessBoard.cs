using UnityEngine;

/// <summary>
/// Manages the chess board state and piece registration.
/// Maintains a 2D array representing the 8x8 chess board and tracks all piece positions.
/// </summary>
public class ChessBoard : MonoBehaviour
{
    [SerializeField] private ChessPiece[] _whitePieces;
    [SerializeField] private ChessPiece[] _blackPieces;

    // 2D array representing the 8x8 chess board grid
    // Each cell contains a reference to the ChessPiece occupying that position, or null if empty
    private readonly ChessPiece[,] _boardState = new ChessPiece[BOARD_SIZE, BOARD_SIZE];
    
    // Standard chess board dimensions (8x8 grid)
    private const int BOARD_SIZE = 8;

    private void Awake()
    {
        InitializeBoardState();
    }

    // Initializes the chess board by registering all white and black pieces
    // into their respective positions in the board state array.
    private void InitializeBoardState()
    {
        RegisterPieces(_whitePieces);
        RegisterPieces(_blackPieces);
    }

    // Registers an array of chess pieces into the board state array based on their current positions.
    // Each piece is placed in the 2D array at coordinates matching its PiecePosition.
    private void RegisterPieces(ChessPiece[] chessPieces)
    {
        if (chessPieces == null) return;
        
        for (int i = 0; i < chessPieces.Length; i++)
        {
            // Get the current position of the piece from its transform
            Vector2Int piecePosition = chessPieces[i].PiecePosition;

            // Store the piece reference in the board state array at its position
            _boardState[piecePosition.x, piecePosition.y] = chessPieces[i];
        }
    }
}
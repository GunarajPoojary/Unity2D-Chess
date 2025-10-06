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
    private static BoardGrid<ChessPiece> _board;

    private void Awake()
    {
        _board = new BoardGrid<ChessPiece>(BoardConstants.FILES_COUNT, BoardConstants.RANKS_COUNT);
        InitializeBoardState();
    }

    #region Utility Methods
    public static bool IsInsideBoard(Vector2Int tile) => _board.IsInside(tile);
    public static bool IsTileEmpty(Vector2Int pos) => _board.IsInside(pos) && _board.Get(pos) == null;

    public static bool TryGetPieceByColor(Vector2Int input, TeamColor color, out ChessPiece piece)
    {
        piece = null;

        if (TryGetOccupiedPiece(input, out ChessPiece occupiedPiece))
        {
            piece = occupiedPiece.Color == color ? occupiedPiece : null;
        }

        return piece != null;
    }

    public static bool TryGetOccupiedPiece(Vector2Int pos, out ChessPiece piece)
    {
        piece = null;
        if (!_board.IsInside(pos)) return false;

        piece = _board.Get(pos);
        return piece != null;
    }

    public static void SetOccupiedPiece(ChessPiece occupiedPiece, Vector2Int pos)
    {
        if (!_board.IsInside(pos))
        {
            Debug.LogError($"Invalid position: {pos}");
            return;
        }
        _board.Set(pos, occupiedPiece);
    }
    #endregion

    // Initializes the chess board by registering all white and black pieces
    // into their respective positions in the board state array.
    private void InitializeBoardState()
    {
        RegisterPieces(_whitePieces);
        RegisterPieces(_blackPieces);
    }

    // Registers an array of chess pieces into the board state array based on their current positions.
    // Each piece is placed in the 2D array at coordinates matching its PiecePosition.
    private void RegisterPieces(ChessPiece[] pieces)
    {
        if (pieces == null) return;

        foreach (var piece in pieces)
        {
            Vector2Int pos = piece.CurrentTile;

            if (_board.IsInside(pos))
                _board.Set(pos, piece);
        }
    }
}
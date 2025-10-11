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
    private static BoardGrid<ChessPiece> _boardState;

    public ChessPiece[] WhitePieces => _whitePieces;
    public ChessPiece[] BlackPieces => _blackPieces;
    public static King WhiteKing { get; private set; }
    public static King BlackKing { get; private set; }

    private void Awake() => _boardState = new BoardGrid<ChessPiece>(BoardConstants.FILES_COUNT, BoardConstants.RANKS_COUNT);
    private void Start() => InitializeBoardState();

    #region Utility Methods
    public static bool IsInsideBoard(Vector2Int tile) => _boardState.IsInside(tile);
    public static bool IsTileEmpty(Vector2Int pos) => _boardState.IsInside(pos) && _boardState.Get(pos) == null;

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
        if (!_boardState.IsInside(pos)) return false;

        piece = _boardState.Get(pos);
        return piece != null;
    }

    private static void SetOccupiedPiece(ChessPiece piece, Vector2Int pos)
    {
        if (!_boardState.IsInside(pos))
        {
            Debug.LogError($"Invalid position: {pos}");
            return;
        }

        _boardState.Set(pos, piece);

        if (piece != null)
            piece.SetPiecePosition(pos);
    }

    public static void MovePiece(ChessPiece piece, Vector2Int newPosition)
    {
        if (piece == null) return;

        Vector2Int oldPosition = piece.CurrentTile;

        // Update the board state
        SetOccupiedPiece(piece, newPosition);
        ClearOccupiedTile(oldPosition);
    }

    private static void ClearOccupiedTile(Vector2Int oldPosition)
    {
        if (!_boardState.IsInside(oldPosition)) return;

        SetOccupiedPiece(null, oldPosition);
    }

    public static void PseudoMovePiece(ChessPiece piece, Vector2Int from, Vector2Int to, out ChessPiece capturedPiece)
    {
        capturedPiece = null;

        // Remove from current tile
        _boardState.Set(from, null);

        // Check if target has opponent piece
        if (_boardState.IsInside(to))
        {
            capturedPiece = _boardState.Get(to);
            _boardState.Set(to, piece);
        }
    }

    public static void UndoPseudoMove(ChessPiece piece, Vector2Int from, Vector2Int to, ChessPiece capturedPiece)
    {
        // Restore board state
        _boardState.Set(to, capturedPiece);
        _boardState.Set(from, piece);
    }

    public static ChessPiece GetPieceAt(Vector2Int pos)
    {
        if (!_boardState.IsInside(pos)) return null;
        return _boardState.Get(pos);
    }
    #endregion

    // Initializes the chess board by registering all white and black pieces
    // into their respective positions in the board state array.
    private void InitializeBoardState()
    {
        RegisterPieces(_blackPieces);
        RegisterPieces(_whitePieces);
    }

    // Registers an array of chess pieces into the board state array based on their current positions.
    // Each piece is placed in the 2D array at coordinates matching its PiecePosition.
    private void RegisterPieces(ChessPiece[] pieces)
    {
        if (pieces == null) return;

        foreach (ChessPiece piece in pieces)
        {
            if (!piece.gameObject.activeSelf) continue;

            Vector2Int pos = piece.CurrentTile;

            if (_boardState.IsInside(pos))
                _boardState.Set(pos, piece);

            if (piece.Type == PieceType.King)
            {
                if (piece.Color == TeamColor.White)
                    WhiteKing = (King)piece;
                else
                    BlackKing = (King)piece;
            }
        }
    }

    public Vector3 TileToWorld(Vector2Int boardPos)
    {
        return transform.position;
    }
}
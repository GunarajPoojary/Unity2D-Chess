using UnityEngine;

public interface IBoardQuery
{
    bool IsTileEmpty(Vector2Int pos);
    bool HasAlly(Vector2Int pos, TeamColor color);
    bool TryGetOpponent(Vector2Int pos, TeamColor color, out ChessPiece piece);
}

/// <summary>
/// Manages the chess board state and piece registration.
/// Maintains a 2D array representing the 8x8 chess board and tracks all piece positions.
/// </summary>
public class ChessBoard : MonoBehaviour, IBoardQuery
{
    [SerializeField] private ChessPiece[] _whitePieces;
    [SerializeField] private ChessPiece[] _blackPieces;

    private const int ROW_TILES_COUNT = 8;
    private const int COLUMN_TILES_COUNT = 8;

    // 2D array representing the 8x8 chess board grid
    // Each cell contains a reference to the ChessPiece occupying that position, or null if empty
    private readonly ChessPiece[,] _boardState = new ChessPiece[ROW_TILES_COUNT, COLUMN_TILES_COUNT];

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

            // Validate position before storing
            if (BoardUtilities.IsInsideBoard(piecePosition))
            {
                _boardState[piecePosition.x, piecePosition.y] = chessPieces[i];
            }
            else
            {
                Debug.LogError($"Attempted to register piece at invalid position: {piecePosition}");
            }
        }
    }

    public bool TryGetPlayerPiece(Vector2Int piecePosition, out ChessPiece piece)
    {
        piece = null;

        if (!BoardUtilities.IsInsideBoard(piecePosition))
            return false;

        piece = _boardState[piecePosition.x, piecePosition.y];
        return piece != null /*&& piece.Color == PlayerManager.Instance.PlayerColor*/;
    }

    public bool IsTileEmpty(Vector2Int piecePosition)
    {
        if (!BoardUtilities.IsInsideBoard(piecePosition))
            return false;

        return _boardState[piecePosition.x, piecePosition.y] == null;
    }

    public bool HasOpponent(Vector2Int piecePosition, TeamColor color)
    {
        if (!BoardUtilities.IsInsideBoard(piecePosition))
            return false;

        return !IsTileEmpty(piecePosition) && _boardState[piecePosition.x, piecePosition.y].Color != color;
    }

    public bool TryGetOpponent(Vector2Int piecePosition, TeamColor color, out ChessPiece piece)
    {
        piece = null;

        if (!BoardUtilities.IsInsideBoard(piecePosition))
            return false;

        if (!IsTileEmpty(piecePosition))
        {
            piece = _boardState[piecePosition.x, piecePosition.y];

            if (piece.Color != color)
                return true;
        }

        piece = null;
        return false;
    }

    public bool HasAlly(Vector2Int piecePosition, TeamColor color)
    {
        if (!BoardUtilities.IsInsideBoard(piecePosition))
            return false;

        return !IsTileEmpty(piecePosition) && _boardState[piecePosition.x, piecePosition.y].Color == color;
    }

    public void SetOccupiedPiece(ChessPiece occupiedPiece, Vector2Int piecePosition)
    {
        if (!BoardUtilities.IsInsideBoard(piecePosition))
        {
            Debug.LogError($"Attempted to set piece at invalid position: {piecePosition}");
            return;
        }

        _boardState[piecePosition.x, piecePosition.y] = occupiedPiece;
    }

    public bool TryCapturePiece(Vector2Int piecePosition, TeamColor color, out ChessPiece capturedPiece)
    {
        capturedPiece = null;

        if (!BoardUtilities.IsInsideBoard(piecePosition))
            return false;

        if (HasOpponent(piecePosition, color))
        {
            capturedPiece = _boardState[piecePosition.x, piecePosition.y];
            CapturePiece(piecePosition, capturedPiece);
        }

        return capturedPiece != null;
    }

    public bool TryGetOccupiedPiece(Vector2Int piecePosition, out ChessPiece occupiedPiece)
    {
        occupiedPiece = null;

        if (!BoardUtilities.IsInsideBoard(piecePosition))
            return false;

        occupiedPiece = _boardState[piecePosition.x, piecePosition.y];
        return occupiedPiece != null;
    }

    public ChessPiece GetOccupiedPiece(Vector2Int piecePosition)
    {
        if (!BoardUtilities.IsInsideBoard(piecePosition))
        {
            Debug.LogWarning($"Attempted to get piece at invalid position: {piecePosition}");
            return null;
        }

        return _boardState[piecePosition.x, piecePosition.y];
    }

    private void CapturePiece(Vector2Int piecePosition, ChessPiece chessPiece)
    {
        if (chessPiece != null)
        {
            chessPiece.gameObject.SetActive(false);
            SetOccupiedPiece(null, piecePosition);
        }
    }
}
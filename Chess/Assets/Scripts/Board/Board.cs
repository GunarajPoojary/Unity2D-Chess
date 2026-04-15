using System;
using UnityEngine;

public class Board : MonoBehaviour, IBoardService
{
    [Header("Colors")]
    [field: SerializeField] public Color LightTileColor { get; private set; } = Color.white;
    [field: SerializeField] public Color DarkTileColor { get; private set; } = Color.black;
    [SerializeField] private PieceSetSO _peiceSetSO;
    [SerializeField] private Vector3 _originPosition;
    [SerializeField] private float _tileSize;

    [SerializeField] private string _fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - - 0 1";
    private Transform _piecesParent;

    private const int BOARD_SIZE = 8;
    private BoardOrientation _orientation = BoardOrientation.LightBottom;
    private Grid<ChessPiece> _boardState = new(8, 8);
    private IFenParser _fenParser = new FenParser();

    private void Awake()
    {
        _piecesParent = new GameObject("Tiles").transform;

        _piecesParent.SetParent(transform);
        SetBoardState();
    }

    public void FlipBoard()
    {
        _orientation = _orientation == BoardOrientation.LightBottom
            ? BoardOrientation.DarkBottom
            : BoardOrientation.LightBottom;
    }

    private void SetBoardState()
    {
        Grid<PieceData> boardState = _fenParser.Parse(_fen).boardData;

        for (int x = 0; x < BOARD_SIZE; x++)
        {
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                PieceData pieceData = boardState.Get(x, y);
                _boardState.Set(x, y, pieceData == null ? null : CreatePiece(pieceData, new Vector2(x, y)));
            }
        }
    }

    public ChessPiece CreatePiece(PieceData pieceData, Vector2 pos)
    {
        Color fillColor = pieceData.Color == TeamColor.Light ? LightTileColor : DarkTileColor;
        Color outlineColor = pieceData.Color == TeamColor.Light ? DarkTileColor : LightTileColor;

        ChessPiece piece = Instantiate(_peiceSetSO.Get(pieceData.Type), pos, Quaternion.identity, _piecesParent);

        piece.pieceData = pieceData;

        piece.SetColors(fillColor, outlineColor);

        return piece;
    }

    public bool IsWithinBoard(TileData tile) => _boardState.IsInside(tile.columnIndex, tile.rowIndex);

    public bool IsTileEmptyAt(TileData tile)
    {
        ValidateTile(tile);

        return _boardState[tile.columnIndex, tile.rowIndex] == null;
    }

    public bool TryGetPieceAt(TileData tile, out ChessPiece piece)
    {
        ValidateTile(tile);

        piece = _boardState[tile.columnIndex, tile.rowIndex];

        if (piece == null)
            return false;

        return true;
    }

    public bool TryGetPieceByColor(TeamColor color, TileData tile, out ChessPiece piece)
    {
        ValidateTile(tile);

        piece = _boardState[tile.columnIndex, tile.rowIndex];

        if (piece == null)
            return false;

        if (piece.pieceData.Color != color)
            return false;

        return true;
    }

    public void SetPieceAt(TileData tile, PieceData piece)
    {
        ValidateTile(tile);

        _boardState[tile.columnIndex, tile.rowIndex] = null;// piece;
    }

    private void ValidateTile(TileData tile)
    {
        if (!IsWithinBoard(tile)) throw new ArgumentException("Tile is out of board");
    }

    public TileData GetTile(ChessPiece piece)
    {
        int x;
        int y;

        Vector3 localPos = piece.transform.position - _originPosition;

        x = Mathf.RoundToInt(localPos.x / _tileSize); // RoundToInt gives 2 if value is 1.5, for 1.2 value becomes 1.  
        y = Mathf.RoundToInt(localPos.y / _tileSize);// RoundToInt gives -2 if value is -1.5, for -1.2 value becomes -1.

        // Make sure the grid coordinate values don't exceed bounds
        x = Mathf.Clamp(x, 0, 8 - 1);
        y = Mathf.Clamp(y, 0, 8 - 1);

        return new(x, y);
    }
}
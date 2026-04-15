using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Colors")]
    [field: SerializeField] public Color LightTileColor { get; private set; } = Color.white;
    [field: SerializeField] public Color DarkTileColor { get; private set; } = Color.black;
    [SerializeField] private PieceSetSO _peiceSetSO;
    [SerializeField] private Vector3 _originPosition;
    [SerializeField] private float _tileSize = 1;

    [SerializeField] private string _fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - - 0 1";
    private Transform _piecesParent;

    private const int BOARD_SIZE = 8;
    private BoardOrientation _orientation = BoardOrientation.LightBottom;
    private BoardState _boardState;
    private IFenParser _fenParser = new FenParser();
    private Dictionary<PieceData, ChessPiece> _pieceMap = new();

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
        _boardState = new BoardState(_fenParser.Parse(_fen).boardData);

        for (int x = 0; x < BOARD_SIZE; x++)
        {
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                if (_boardState.TryGetPieceAt(new TileData(x, y), out PieceData pieceData))
                {
                    _pieceMap.Add(pieceData, CreatePiece(pieceData, new Vector2(x, y)));
                }
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

    public bool IsWithinBoard(TileData tile) => _boardState.IsWithinBoard(tile);

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
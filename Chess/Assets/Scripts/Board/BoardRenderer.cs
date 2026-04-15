using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardRenderer : MonoBehaviour
{
    [Header("Board Settings")]
    [field: SerializeField] public Vector2 TileSize { get; private set; } = Vector2.one;

    [Header("Colors")]
    [field: SerializeField] public Color LightTileColor { get; private set; } = Color.white;
    [field: SerializeField] public Color DarkTileColor { get; private set; } = Color.black;

    [Header("Board Pivot (Start Position Offset)")]
    [field: SerializeField] public Vector2 PivotPoint { get; private set; } = Vector2.zero;
    [SerializeField] private SpriteRenderer _tilePrefab;
    [SerializeField] private TextMeshPro _labelPrefab;
    private Transform _tilesParent;
    private Transform _ranksParent;
    private Transform _filesParent;
    private BoardOrientation _orientation = BoardOrientation.LightBottom;
    private const int BOARD_SIZE = 8;
    private readonly List<SpriteRenderer> _tiles = new(64);
    private readonly List<TextMeshPro> _fileLabels = new(8);
    private readonly List<TextMeshPro> _rankLabels = new(8);

    private void Awake()
    {
        _ranksParent = new GameObject("Ranks").transform;
        _filesParent = new GameObject("Files").transform;
        _tilesParent = new GameObject("Tiles").transform;

        _ranksParent.SetParent(transform);
        _filesParent.SetParent(transform);
        _tilesParent.SetParent(transform);

        GenerateBoard();
        InitializeLabels();
        UpdateLabels(_orientation);
    }

    [ContextMenu("Flip")]
    public void FlipBoard()
    {
        _orientation = _orientation == BoardOrientation.LightBottom
            ? BoardOrientation.DarkBottom
            : BoardOrientation.LightBottom;

        UpdateLabels(_orientation);
    }

    public void InitializeLabels()
    {
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            // FILE
            TextMeshPro fileLabel = Instantiate(_labelPrefab, _filesParent);
            fileLabel.alignment = TextAlignmentOptions.BottomRight;

            _fileLabels.Add(fileLabel);

            // RANK
            TextMeshPro rankLabel = Instantiate(_labelPrefab, _ranksParent);
            rankLabel.alignment = TextAlignmentOptions.TopLeft;

            _rankLabels.Add(rankLabel);
        }
    }

    private void GenerateBoard()
    {
        for (int i = 0; i < BOARD_SIZE * BOARD_SIZE; i++)
        {
            int x = i / BOARD_SIZE;
            int y = i % BOARD_SIZE;

            bool isLight = (x + y) % 2 != 0;

            DrawTile(
                isLight ? LightTileColor : DarkTileColor,
                PivotPoint + new Vector2(x, y)
            );
        }
    }

    public void DrawTile(Color color, Vector2 position)
    {
        SpriteRenderer tile = Instantiate(_tilePrefab, _tilesParent);
        tile.color = color;
        tile.transform.position = position;

        _tiles.Add(tile);
    }

    public void UpdateLabels(BoardOrientation orientation)
    {
        for (int x = 0; x < BOARD_SIZE; x++)
        {
            TextMeshPro label = _fileLabels[x];

            int fileIndex = orientation == BoardOrientation.LightBottom
                ? x
                : BOARD_SIZE - 1 - x;

            label.text = ((char)('A' + fileIndex)).ToString();

            float posX = x * TileSize.x;
            float posY = 0;

            label.transform.localPosition = new Vector3(posX, posY, 0);

            bool isLightTile = (x + 0) % 2 != 0;
            label.color = isLightTile ? DarkTileColor : LightTileColor;
        }

        for (int y = 0; y < BOARD_SIZE; y++)
        {
            TextMeshPro label = _rankLabels[y];

            int rankIndex = orientation == BoardOrientation.LightBottom
                ? y
                : BOARD_SIZE - 1 - y;

            label.text = (rankIndex + 1).ToString();

            float posX = 0;
            float posY = y * TileSize.y;

            label.transform.localPosition = new Vector3(posX, posY, 0);

            bool isLightTile = (0 + y) % 2 != 0;
            label.color = isLightTile ? DarkTileColor : LightTileColor;

            label.alignment = TextAlignmentOptions.TopLeft;

            _rankLabels.Add(label);
        }
    }
}
using UnityEngine;

public class HighlighterManager : MonoBehaviour
{
    [SerializeField] private Highlighter _highlighterPrefab;

    private const int ROW_TILES_COUNT = 8;
    private const int COLUMN_TILES_COUNT = 8;
    private readonly Highlighter[,] _highlighters = new Highlighter[ROW_TILES_COUNT, COLUMN_TILES_COUNT];

    private void Awake() => InitializeHighlighters();

    private void OnEnable()
    {
        GameEvents.OnHighlightEventRaised += Highlight;
        GameEvents.OnUnHighlightEventRaised += UnHighlight;
    }

    private void OnDisable()
    {
        GameEvents.OnHighlightEventRaised -= Highlight;
        GameEvents.OnUnHighlightEventRaised -= UnHighlight;
    }

    public void InitializeHighlighters()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Highlighter highlighter = Instantiate(_highlighterPrefab, new Vector3(col, row), Quaternion.identity, transform);

                highlighter.transform.name = $"Highlighter_C{col}_R{row}";

                _highlighters[row, col] = highlighter;
            }
        }
    }

    public void Highlight(Vector2Int worldPosition, HighlightType highlightType)
    {
        Vector2Int pos = worldPosition;
        HighlightType type = highlightType;

        Highlighter highlighter = _highlighters[pos.y, pos.x];
        highlighter.Highlight(type);
    }

    public void UnHighlight(Vector2Int worldPosition) => _highlighters[worldPosition.y, worldPosition.x].UnHighlight();
}
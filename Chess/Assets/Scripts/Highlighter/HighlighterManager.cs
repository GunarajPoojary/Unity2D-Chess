using UnityEngine;

public class HighlighterManager : MonoBehaviour
{
    [SerializeField] private Highlighter _highlighterPrefab;

    private BoardGrid<Highlighter> _highlighters;

    private void Awake()
    {
        _highlighters = new BoardGrid<Highlighter>(BoardConstants.FILES_COUNT, BoardConstants.RANKS_COUNT);
        InitializeHighlighters();
    }

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

    private void InitializeHighlighters()
    {
        for (int row = 0; row < BoardConstants.RANKS_COUNT; row++)
        {
            for (int col = 0; col < BoardConstants.FILES_COUNT; col++)
            {
                Highlighter h = Instantiate(_highlighterPrefab, new Vector3(col, row), Quaternion.identity, transform);
                h.transform.name = $"Highlighter_C{col}_R{row}";
                _highlighters.Set(new Vector2Int(col, row), h);
            }
        }
    }

    public void Highlight(Vector2Int pos, HighlightType type) =>
        _highlighters.Get(pos).Highlight(type);

    public void UnHighlight(Vector2Int pos) =>
        _highlighters.Get(pos).UnHighlight();
}
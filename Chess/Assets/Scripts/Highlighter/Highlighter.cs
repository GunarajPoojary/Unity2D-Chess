using UnityEngine;

/// <summary>
/// Controls tile highlighting using a simple finite state machine (FSM)
/// </summary>
public class Highlighter : MonoBehaviour
{
    [SerializeField] private HighlighterConfig _highlighterConfig;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        UnHighlight();
    }

    public void Highlight(HighlightType type)
    {
        switch (type)
        {
            case HighlightType.Selected:
                ApplyHighlight(_highlighterConfig.SelectionHighlightSprite);
                break;

            case HighlightType.Move:
                ApplyHighlight(_highlighterConfig.MoveHighlightSprite);
                break;

            case HighlightType.Capture:
                ApplyHighlight(_highlighterConfig.CaptureHighlightSprite);
                break;

            case HighlightType.Check:
                ApplyHighlight(_highlighterConfig.CheckHighlightSprite);
                break;
        }

        _renderer.enabled = true;
    }

    public void UnHighlight() => _renderer.enabled = false;

    private void ApplyHighlight(Sprite sprite) => _renderer.sprite = sprite;
}
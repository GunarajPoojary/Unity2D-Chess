using UnityEngine;

[CreateAssetMenu(fileName = "HighlighterConfig", menuName = "Custom/Highlighter Config")]
public class HighlighterConfig : ScriptableObject
{
    [Header("Highlighters Sprite")]
    [field: SerializeField] public Sprite SelectionHighlightSprite { get; private set; }
    [field: SerializeField] public Sprite CaptureHighlightSprite { get; private set; }
    [field: SerializeField] public Sprite MoveHighlightSprite { get; private set; }
    [field: SerializeField] public Sprite CheckHighlightSprite { get; private set; }
}
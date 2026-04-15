using UnityEngine;

[CreateAssetMenu(fileName = "PieceConfig", menuName = "Game/Piece Config")]
public class PieceConfigSO : ScriptableObject
{
    [Header("Colors")]
    [field: SerializeField] public Color LightPieceColor { get; private set; } = Color.white;
    [field: SerializeField] public Color DarkPieceColor { get; private set; } = Color.black;
}
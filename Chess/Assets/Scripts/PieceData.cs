using UnityEngine;

[System.Serializable]
public class PieceData
{
    [field: SerializeField] public PieceType Type { get; private set; }
    [field: SerializeField] public TeamColor Color { get; private set; }
    [field: SerializeField] public BoardSide Side { get; set; }
}

using UnityEngine;

/// <summary>
/// Data container class that holds the defining characteristics of a chess piece.
/// </summary>
[System.Serializable]
public class PieceData
{
    [field: SerializeField] public PieceType Type { get; private set; }
    [field: SerializeField] public TeamColor Color { get; private set; }
    [field: SerializeField] public BoardSide Side { get; set; }
}
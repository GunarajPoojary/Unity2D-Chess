using UnityEngine;

public class Rook : SlidingChessPiece
{
    protected override Vector2Int[] Directions => DirectionUtility.Orthogonals;
}
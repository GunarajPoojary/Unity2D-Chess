using UnityEngine;

public class Bishop : SlidingChessPiece
{
    protected override Vector2Int[] Directions => DirectionUtility.Diagonals;
}
using UnityEngine;

public class Queen : SlidingChessPiece
{
    protected override Vector2Int[] Directions => DirectionUtility.EightDirections;
}
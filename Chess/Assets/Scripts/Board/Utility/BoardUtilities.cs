using UnityEngine;

public static class BoardUtilities
{
    public static bool IsInsideBoard(Vector2Int tile) => tile.x < 8 && tile.x >= 0 && tile.y < 8 && tile.y >= 0;
}
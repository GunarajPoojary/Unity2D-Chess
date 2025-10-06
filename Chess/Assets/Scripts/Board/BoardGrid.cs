using UnityEngine;

public class BoardGrid<T>
{
    private readonly T[,] _grid;

    public int Width { get; }
    public int Height { get; }

    public BoardGrid(int width, int height)
    {
        Width = width;
        Height = height;
        _grid = new T[width, height];
    }

    public bool IsInside(Vector2Int pos) =>
        pos.x >= 0 && pos.x < Width &&
        pos.y >= 0 && pos.y < Height;

    public T Get(Vector2Int pos) => _grid[pos.x, pos.y];

    public void Set(Vector2Int pos, T value) => _grid[pos.x, pos.y] = value;

    public T this[int x, int y]
    {
        get => _grid[x, y];
        set => _grid[x, y] = value;
    }
}
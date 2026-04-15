/// <summary>
/// Generic Grid class that stores data in 2D array
/// </summary>
/// <typeparam name="T"></typeparam>
public class Grid<T>
{
    private readonly T[,] _grid;

    public int Width { get; }
    public int Height { get; }

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        _grid = new T[width, height];
    }

    public bool IsInside(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    public T Get(int x, int y) => _grid[x, y];

    public void Set(int x, int y, T value) => _grid[x, y] = value;

    public (int x, int y) FindIndex(T target)
    {
        for (int i = 0; i < _grid.GetLength(0); i++)
        {
            for (int j = 0; j < _grid.GetLength(1); j++)
            {
                if (Equals(_grid[i, j], target))
                {
                    return (i, j);
                }
            }
        }

        return (-1, -1);
    }

    public T this[int x, int y]
    {
        get => _grid[x, y];
        set => _grid[x, y] = value;
    }
}
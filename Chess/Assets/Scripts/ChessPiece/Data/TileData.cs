[System.Serializable]
public struct TileData
{
    public int rowIndex;
    public int columnIndex;

    private static readonly TileData _up = new(0, 1);
    private static readonly TileData _down = new(0, -1);
    private static readonly TileData _right = new(1, 0);
    private static readonly TileData _left = new(-1, 0);

    private static readonly TileData _upRight = _up + _right;
    private static readonly TileData _upLeft = _up + _left;
    private static readonly TileData _downLeft = _down + _left;
    private static readonly TileData _downRight = _down + _right;

    public static TileData Up => _up;
    public static TileData Down => _down;
    public static TileData Right => _right;
    public static TileData Left => _left;
    public static TileData UpRight => _upRight;
    public static TileData UpLeft => _upLeft;
    public static TileData DownLeft => _downLeft;
    public static TileData DownRight => _downRight;

    public TileData(int columnIndex, int rowIndex)
    {
        this.rowIndex = rowIndex;
        this.columnIndex = columnIndex;
    }

    public static TileData operator -(TileData tile) => new(-tile.columnIndex, -tile.rowIndex);

    public static TileData operator +(TileData a, TileData b)
        => new(a.columnIndex + b.columnIndex, a.rowIndex + b.rowIndex);

    public static TileData operator -(TileData a, TileData b)
        => new(a.columnIndex - b.columnIndex, a.rowIndex - b.rowIndex);

    public static TileData operator *(TileData a, TileData b)
        => new(a.columnIndex * b.columnIndex, a.rowIndex * b.rowIndex);

    public static TileData operator *(int a, TileData b)
        => new(a * b.columnIndex, a * b.rowIndex);

    public static TileData operator *(TileData a, int b)
        => new(a.columnIndex * b, a.rowIndex * b);

    public static TileData operator /(TileData a, int b)
        => new(a.columnIndex / b, a.rowIndex / b);

    public static bool operator ==(TileData lhs, TileData rhs)
        => lhs.rowIndex == rhs.rowIndex && lhs.columnIndex == rhs.columnIndex;

    public static bool operator !=(TileData lhs, TileData rhs) => !(lhs == rhs);

    public override bool Equals(object other)
    {
        if (other is TileData other2)
            return Equals(other2);
        return false;
    }

    public bool Equals(TileData other)
        => rowIndex == other.rowIndex && columnIndex == other.columnIndex;

    public override int GetHashCode()
        => (rowIndex * 73856093) ^ (columnIndex * 83492791);

    public override string ToString()
        => $"(row={rowIndex}, col={columnIndex})";
}
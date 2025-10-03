using UnityEngine;

/// <summary>
/// Represents an individual chess piece on the board.
/// </summary>
public class ChessPiece : MonoBehaviour
{
    [field: SerializeField] public PieceData PieceData { get; private set; }

    public TeamColor Color => PieceData.Color;
    public PieceType Type => PieceData.Type;
    public BoardSide Side => PieceData.Side;

    /// <summary>
    /// Gets the current board position of this piece as a 2D integer coordinate.
    /// Converts the piece's world transform position to grid coordinates.
    /// </summary>
    public Vector2Int PiecePosition => new((int)transform.position.x, (int)transform.position.y);

    /// <summary>
    /// Sets the piece's position on the board by updating its transform.
    /// Converts the 2D grid position to a 3D world position (with z=0 implied).
    /// </summary>
    /// <param name="position">The target grid position as a 2D integer coordinate.</param>
    public void SetPiecePosition(Vector2Int position) => transform.position = new Vector3(position.x, position.y);
}
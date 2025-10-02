using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    [field: SerializeField] public PieceData PieceData { get; private set; }
    public TeamColor Color => PieceData.Color;
    public PieceType Type => PieceData.Type;
    public BoardSide Side => PieceData.Side;
    public Vector2Int PiecePosition => new((int)transform.position.x, (int)transform.position.y);

    public void SetPiecePosition(Vector2Int position) => transform.position = new Vector3(position.x, position.y);
}
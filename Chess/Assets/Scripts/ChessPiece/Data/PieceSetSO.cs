using UnityEngine;

[CreateAssetMenu(fileName = "PieceSet", menuName = "Game/Piece Set")]
public class PieceSetSO : ScriptableObject
{
    [SerializeField] private ChessPiece pawn;
    [SerializeField] private ChessPiece rook;
    [SerializeField] private ChessPiece knight;
    [SerializeField] private ChessPiece bishop;
    [SerializeField] private ChessPiece queen;
    [SerializeField] private ChessPiece king;

    public ChessPiece Get(PieceType type)
    {
        return type switch
        {
            PieceType.Pawn => pawn,
            PieceType.Rook => rook,
            PieceType.Knight => knight,
            PieceType.Bishop => bishop,
            PieceType.Queen => queen,
            PieceType.King => king,
            _ => null
        };
    }
}
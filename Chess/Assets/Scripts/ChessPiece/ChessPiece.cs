using System;
using UnityEngine;

public abstract class ChessPiece : MonoBehaviour
{
    public PieceData pieceData;
    [SerializeField] private SpriteRenderer _fillRenderer;
    [SerializeField] private SpriteRenderer _outlineRenderer;

    public abstract void CalculatePossibleMoves(IBoardService board, TileData currentTile, Action<Move> onPossibleMoveFound);

    public void SetColors(Color fillColor, Color outlineColor)
    {
        _fillRenderer.color = fillColor;
        _outlineRenderer.color = outlineColor;
    }

    public void SetPosition(Vector2 pos) => transform.position = pos;
}
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Board _chessBoard;
    [SerializeField] private float _rayDistance = 100f;
    [SerializeField] private LayerMask _pieceMask;

    private ChessPiece _draggingPiece;
    private Vector3 _offset;
    private readonly List<Move> _validMoves = new List<Move>();

    private void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
            TryStartDrag();
        else if (Input.GetMouseButton(0))
            Drag();
        else if (Input.GetMouseButtonUp(0))
            Drop();
    }

    private void TryStartDrag()
    {
        Vector2 mouseWorld = GetMouseWorldPosition();
        RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero, _rayDistance, _pieceMask);

        if (hit.collider == null)
        {
            Debug.Log("[PlayerController] TryStartDrag — no collider hit.");
            return;
        }

        if (!hit.collider.TryGetComponent(out ChessPiece piece))
        {
            return;
        }

        _draggingPiece = piece;
        _offset = piece.transform.position - (Vector3)mouseWorld;

        TileData pieceTile = _chessBoard.GetTile(piece);

        _validMoves.Clear();
        piece.CalculatePossibleMoves(_chessBoard, pieceTile, move => _validMoves.Add(move));

        Debug.Log($"[PlayerController] Started dragging {piece.name} at {pieceTile}. " +
                  $"Found {_validMoves.Count} valid move(s).");

        foreach (var move in _validMoves)
        {
            Debug.Log($"[PlayerController] Valid move → {move.To}" +
                      (move.CapturedPiece != null ? $" (captures {move.CapturedPiece.name})" : ""));
        }
    }

    private void Drag()
    {
        if (_draggingPiece == null)
            return;

        Vector2 mouseWorld = GetMouseWorldPosition();
        _draggingPiece.transform.position = mouseWorld + (Vector2)_offset;
    }

    private void Drop()
    {
        if (_draggingPiece == null)
            return;

        Vector2 mouseWorld = GetMouseWorldPosition();
        RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);

        bool validDrop = false;
        TileData targetTile = default;

        // if (hit.collider != null && _chessBoard.TryGetTile(hit.collider, out targetTile))
        // {
        //     validDrop = _validMoves.Exists(m => m.To == targetTile);
        //     Debug.Log($"[PlayerController] Drop — hit tile {targetTile}. Valid: {validDrop}");
        // }
        // else
        // {
        //     Debug.Log("[PlayerController] Drop — no valid tile hit. Snapping piece back.");
        // }

        if (validDrop)
            ExecuteMove(_draggingPiece, targetTile);
        else
            SnapBack(_draggingPiece);

        // _chessBoard.SetAllColliders(true);
        _draggingPiece = null;
        _validMoves.Clear();
    }

    private void ExecuteMove(ChessPiece piece, TileData targetTile)
    {
        // Debug.Log($"[PlayerController] ExecuteMove — {piece.name} → {targetTile}");


        // if (piece is Pawn pawn)
        // {
        //     pawn.MarkAsMoved();
        //     Debug.Log($"[PlayerController] Pawn {pawn.name} marked as moved.");
        // }
    }

    private void SnapBack(ChessPiece piece)
    {
        // Debug.Log($"[PlayerController] SnapBack — returning {piece.name} to {piece.CurrentTile}");
    }

    private Vector2 GetMouseWorldPosition()
    {
        return _camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
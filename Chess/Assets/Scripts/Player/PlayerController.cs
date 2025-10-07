using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ChessBoard _board;
    [SerializeField] private MoveValidator _moveValidator;
    private Camera _mainCamera;
    private ChessPiece _selectedPiece;
    private readonly List<Vector2Int> _selectedPieceLegalMoves = new();

    private void Awake() => _mainCamera = Camera.main;
    private void Update() => HandleInput();

    private void HandleInput()
    {
        Vector3 inputPos = Vector3.zero;
        bool inputDown = false;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        inputDown = Input.GetMouseButtonDown(0);
        inputPos = Input.mousePosition;
#else
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                inputPos = touch.position;
                inputDown = touch.phase == TouchPhase.Began;
            }
#endif
        if (inputDown)
        {
            Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(inputPos);
            Vector2Int input = Vector2Int.FloorToInt(mouseWorldPos);

            OnTileClicked(input);
        }
    }

    private void OnTileClicked(Vector2Int clickedTile)
    {
        if (!ChessBoard.IsInsideBoard(clickedTile)) return;

        if (_selectedPiece == null)
            TrySelectPiece(clickedTile);
        else
            TryMoveSelectedPiece(clickedTile);
    }

    private void TrySelectPiece(Vector2Int tile)
    {
        if (ChessBoard.TryGetPieceByColor(tile, ServiceLocator.Get<IPlayerContext>().Color, out ChessPiece piece))
        {
            _selectedPiece = piece;
            GameEvents.RaiseHighlightEvent(tile, HighlightType.Selected);
            _moveValidator.GetLegalMoves(piece, OnLegalMoveFound);
        }
    }

    private void TryMoveSelectedPiece(Vector2Int targetTile)
    {
        Vector2Int previousPos = _selectedPiece.CurrentTile;

        if (_selectedPieceLegalMoves.Contains(targetTile))
            MoveExecutor.Execute(_selectedPiece, targetTile);

        DeselectPiece(previousPos);
    }

    private void DeselectPiece(Vector2Int previousPos)
    {
        foreach (Vector2Int position in _selectedPieceLegalMoves)
            GameEvents.RaiseUnHighlightEvent(position);

        GameEvents.RaiseUnHighlightEvent(previousPos);

        _selectedPieceLegalMoves.Clear();

        _selectedPiece = null;
    }

    private void OnLegalMoveFound(Vector2Int position, bool isOccupiedByOpponent)
    {
        GameEvents.RaiseHighlightEvent(new Vector2Int(position.x, position.y),
            isOccupiedByOpponent ? HighlightType.Capture : HighlightType.Move);

        _selectedPieceLegalMoves.Add(position);
    }
}
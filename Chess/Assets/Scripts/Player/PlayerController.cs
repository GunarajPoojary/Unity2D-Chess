using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera _mainCamera;
    private ChessPiece _selectedPiece;
    private readonly List<Vector2Int> _selectedPieceLegalMoves = new();
    [SerializeField] private ChessBoard _board;

    private void Awake() => _mainCamera = Camera.main;

    private void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
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

            if (!ChessBoard.IsInsideBoard(input)) return;

            if (ChessBoard.TryGetPieceByColor(input, ServiceLocator.Get<IPlayerContext>().Color, out ChessPiece piece))
            {
                // If new piece selected, unhighlight previous
                if (_selectedPiece != null && _selectedPiece != piece)
                {
                    GameEvents.RaiseUnHighlightEvent(_selectedPiece.CurrentTile);
                    ClearValidMoves();
                }

                _selectedPiece = piece;
                GameEvents.RaiseHighlightEvent(input, HighlightType.Selected);
                _selectedPiece.CalculateLegalMoves(OnLegalMoveFound);
            }
            else
            {
                if (_selectedPiece != null && _selectedPieceLegalMoves.Contains(input))
                {
                    ClearValidMoves();

                    Vector2Int previousPosition = _selectedPiece.CurrentTile;
                    _selectedPiece.SetPiecePosition(input);

                    MakeMove(previousPosition, input, _selectedPiece);
                }
            }
        }
    }

    private void ClearValidMoves()
    {
        foreach (Vector2Int position in _selectedPieceLegalMoves)
            GameEvents.RaiseUnHighlightEvent(position);

        if (_selectedPiece != null)
            GameEvents.RaiseUnHighlightEvent(_selectedPiece.CurrentTile);

        _selectedPieceLegalMoves.Clear();
    }

    private void OnLegalMoveFound(Vector2Int position, bool isOccupiedByOpponent)
    {
        GameEvents.RaiseHighlightEvent(new Vector2Int(position.x, position.y),
            isOccupiedByOpponent ? HighlightType.Capture : HighlightType.Move);

        _selectedPieceLegalMoves.Add(position);
    }

    private void MakeMove(Vector2Int from, Vector2Int to, ChessPiece piece)
    {
        // Check if target tile contains an opponent piece
        if (ChessBoard.TryGetOccupiedPiece(to, out ChessPiece capturedPiece) && capturedPiece.Color != piece.Color)
        {
            // Deactivate the captured piece
            capturedPiece.gameObject.SetActive(false);

            // Raise capture event (you'll need to add this to GameEvents)
            GameEvents.RaisePieceCapturedEvent(capturedPiece, to);
        }

        // Update board state
        ChessBoard.SetOccupiedPiece(null, from);
        ChessBoard.SetOccupiedPiece(piece, to);
    }
}
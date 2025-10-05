using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera _mainCamera;
    private ChessPiece _selectedPiece;
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

            if (!BoardUtilities.IsInsideBoard(input)) return;

            if (_board.TryGetPlayerPiece(input, out ChessPiece piece))
            {
                // If new piece selected, unhighlight previous
                if (_selectedPiece != null && _selectedPiece != piece)
                {
                    GameEvents.RaiseUnHighlightEvent(_selectedPiece.PiecePosition);
                }

                _selectedPiece = piece;
                GameEvents.RaiseHighlightEvent(input, HighlightType.Selected);
            }
        }
    }
}
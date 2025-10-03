using UnityEngine;

namespace Chess
{
    public class HighlightTest : MonoBehaviour
    {
        [SerializeField] private HighlightType _highlightType = HighlightType.Selected;
        [SerializeField] private ChessBoard _board;
        private Camera _mainCamera;

        private void Awake() => _mainCamera = Camera.main;

        private void Update()
        {
            HandleMouseInput();
        }

        private void HandleMouseInput()
        {
            Vector3 inputPos;
            bool inputDown;

            inputDown = Input.GetMouseButtonDown(0);
            inputPos = Input.mousePosition;

            if (inputDown)
            {
                Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(inputPos);
                Vector2Int input = Vector2Int.FloorToInt(mouseWorldPos);

                if (!BoardUtilities.IsInsideBoard(input)) return;

                GameEvents.RaiseHighlightEvent(input, _highlightType);
            }
        }
    }
}
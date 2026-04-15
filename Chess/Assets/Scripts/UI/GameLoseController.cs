using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameLoseController : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject losePanel;
    public Image dimOverlay;

    [Header("Buttons")]
    public Button retryButton;
    public Button mainMenuButton;
    public Button reviewButton;

    [Header("Text References")]
    public TextMeshProUGUI loserNameText;
    public TextMeshProUGUI movesCountText;
    public TextMeshProUGUI timeElapsedText;
    public TextMeshProUGUI defeatReasonText;

    [Header("Fallen King")]
    public Transform fallenKingTransform;

    [Header("Animation Settings")]
    [SerializeField] float overlayFadeDuration = 0.40f;
    [SerializeField] float cardShakeDuration = 0.45f;
    [SerializeField] float statCountDuration = 1.0f;
    [SerializeField] float buttonStaggerDelay = 0.09f;
    [SerializeField] Ease cardInEase = Ease.OutBack;

    [Header("Scene Names")]
    [SerializeField] string gameSceneName = "GameScene";
    [SerializeField] string mainMenuSceneName = "MainMenu";

    int _moves;
    float _timeSeconds;
    Sequence _entranceSeq;

    void Awake()
    {
        DOTween.Init();
    }

    void Start()
    {
        retryButton.onClick.AddListener(OnRetry);
        mainMenuButton.onClick.AddListener(OnMainMenu);
        reviewButton.onClick.AddListener(OnReview);

        HideImmediate();
    }

    [ContextMenu("Show")]
    public void ShowMenu() => Show();

    public void Show(string loserName = "Black",
                     int moves = 0,
                     float timeSeconds = 0f,
                     string defeatReason = "Checkmate — King is trapped")
    {
        _moves = moves;
        _timeSeconds = timeSeconds;

        if (loserNameText != null) loserNameText.text = loserName;
        if (defeatReasonText != null) defeatReasonText.text = defeatReason;
        if (movesCountText != null) movesCountText.text = "0";
        if (timeElapsedText != null) timeElapsedText.text = "00:00";

        losePanel.SetActive(true);
        losePanel.transform.localScale = Vector3.zero;
        if (dimOverlay) dimOverlay.color = new Color(0, 0, 0, 0);

        if (fallenKingTransform != null)
            fallenKingTransform.localRotation = Quaternion.identity;

        PlayEntrance();
    }


    void PlayEntrance()
    {
        _entranceSeq?.Kill();
        _entranceSeq = DOTween.Sequence();

        if (dimOverlay != null)
            _entranceSeq.Append(dimOverlay.DOFade(0.65f, overlayFadeDuration));

        _entranceSeq.Append(
            losePanel.transform.DOScale(Vector3.one, cardShakeDuration)
                               .SetEase(cardInEase));

        _entranceSeq.AppendCallback(() =>
        {
            losePanel.transform
                .DOShakePosition(0.35f, new Vector3(14f, 4f, 0), 18, 90f, false, true);
        });

        float afterCard = overlayFadeDuration + cardShakeDuration + 0.35f;

        if (fallenKingTransform != null)
        {
            _entranceSeq.Insert(overlayFadeDuration + 0.05f,
                fallenKingTransform.DOLocalRotate(
                    new Vector3(0, 0, -90f), 0.55f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    // Subtle bounce-back
                    fallenKingTransform
                        .DOLocalRotate(new Vector3(0, 0, 8f), 0.20f, RotateMode.LocalAxisAdd)
                        .SetLoops(2, LoopType.Yoyo)
                        .SetEase(Ease.InOutSine);
                }));
        }

        Transform reasonBg = FindDeepTransform(losePanel.transform, "DefeatReasonBg");
        if (reasonBg != null)
        {
            RectTransform rt = reasonBg.GetComponent<RectTransform>();
            Vector2 targetPos = rt.anchoredPosition;
            rt.anchoredPosition = targetPos + new Vector2(300f, 0f);
            _entranceSeq.Insert(afterCard - 0.1f,
                rt.DOAnchorPos(targetPos, 0.40f).SetEase(Ease.OutBack));
        }

        if (movesCountText != null)
        {
            int capturedMoves = _moves;
            _entranceSeq.Insert(afterCard,
                DOTween.To(() => 0, v => movesCountText.text = v.ToString(),
                           capturedMoves, statCountDuration)
                       .SetEase(Ease.OutQuad));
        }

        if (timeElapsedText != null)
        {
            float capturedTime = _timeSeconds;
            _entranceSeq.Insert(afterCard,
                DOTween.To(() => 0f, v =>
                    {
                        int m = Mathf.FloorToInt(v / 60f);
                        int s = Mathf.FloorToInt(v % 60f);
                        timeElapsedText.text = $"{m:00}:{s:00}";
                    },
                    capturedTime, statCountDuration)
                .SetEase(Ease.OutQuad));
        }

        Transform encourageTr = FindDeepTransform(losePanel.transform, "EncouragementText");
        if (encourageTr != null)
        {
            TextMeshProUGUI tmp = encourageTr.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.alpha = 0f;
                _entranceSeq.Insert(afterCard + statCountDuration * 0.6f,
                    tmp.DOFade(1f, 0.6f).SetEase(Ease.OutQuad));
            }
        }

        Button[] buttons = { retryButton, reviewButton, mainMenuButton };
        float btnStart = afterCard + statCountDuration * 0.4f;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;
            buttons[i].transform.localScale = Vector3.zero;
            _entranceSeq.Insert(btnStart + i * buttonStaggerDelay,
                buttons[i].transform.DOScale(Vector3.one, 0.28f)
                                    .SetEase(Ease.OutBack));
        }
    }


    void OnRetry()
    {
        AnimateExit(() =>
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName));
    }

    void OnMainMenu()
    {
        AnimateExit(() =>
            UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName));
    }

    void OnReview()
    {
        AnimateExit(() =>
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName));
    }


    void AnimateExit(System.Action onComplete)
    {
        _entranceSeq?.Kill();

        Sequence seq = DOTween.Sequence();
        seq.Append(losePanel.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack));
        if (dimOverlay != null)
            seq.Join(dimOverlay.DOFade(0f, 0.25f));
        seq.OnComplete(() =>
        {
            losePanel.SetActive(false);
            onComplete?.Invoke();
        });
    }

    void HideImmediate()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(false);
            losePanel.transform.localScale = Vector3.one;
        }
        if (dimOverlay != null)
            dimOverlay.color = new Color(0, 0, 0, 0.65f);
    }

    Transform FindDeepTransform(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform found = FindDeepTransform(child, name);
            if (found != null) return found;
        }
        return null;
    }
}
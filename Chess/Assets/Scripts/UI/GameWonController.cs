using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameWonController : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject wonPanel;
    public Image dimOverlay;

    [Header("Buttons")]
    public Button playAgainButton;
    public Button mainMenuButton;
    public Button nextGameButton;

    [Header("Text References")]
    public TextMeshProUGUI winnerNameText;
    public TextMeshProUGUI movesCountText;
    public TextMeshProUGUI timeElapsedText;

    [Header("Star Transforms")]
    public Transform star1;
    public Transform star2;
    public Transform star3;

    [Header("Animation Settings")]
    [SerializeField] float overlayFadeDuration = 0.35f;
    [SerializeField] float cardPopDuration = 0.45f;
    [SerializeField] float starDelay = 0.12f;
    [SerializeField] float starPopDuration = 0.55f;
    [SerializeField] float statCountDuration = 1.2f;
    [SerializeField] float buttonStaggerDelay = 0.08f;
    [SerializeField] Ease cardEase = Ease.OutBack;
    [SerializeField] Ease starEase = Ease.OutElastic;
    [SerializeField] int starCount = 3;

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
        playAgainButton?.onClick.AddListener(OnPlayAgain);
        mainMenuButton?.onClick.AddListener(OnMainMenu);
        nextGameButton?.onClick.AddListener(OnNextGame);

        HideImmediate();
    }

    [ContextMenu("Show")]
    public void ShowMenu() => Show();

    public void Show(string winnerName = "White",
                     int moves = 0,
                     float timeSeconds = 0f,
                     int stars = 3)
    {
        _moves = moves;
        _timeSeconds = timeSeconds;
        starCount = Mathf.Clamp(stars, 1, 3);

        if (winnerNameText != null) winnerNameText.text = winnerName;
        if (movesCountText != null) movesCountText.text = "0";
        if (timeElapsedText != null) timeElapsedText.text = "00:00";

        wonPanel.SetActive(true);
        wonPanel.transform.localScale = Vector3.zero;
        if (dimOverlay) dimOverlay.color = new Color(0, 0, 0, 0);

        PlayEntrance();
    }


    void PlayEntrance()
    {
        _entranceSeq?.Kill();
        _entranceSeq = DOTween.Sequence();

        if (dimOverlay != null)
            _entranceSeq.Append(dimOverlay.DOFade(0.55f, overlayFadeDuration));

        _entranceSeq.Append(
            wonPanel.transform.DOScale(Vector3.one, cardPopDuration)
                              .SetEase(cardEase));

        Transform[] stars = { star1, star2, star3 };
        for (int i = 0; i < 3; i++)
        {
            if (stars[i] == null) continue;

            bool active = i < starCount;
            float delay = i * starDelay;

            stars[i].localScale = Vector3.zero;
            var tmp = stars[i].GetComponent<TextMeshProUGUI>();

            if (active)
            {
                if (tmp != null)
                    tmp.color = new Color(0.99f, 0.85f, 0.30f, 1f);

                _entranceSeq.Insert(cardPopDuration + delay,
                    stars[i].DOScale(Vector3.one, starPopDuration)
                             .SetEase(starEase));
            }
            else
            {
                if (tmp != null)
                    tmp.color = new Color(0.6f, 0.6f, 0.62f, 1f);

                _entranceSeq.Insert(cardPopDuration + delay,
                    stars[i].DOScale(Vector3.one, starPopDuration * 0.6f)
                             .SetEase(Ease.OutQuad));
            }
        }

        float statStart = cardPopDuration + 3 * starDelay + 0.1f;

        if (movesCountText != null)
        {
            int capturedMoves = _moves;
            _entranceSeq.Insert(statStart,
                DOTween.To(() => 0, v => movesCountText.text = v.ToString(),
                           capturedMoves, statCountDuration)
                       .SetEase(Ease.OutQuad));
        }

        if (timeElapsedText != null)
        {
            float capturedTime = _timeSeconds;
            _entranceSeq.Insert(statStart,
                DOTween.To(() => 0f, v =>
                    {
                        int m = Mathf.FloorToInt(v / 60f);
                        int s = Mathf.FloorToInt(v % 60f);
                        timeElapsedText.text = $"{m:00}:{s:00}";
                    },
                    capturedTime, statCountDuration)
                .SetEase(Ease.OutQuad));
        }

        Button[] buttons = { playAgainButton, nextGameButton, mainMenuButton };
        float btnStart = statStart + statCountDuration * 0.5f;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;
            var btnRT = buttons[i].GetComponent<RectTransform>();
            buttons[i].transform.localScale = Vector3.zero;
            _entranceSeq.Insert(btnStart + i * buttonStaggerDelay,
                buttons[i].transform.DOScale(Vector3.one, 0.30f)
                                    .SetEase(Ease.OutBack));
        }

        _entranceSeq.OnComplete(() =>
        {
            if (star2 != null)
            {
                star2.DORotate(new Vector3(0, 0, 8f), 0.6f, RotateMode.LocalAxisAdd)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetEase(Ease.InOutSine);
            }
        });
    }


    void OnPlayAgain()
    {
        AnimateExit(() =>
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName));
    }

    void OnNextGame()
    {
        AnimateExit(() =>
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName));
    }

    void OnMainMenu()
    {
        AnimateExit(() =>
            UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName));
    }


    void AnimateExit(System.Action onComplete)
    {
        _entranceSeq?.Kill();
        DOTween.Kill(star2);

        var seq = DOTween.Sequence();
        seq.Append(wonPanel.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack));
        if (dimOverlay != null)
            seq.Join(dimOverlay.DOFade(0f, 0.25f));
        seq.OnComplete(() =>
        {
            wonPanel.SetActive(false);
            onComplete?.Invoke();
        });
    }

    void HideImmediate()
    {
        if (wonPanel != null)
        {
            wonPanel.SetActive(false);
            wonPanel.transform.localScale = Vector3.one;
        }
        if (dimOverlay != null)
            dimOverlay.color = new Color(0, 0, 0, 0.55f);
    }
}
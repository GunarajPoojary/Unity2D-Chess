using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PauseMenuController : MonoBehaviour
{
    [Header("References")]
    public GameObject pausePanel;
    public Image dimOverlay;

    [Header("Buttons")]
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    public Button musicToggleButton;
    public Button sfxToggleButton;

    [Header("Toggle Labels")]
    public TextMeshProUGUI musicToggleText;
    public TextMeshProUGUI sfxToggleText;

    [Header("Animation")]
    [SerializeField] float animDuration = 0.30f;
    [SerializeField] Ease inEase = Ease.OutBack;
    [SerializeField] Ease outEase = Ease.InBack;

    [Header("Scene Names")]
    [SerializeField] string mainMenuSceneName = "MainMenu";
    [SerializeField] string gameSceneName = "GameScene";

    bool _isPaused = false;
    bool _musicOn = true;
    bool _sfxOn = true;

    static readonly Color BtnGreen = new Color(0.25f, 0.78f, 0.40f, 1f);
    static readonly Color BtnRed = new Color(0.90f, 0.25f, 0.28f, 1f);

    public System.Action OnResume;
    public System.Action OnRestart;
    public System.Action OnMainMenu;


    void Awake() => DOTween.Init();

    void Start()
    {
        resumeButton?.onClick.AddListener(Close);
        restartButton?.onClick.AddListener(DoRestart);
        mainMenuButton?.onClick.AddListener(DoMainMenu);
        musicToggleButton?.onClick.AddListener(ToggleMusic);
        sfxToggleButton?.onClick.AddListener(ToggleSFX);

        // Load saved preferences
        _musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        _sfxOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;
        RefreshToggles();

        HideImmediate();
    }

    // void Update()
    // {
    //     // Escape key toggles pause
    //     if (Input.GetKeyDown(KeyCode.Escape))
    //     {
    //         if (_isPaused) Close();
    //         else Open();
    //     }
    // }

    [ContextMenu("Open")]
    public void Open()
    {
        if (_isPaused) return;
        _isPaused = true;
        Time.timeScale = 0f;

        pausePanel.SetActive(true);
        pausePanel.transform.localScale = Vector3.zero;

        if (dimOverlay != null)
        {
            dimOverlay.gameObject.SetActive(true);
            dimOverlay.color = new Color(0, 0, 0, 0);
            dimOverlay.DOFade(0.60f, animDuration).SetUpdate(true);
        }

        pausePanel.transform
            .DOScale(Vector3.one, animDuration)
            .SetEase(inEase)
            .SetUpdate(true);   // SetUpdate(true) = runs while timeScale == 0

        // Stagger buttons
        var buttons = pausePanel.GetComponentsInChildren<Button>();
        foreach (var (btn, i) in WithIndex(buttons))
        {
            btn.transform.localScale = Vector3.zero;
            btn.transform
               .DOScale(Vector3.one, 0.25f)
               .SetEase(Ease.OutBack)
               .SetDelay(i * 0.06f)
               .SetUpdate(true);
        }
    }

    public void Close()
    {
        if (!_isPaused) return;

        var seq = DOTween.Sequence().SetUpdate(true);
        seq.Append(pausePanel.transform
            .DOScale(Vector3.zero, animDuration * 0.8f)
            .SetEase(outEase).SetUpdate(true));

        if (dimOverlay != null)
            seq.Join(dimOverlay.DOFade(0f, animDuration * 0.8f).SetUpdate(true));

        seq.OnComplete(() =>
        {
            _isPaused = false;
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
            if (dimOverlay != null) dimOverlay.gameObject.SetActive(false);
            OnResume?.Invoke();
        });
    }


    void DoRestart()
    {
        Time.timeScale = 1f;
        _isPaused = false;
        OnRestart?.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    void DoMainMenu()
    {
        Time.timeScale = 1f;
        _isPaused = false;
        OnMainMenu?.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }

    void ToggleMusic()
    {
        _musicOn = !_musicOn;
        PlayerPrefs.SetInt("MusicOn", _musicOn ? 1 : 0);
        AudioManager.Instance?.SetMusicEnabled(_musicOn);
        RefreshToggleButton(musicToggleButton, musicToggleText, _musicOn);
    }

    void ToggleSFX()
    {
        _sfxOn = !_sfxOn;
        PlayerPrefs.SetInt("SFXOn", _sfxOn ? 1 : 0);
        AudioManager.Instance?.SetSFXEnabled(_sfxOn);
        RefreshToggleButton(sfxToggleButton, sfxToggleText, _sfxOn);
    }


    void RefreshToggles()
    {
        RefreshToggleButton(musicToggleButton, musicToggleText, _musicOn);
        RefreshToggleButton(sfxToggleButton, sfxToggleText, _sfxOn);
    }

    void RefreshToggleButton(Button btn, TextMeshProUGUI lbl, bool isOn)
    {
        if (btn == null) return;
        var img = btn.GetComponent<Image>();
        if (img != null) img.color = isOn ? BtnGreen : BtnRed;
        if (lbl != null) lbl.text = isOn ? "ON" : "OFF";
        btn.transform.DOPunchScale(Vector3.one * 0.15f, 0.18f, 5, 0.5f).SetUpdate(true);
    }

    void HideImmediate()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (dimOverlay != null)
        {
            dimOverlay.color = new Color(0, 0, 0, 0.6f);
            dimOverlay.gameObject.SetActive(false);
        }
    }

    // Utility: iterate with index without LINQ
    static System.Collections.Generic.IEnumerable<(T item, int index)>
        WithIndex<T>(T[] arr)
    {
        for (int i = 0; i < arr.Length; i++) yield return (arr[i], i);
    }
}

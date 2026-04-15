using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private MainPanel _mainMenuPanel;
    [SerializeField] private OptionsPanel _optionsPanel;
    [SerializeField] private CreditsPanel _creditsPanel;
    [SerializeField] private UIPopup _popupPanel;

    bool _musicOn = true;
    bool _sfxOn = true;

    private void Start()
    {
        _optionsPanel.gameObject.SetActive(false);
        _creditsPanel.gameObject.SetActive(false);

        _mainMenuPanel.gameObject.SetActive(true);
        _mainMenuPanel.Show();
    }

    private void OnEnable()
    {
        _mainMenuPanel.AddButtonClickListeners(OnPlay, OnOpenOptions, OnOpenCredits, OnQuit);
        _optionsPanel.AddButtonClickListeners(OnCloseOptions, OnApplyOptions, OnCloseOptions, OnToggleSFX, OnToggleMusic);

        _creditsPanel.AddButtonClickListeners(OnCloseCredits);
        _popupPanel.AddButtonListeners(OnQuitConfirm, OnQuitCancel);
    }

    private void OnDisable()
    {
        _mainMenuPanel.RemoveButtonClickListeners(OnPlay, OnOpenOptions, OnOpenCredits, OnQuit);
        _optionsPanel.RemoveButtonClickListeners(OnCloseOptions, OnApplyOptions, OnCloseOptions, OnToggleSFX, OnToggleMusic);
        _creditsPanel.RemoveButtonClickListeners(OnCloseCredits);

        _popupPanel.AddButtonListeners(OnQuitConfirm, OnQuitCancel);
    }

    private void OnPlay()
    {
        _mainMenuPanel.Hide(() => UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene"));
    }

    private void OnOpenOptions()
    {
        _mainMenuPanel.Hide(() =>
        {
            _optionsPanel.gameObject.SetActive(true);
            _optionsPanel.Show();
        });
    }

    private void OnOpenCredits()
    {
        _mainMenuPanel.Hide(() =>
        {
            _creditsPanel.gameObject.SetActive(true);
            _creditsPanel.Show();
        });
    }

    private void OnCloseOptions()
    {
        _optionsPanel.Hide(() =>
        {
            _optionsPanel.gameObject.SetActive(false);
            _mainMenuPanel.gameObject.SetActive(true);
            _mainMenuPanel.Show();
        });
    }

    private void OnApplyOptions()
    {
        PlayerPrefs.SetInt("MusicOn", _musicOn ? 1 : 0);
        PlayerPrefs.SetInt("SFXOn", _sfxOn ? 1 : 0);
        PlayerPrefs.Save();

        _optionsPanel.Hide(() =>
        {
            _optionsPanel.gameObject.SetActive(false);
            _mainMenuPanel.gameObject.SetActive(true);
            _mainMenuPanel.Show();
        });
    }

    private void OnCloseCredits()
    {
        _creditsPanel.Hide(() =>
        {
            _creditsPanel.gameObject.SetActive(false);
            _mainMenuPanel.gameObject.SetActive(true);
            _mainMenuPanel.Show();
        });
    }

    private void OnQuitConfirm()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }

    private void OnQuitCancel()
    {
        _popupPanel.Hide(() =>
        {
            _popupPanel.gameObject.SetActive(false);
            _mainMenuPanel.gameObject.SetActive(true);
            _mainMenuPanel.Show(() => _mainMenuPanel.gameObject.SetActive(true));
        });
    }

    private void OnQuit()
    {
        _mainMenuPanel.Hide(() =>
        {
            _mainMenuPanel.gameObject.SetActive(false);
            _popupPanel.gameObject.SetActive(true);
            _popupPanel.Show();
        });
    }

    private void OnToggleMusic()
    {
        _musicOn = !_musicOn;
        AudioManager.Instance.SetMusicEnabled(_musicOn);
    }

    private void OnToggleSFX()
    {
        _sfxOn = !_sfxOn;
        AudioManager.Instance.SetSFXEnabled(_sfxOn);
    }
}
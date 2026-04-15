using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton

    public static AudioManager Instance { get; private set; }

    #endregion

    #region Fields

    [Header("Audio Sources")]
    [Space]
    [SerializeField] protected AudioSource _musicAudioSource;
    [SerializeField] protected AudioSource _sfxAudioSource;

    [Header("Feedback")]
    [Space]
    [SerializeField] private AudioClip _sfxVolumeChangeFeedbackClip;
    [SerializeField] private float _feedbackDelay = 0.1f;

    private float _masterVolume = 1f;
    private float _musicVolume = 1f;
    private float _sfxVolume = 1f;

    private bool _isMusicEnabled = true;
    private bool _isSFXEnabled = true;

    #endregion

    #region MonoBehaviour Messages

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ApplyAllVolumes();
    }

    #endregion

    #region Public API — Playback

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        _musicAudioSource.clip = clip;

        if (_isMusicEnabled)
            _musicAudioSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || !_isSFXEnabled) return;

        PlayOneShotAudio(clip);
    }

    #endregion

    #region Public API — Mute Toggles

    public void SetMusicEnabled(bool musicOn)
    {
        _isMusicEnabled = musicOn;

        if (_isMusicEnabled)
        {
            _musicAudioSource.volume = GetEffectiveMusicVolume();
            if (!_musicAudioSource.isPlaying && _musicAudioSource.clip != null)
                _musicAudioSource.Play();
        }
        else
        {
            _musicAudioSource.volume = 0f;
            _musicAudioSource.Pause();
        }
    }

    public void SetSFXEnabled(bool sfxOn)
    {
        _isSFXEnabled = sfxOn;
        _sfxAudioSource.volume = _isSFXEnabled ? GetEffectiveSFXVolume() : 0f;
    }

    #endregion

    #region Public API — Volume Control

    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        ApplyAllVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);

        if (_isMusicEnabled)
            _musicAudioSource.volume = GetEffectiveMusicVolume();
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);

        if (_isSFXEnabled)
        {
            _sfxAudioSource.volume = GetEffectiveSFXVolume();

            if (_sfxVolumeChangeFeedbackClip != null)
            {
                _sfxAudioSource.clip = _sfxVolumeChangeFeedbackClip;
                _sfxAudioSource.PlayDelayed(_feedbackDelay);
            }
        }
    }

    #endregion

    #region Public API — State Queries

    public bool IsMusicOn() => _isMusicEnabled;
    public bool IsSFXOn() => _isSFXEnabled;

    public float GetMasterVolume() => _masterVolume;
    public float GetMusicVolume() => _musicVolume;
    public float GetSFXVolume() => _sfxVolume;

    #endregion

    #region Private Helpers

    private float GetEffectiveMusicVolume() => _musicVolume * _masterVolume;
    private float GetEffectiveSFXVolume() => _sfxVolume * _masterVolume;

    private void ApplyAllVolumes()
    {
        _musicAudioSource.volume = _isMusicEnabled ? GetEffectiveMusicVolume() : 0f;
        _sfxAudioSource.volume = _isSFXEnabled ? GetEffectiveSFXVolume() : 0f;
    }

    private void PlayOneShotAudio(AudioClip clip) =>
        _sfxAudioSource.PlayOneShot(clip, GetEffectiveSFXVolume());

    #endregion
}
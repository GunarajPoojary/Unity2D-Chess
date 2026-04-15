using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(UIButton), typeof(RectTransform))]
public class UIButtonTween : MonoBehaviour
{
    [SerializeField] private SOUIButtonTweenConfig _config;

    private RectTransform _rectTransform;
    private Vector2 _originalPos;
    private UIButton _button;
    private bool _pressed;

    private Tween _scaleTween;
    private Tween _posTween;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originalPos = _rectTransform.anchoredPosition;
        _button = GetComponent<UIButton>();
    }

    private void OnEnable()
    {
        _button.OnHoverEnter += HandleHoverEnter;
        _button.OnHoverExit += HandleHoverExit;
        _button.OnPress += HandlePress;
        _button.OnRelease += HandleRelease;
    }

    private void OnDisable()
    {
        _button.OnHoverEnter -= HandleHoverEnter;
        _button.OnHoverExit -= HandleHoverExit;
        _button.OnPress -= HandlePress;
        _button.OnRelease -= HandleRelease;
    }

    private void HandleHoverEnter()
    {
        if (_pressed) return;
        SetScale(_config.hoverScale, _config.hoverDuration, _config.hoverEase);
    }

    private void HandleHoverExit()
    {
        if (_pressed) return;
        SetScale(1f, _config.hoverDuration, _config.hoverEase);
    }

    private void HandlePress()
    {
        _pressed = true;
        SetScale(_config.pressScale, _config.pressDuration, _config.pressEase);
        SetPosY(_originalPos.y + _config.pressDownY, _config.pressDuration, _config.pressEase);
    }

    private void HandleRelease()
    {
        _pressed = false;
        SetScale(1f, _config.releaseDuration, _config.releaseEase);
        SetPosY(_originalPos.y, _config.releaseDuration, _config.releaseEase);
    }

    private void SetScale(float target, float duration, Ease ease)
    {
        _scaleTween?.Kill();
        _scaleTween = _rectTransform.DOScale(target, duration).SetEase(ease).SetUpdate(true);
    }

    private void SetPosY(float targetY, float duration, Ease ease)
    {
        _posTween?.Kill();
        _posTween = _rectTransform.DOAnchorPosY(targetY, duration).SetEase(ease).SetUpdate(true);
    }
}
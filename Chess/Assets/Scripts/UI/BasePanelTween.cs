using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(UIPanel), typeof(RectTransform), typeof(CanvasGroup))]
public class BasePanelTween : MonoBehaviour
{
    [SerializeField] protected SOPanelTweenConfig _config;
    [SerializeField] protected float _buttonStaggerDelay = 0.07f;

    protected CanvasGroup _canvasGroup;
    protected RectTransform _rect;
    protected UIPanel _panel;
    protected Vector2 _restingPos;

    protected Sequence _sequence;

    protected virtual void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _panel = GetComponent<UIPanel>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _restingPos = _rect.anchoredPosition;
    }

    protected virtual void OnEnable()
    {
        _panel.OnShow += Show;
        _panel.OnHide += Hide;
    }

    protected virtual void OnDisable()
    {
        _panel.OnShow -= Show;
        _panel.OnHide -= Hide;
    }

    protected virtual void Show(Action callback)
    {
        _sequence?.Kill();

        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        _rect.localScale = Vector3.one * _config.startScale;
        _rect.anchoredPosition = _restingPos + new Vector2(0, _config.moveOffsetY);

        UIButton[] buttons = GetComponentsInChildren<UIButton>(true);
        foreach (var btn in buttons)
            btn.transform.localScale = Vector3.zero;

        _sequence = DOTween.Sequence().SetUpdate(true);

        _sequence.Join(_canvasGroup.DOFade(1f, _config.fadeDuration));
        _sequence.Join(_rect.DOAnchorPos(_restingPos, _config.moveDuration).SetEase(_config.moveEase));
        _sequence.Join(_rect.DOScale(Vector3.one, _config.scaleDuration).SetEase(_config.scaleEase));

        float baseDelay = _config.fadeDuration * 0.5f;

        for (int i = 0; i < buttons.Length; i++)
        {
            _sequence.Insert(baseDelay + i * _buttonStaggerDelay,
                buttons[i].transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));
        }

        _sequence.OnComplete(() =>
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            callback?.Invoke();
        });
    }

    protected virtual void Hide(Action callback)
    {
        _sequence?.Kill();

        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        UIButton[] buttons = GetComponentsInChildren<UIButton>(true);

        _sequence = DOTween.Sequence().SetUpdate(true);

        for (int i = buttons.Length - 1; i >= 0; i--)
        {
            _sequence.Insert(0f,
                buttons[i].transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack));
        }

        _sequence.AppendInterval(0.15f);
        _sequence.Append(_canvasGroup.DOFade(0f, _config.hideFadeDuration));
        _sequence.Join(_rect.DOAnchorPos(_restingPos + new Vector2(0, _config.hideMoveOffsetY), _config.hideDuration).SetEase(_config.hideEase));
        _sequence.Join(_rect.DOScale(_config.startScale, _config.hideDuration).SetEase(Ease.InBack));

        _sequence.OnComplete(() => callback?.Invoke());
    }
}
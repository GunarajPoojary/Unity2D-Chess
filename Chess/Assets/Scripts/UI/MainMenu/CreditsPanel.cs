using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CreditsPanel : UIPanel
{
    [SerializeField] private UIButton _closeButton;

    public void AddButtonClickListeners(Action onCloseButtonClick) => _closeButton.OnClick += onCloseButtonClick;
    public void RemoveButtonClickListeners(Action onCloseButtonClick) => _closeButton.OnClick -= onCloseButtonClick;
}
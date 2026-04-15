using UnityEngine;
using System;

[RequireComponent(typeof(RectTransform))]
public class UIPopup : UIPanel
{
    [SerializeField] private UIButton _confirmButton;
    [SerializeField] private UIButton _cancelButton;

    public void AddButtonListeners(Action onConfirm, Action onCancel)
    {
        _confirmButton.OnClick += onConfirm;
        _cancelButton.OnClick += onCancel;
    }

    public void RemoveButtonListeners(Action onConfirm, Action onCancel)
    {
        _confirmButton.OnClick -= onConfirm;
        _cancelButton.OnClick -= onCancel;
    }
}
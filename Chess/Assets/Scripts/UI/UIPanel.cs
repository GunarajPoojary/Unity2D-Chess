using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIPanel : MonoBehaviour
{
    public event Action<Action> OnShow;
    public event Action<Action> OnHide;

    public virtual void Show(Action showActionCallback = null) => OnShow?.Invoke(showActionCallback);
    public virtual void Hide(Action hideActionCallback = null) => OnHide?.Invoke(hideActionCallback);
}
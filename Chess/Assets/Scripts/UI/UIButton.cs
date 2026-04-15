using UnityEngine;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(RectTransform))]
public class UIButton : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler
{
    [field: SerializeField] public bool IsInteractable { get; protected set; } = true;

    public event Action OnClick;
    public event Action OnHoverEnter;
    public event Action OnHoverExit;
    public event Action OnPress;
    public event Action OnRelease;

    public virtual void OnPointerEnter(PointerEventData _)
    {
        if (!IsInteractable) return;

        OnHoverEnter?.Invoke();
    }

    public virtual void OnPointerExit(PointerEventData _)
    {
        if (!IsInteractable) return;

        OnHoverExit?.Invoke();
    }

    public virtual void OnPointerDown(PointerEventData _)
    {
        if (!IsInteractable) return;

        OnPress?.Invoke();
    }

    public virtual void OnPointerUp(PointerEventData _)
    {
        if (!IsInteractable) return;

        OnRelease?.Invoke();
    }

    public virtual void OnPointerClick(PointerEventData _)
    {
        if (!IsInteractable) return;

        OnClick?.Invoke();
    }
}
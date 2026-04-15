using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "Game/Button Tween Config")]
public class SOUIButtonTweenConfig : ScriptableObject
{
    [Header("Hover")]
    public float hoverScale = 1.06f;
    public float hoverDuration = 0.14f;
    public Ease hoverEase = Ease.OutBack;

    [Header("Press")]
    public float pressDownY = -4f;
    public float pressDuration = 0.07f;
    public float pressScale = 0.95f;
    public Ease pressEase = Ease.OutQuad;

    [Header("Release")]
    public float releaseDuration = 0.14f;
    public Ease releaseEase = Ease.OutBack;
}
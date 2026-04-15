using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "UI/Panel Tween Config")]
public class SOPanelTweenConfig : ScriptableObject
{
    public float fadeDuration = 0.3f;

    public float moveDuration = 0.4f;
    public float moveOffsetY = 100f;
    public Ease moveEase = Ease.OutCubic;

    public float startScale = 0.9f;
    public float scaleDuration = 0.35f;
    public Ease scaleEase = Ease.OutBack;

    public float hideFadeDuration = 0.25f;
    public float hideMoveOffsetY = 80f;
    public float hideDuration = 0.3f;
    public Ease hideEase = Ease.InCubic;
}
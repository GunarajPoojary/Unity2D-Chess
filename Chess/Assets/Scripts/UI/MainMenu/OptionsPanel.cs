using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class OptionsPanel : UIPanel
{
    [Header("Options Buttons")]
    [SerializeField] private UIButton _closeOptionsButton;
    [SerializeField] private UIButton _applyOptionsButton;
    [SerializeField] private UIButton _cancelOptionsButton;
    [SerializeField] private UIButton _musicToggleButton;
    [SerializeField] private UIButton _sfxToggleButton;

    public void AddButtonClickListeners(
        Action closeOptionsButtonOnClick,
        Action applyOptionsButtonOnClick,
        Action cancelOptionsButtonOnClick,
        Action sfxToggleButtonOnClick,
        Action musicToggleButtonOnClick)
    {
        _closeOptionsButton.OnClick += closeOptionsButtonOnClick;
        _applyOptionsButton.OnClick += applyOptionsButtonOnClick;
        _cancelOptionsButton.OnClick += cancelOptionsButtonOnClick;
        _musicToggleButton.OnClick += musicToggleButtonOnClick;
        _sfxToggleButton.OnClick += sfxToggleButtonOnClick;
    }

    public void RemoveButtonClickListeners(
        Action closeOptionsButtonOnClick,
        Action applyOptionsButtonOnClick,
        Action cancelOptionsButtonOnClick,
        Action sfxToggleButtonOnClick,
        Action musicToggleButtonOnClick)
    {
        _closeOptionsButton.OnClick -= closeOptionsButtonOnClick;
        _applyOptionsButton.OnClick -= applyOptionsButtonOnClick;
        _cancelOptionsButton.OnClick -= cancelOptionsButtonOnClick;
        _musicToggleButton.OnClick -= musicToggleButtonOnClick;
        _sfxToggleButton.OnClick -= sfxToggleButtonOnClick;
    }
}
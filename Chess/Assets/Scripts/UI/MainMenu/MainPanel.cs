using UnityEngine;
using System;

[RequireComponent(typeof(RectTransform))]
public class MainPanel : UIPanel
{
    [Header("Main Menu Buttons")]
    [SerializeField] private UIButton _playButton;
    [SerializeField] private UIButton _settingsButton;
    [SerializeField] private UIButton _creditsButton;
    [SerializeField] private UIButton _quitButton;

    public void AddButtonClickListeners(
        Action playButtonOnClick,
        Action settingsButtonOnClick,
        Action creditsButtonOnClick,
        Action quitButtonOnClick)
    {
        _playButton.OnClick += playButtonOnClick;
        _settingsButton.OnClick += settingsButtonOnClick;
        _creditsButton.OnClick += creditsButtonOnClick;
        _quitButton.OnClick += quitButtonOnClick;
    }

    public void RemoveButtonClickListeners(
        Action playButtonOnClick,
        Action settingsButtonOnClick,
        Action creditsButtonOnClick,
        Action quitButtonOnClick)
    {
        _playButton.OnClick -= playButtonOnClick;
        _settingsButton.OnClick -= settingsButtonOnClick;
        _creditsButton.OnClick -= creditsButtonOnClick;
        _quitButton.OnClick -= quitButtonOnClick;
    }
}
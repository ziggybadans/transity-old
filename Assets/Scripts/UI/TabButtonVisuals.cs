using System;
using System.Collections;
using System.Collections.Generic;
using Nova;
using UnityEngine;

[System.Serializable]
public class TabButtonVisuals : ItemVisuals
{
    public TextBlock Label = null;
    public UIBlock2D Background = null;
    public UIBlock2D SelectedIndicator = null;

    public Color DefaultColor;
    public Color SelectedColor;
    public Color HoveredColor;
    public Color PressedColor;

    public bool IsSelected {
        get => SelectedIndicator.gameObject.activeSelf;
        set {
        Background.Color = value ? SelectedColor : DefaultColor;
        }
    }

    internal static void HandleHover(Gesture.OnHover evt, TabButtonVisuals target, int index)
    {
        //target.Background.Color = target.HoveredColor;
    }

    internal static void HandlePress(Gesture.OnPress evt, TabButtonVisuals target, int index)
    {
        target.Background.Color = target.PressedColor;
    }

    internal static void HandleRelease(Gesture.OnRelease evt, TabButtonVisuals target, int index)
    {
        //target.Background.Color = target.HoveredColor;
    }

    internal static void HandleUnhover(Gesture.OnUnhover evt, TabButtonVisuals target, int index)
    {
        //target.Background.Color = target.DefaultColor;
    }
}

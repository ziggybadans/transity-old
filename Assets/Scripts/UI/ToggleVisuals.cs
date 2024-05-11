using System;
using System.Collections;
using System.Collections.Generic;
using Nova;
using UnityEngine;

[System.Serializable]
public class ToggleVisuals : ItemVisuals
{
    public TextBlock Label = null;
    public UIBlock2D Checkbox = null;
    public UIBlock2D Checkmark = null;

    public Color DefaultColor;
    public Color HoveredColor;
    public Color PressedColor;

    public bool IsChecked {
        get => Checkmark.gameObject.activeSelf;
        set => Checkmark.gameObject.SetActive(value);
    }

    internal static void HandleHover(Gesture.OnHover evt, ToggleVisuals target)
    {
        target.Checkbox.Color = target.HoveredColor;
    }

    internal static void HandlePress(Gesture.OnPress evt, ToggleVisuals target)
    {
        target.Checkbox.Color = target.PressedColor;
    }

    internal static void HandleRelease(Gesture.OnRelease evt, ToggleVisuals target)
    {
        target.Checkbox.Color = target.HoveredColor;
    }

    internal static void HandleUnhover(Gesture.OnUnhover evt, ToggleVisuals target)
    {
        target.Checkbox.Color = target.DefaultColor;
    }
}

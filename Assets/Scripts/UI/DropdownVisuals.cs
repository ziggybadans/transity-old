using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Nova;
using UnityEngine;

[System.Serializable]
public class DropdownItemVisuals : ItemVisuals
{
    public TextBlock Label = null;
    public UIBlock2D Background = null;
    public UIBlock2D SelectedIndicator = null;
}

[System.Serializable]
public class DropdownVisuals : ItemVisuals
{
    public TextBlock Label = null;
    public TextBlock SelectedLabel = null;
    public UIBlock2D Background = null;
    public UIBlock2D ExpandedRoot = null;
    public ListView OptionsList = null;

    public Color DefaultColor;
    public Color HoveredColor;
    public Color PressedColor;

    public Color PrimaryRowColor;
    public Color SecondaryRowColor;

    private MultiOptionSetting dataSource = null;
    private bool eventHandlersRegistered = false;

    public bool IsExpanded => ExpandedRoot.gameObject.activeSelf;

    public void Expand(MultiOptionSetting dataSource) {
        this.dataSource = dataSource;

        EnsureEventHandlers();

        ExpandedRoot.gameObject.SetActive(true);
        OptionsList.SetDataSource(dataSource.Options);
        OptionsList.JumpToIndex(dataSource.SelectedIndex);
    }

    private void EnsureEventHandlers() {
        if (eventHandlersRegistered) {
            return;
        }

        eventHandlersRegistered = true;

        OptionsList.AddGestureHandler<Gesture.OnHover, DropdownItemVisuals>(HandleItemHovered);
        OptionsList.AddGestureHandler<Gesture.OnUnhover, DropdownItemVisuals>(HandleItemUnhovered);
        OptionsList.AddGestureHandler<Gesture.OnPress, DropdownItemVisuals>(HandleItemPressed);
        OptionsList.AddGestureHandler<Gesture.OnRelease, DropdownItemVisuals>(HandleItemReleased);
        OptionsList.AddGestureHandler<Gesture.OnClick, DropdownItemVisuals>(HandleItemClicked);

        OptionsList.AddDataBinder<string, DropdownItemVisuals>(BindItem);
    }

    private void BindItem(Data.OnBind<string> evt, DropdownItemVisuals target, int index)
    {
        target.Label.Text = evt.UserData;
        target.SelectedIndicator.gameObject.SetActive(index == dataSource.SelectedIndex);
        target.Background.Color = index % 2 == 0 ? PrimaryRowColor : SecondaryRowColor;
    }

    private void HandleItemClicked(Gesture.OnClick evt, DropdownItemVisuals target, int index)
    {
        dataSource.SelectedIndex = index;
        SelectedLabel.Text = dataSource.CurrentSelection;
        evt.Consume();
        Collapse();
    }

    private void HandleItemReleased(Gesture.OnRelease evt, DropdownItemVisuals target, int index)
    {
        target.Background.Color = HoveredColor;
    }

    private void HandleItemPressed(Gesture.OnPress evt, DropdownItemVisuals target, int index)
    {
        target.Background.Color = PressedColor;
    }

    private void HandleItemUnhovered(Gesture.OnUnhover evt, DropdownItemVisuals target, int index)
    {
        target.Background.Color = index % 2 == 0 ? PrimaryRowColor : SecondaryRowColor;
    }

    private void HandleItemHovered(Gesture.OnHover evt, DropdownItemVisuals target, int index)
    {
        target.Background.Color = HoveredColor;
    }

    public void Collapse()
    {
        ExpandedRoot.gameObject.SetActive(false);
    }

    internal static void HandleHover(Gesture.OnHover evt, DropdownVisuals target)
    {
        if (evt.Receiver.transform.IsChildOf(target.ExpandedRoot.transform))
        {
            return;
        }
        target.Background.Color = target.HoveredColor;
    }

    internal static void HandlePress(Gesture.OnPress evt, DropdownVisuals target)
    {
        if (evt.Receiver.transform.IsChildOf(target.ExpandedRoot.transform))
        {
            return;
        }
        target.Background.Color = target.PressedColor;
    }

    internal static void HandleRelease(Gesture.OnRelease evt, DropdownVisuals target)
    {
        if (evt.Receiver.transform.IsChildOf(target.ExpandedRoot.transform))
        {
            return;
        }
        target.Background.Color = target.HoveredColor;
    }

    internal static void HandleUnhover(Gesture.OnUnhover evt, DropdownVisuals target)
    {
        if (evt.Receiver.transform.IsChildOf(target.ExpandedRoot.transform))
        {
            return;
        }
        target.Background.Color = target.DefaultColor;
    }
}
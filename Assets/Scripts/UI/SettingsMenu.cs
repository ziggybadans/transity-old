using System;
using System.Collections;
using System.Collections.Generic;
using Nova;
using NovaSamples.UIControls;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenu : MonoBehaviour
{
    public UIBlock Root = null;

    public List<SettingsCollection> SettingsCollections = null;
    public Nova.ListView TabBar = null;

    [Header("Temporary")]
    public BoolSetting BoolSetting = new BoolSetting();
    public ItemView ToggleItemView = null;
    public FloatSetting FloatSetting = new FloatSetting();
    public ItemView SliderItemView = null;
    public MultiOptionSetting MultiOptionSetting = new MultiOptionSetting();
    public ItemView DropdownItemView = null;

    private int selectedIndex = -1;

    private void Start()
    {
        // Visual only
        Root.AddGestureHandler<Gesture.OnHover, ToggleVisuals>(ToggleVisuals.HandleHover);
        Root.AddGestureHandler<Gesture.OnUnhover, ToggleVisuals>(ToggleVisuals.HandleUnhover);
        Root.AddGestureHandler<Gesture.OnPress, ToggleVisuals>(ToggleVisuals.HandlePress);
        Root.AddGestureHandler<Gesture.OnRelease, ToggleVisuals>(ToggleVisuals.HandleRelease);

        Root.AddGestureHandler<Gesture.OnHover, DropdownVisuals>(DropdownVisuals.HandleHover);
        Root.AddGestureHandler<Gesture.OnUnhover, DropdownVisuals>(DropdownVisuals.HandleUnhover);
        Root.AddGestureHandler<Gesture.OnPress, DropdownVisuals>(DropdownVisuals.HandlePress);
        Root.AddGestureHandler<Gesture.OnRelease, DropdownVisuals>(DropdownVisuals.HandleRelease);

        // State changing
        Root.AddGestureHandler<Gesture.OnClick, ToggleVisuals>(HandleToggleClicked);
        Root.AddGestureHandler<Gesture.OnDrag, SliderVisuals>(HandleSliderDragged);
        Root.AddGestureHandler<Gesture.OnClick, DropdownVisuals>(HandleDropdownClicked);

        // Temporary
        BindToggle(BoolSetting, ToggleItemView.Visuals as ToggleVisuals);
        BindSlider(FloatSetting, SliderItemView.Visuals as SliderVisuals);
        BindDropdown(MultiOptionSetting, DropdownItemView.Visuals as DropdownVisuals);

        // Tabs
        TabBar.AddDataBinder<SettingsCollection, TabButtonVisuals>(BindTab);
        TabBar.AddGestureHandler<Gesture.OnHover, TabButtonVisuals>(TabButtonVisuals.HandleHover);
        TabBar.AddGestureHandler<Gesture.OnPress, TabButtonVisuals>(TabButtonVisuals.HandlePress);
        TabBar.AddGestureHandler<Gesture.OnUnhover, TabButtonVisuals>(TabButtonVisuals.HandleUnhover);
        TabBar.AddGestureHandler<Gesture.OnRelease, TabButtonVisuals>(TabButtonVisuals.HandleRelease);
        TabBar.AddGestureHandler<Gesture.OnClick, TabButtonVisuals>(HandleTabClicked);

        TabBar.SetDataSource(SettingsCollections);

        if (TabBar.TryGetItemView(0, out ItemView firstTab)) {
            SelectTab(firstTab.Visuals as TabButtonVisuals, 0);
        }
    }

    private void HandleTabClicked(Gesture.OnClick evt, TabButtonVisuals target, int index)
    {
        SelectTab(target, index);
    }

    private void SelectTab(TabButtonVisuals visuals, int index) {
        if (index == selectedIndex) {
            return;
        }

        if (selectedIndex >= 0 && TabBar.TryGetItemView(selectedIndex, out ItemView currentItemView)) {
            (currentItemView.Visuals as TabButtonVisuals).IsSelected = false;
        }

        selectedIndex = index;
        visuals.IsSelected = true;
    }

    private void BindTab(Data.OnBind<SettingsCollection> evt, TabButtonVisuals target, int index)
    {
        target.Label.Text = evt.UserData.Category;
        target.IsSelected = false;
    }

    private void HandleDropdownClicked(Gesture.OnClick evt, DropdownVisuals target)
    {
        if (target.IsExpanded) {
            target.Collapse();
        } else {
            target.Expand(MultiOptionSetting);
        }
    }

    private void HandleToggleClicked(Gesture.OnClick evt, ToggleVisuals target)
    {
        BoolSetting.State = !BoolSetting.State;
        target.IsChecked = BoolSetting.State;
    }

    private void HandleSliderDragged(Gesture.OnDrag evt, SliderVisuals target)
    {
        Vector3 currentPointerPos = evt.PointerPositions.Current;

        float localXPos = target.SliderBackground.transform.InverseTransformPoint(currentPointerPos).x;
        float sliderWidth = target.SliderBackground.CalculatedSize.X.Value;

        float distanceFromLeft = localXPos + .5f * sliderWidth;
        float percentFromLeft = Mathf.Clamp01(distanceFromLeft / sliderWidth);

        FloatSetting.Value = FloatSetting.Min + percentFromLeft * (FloatSetting.Max - FloatSetting.Min);

        target.FillBar.Size.X.Percent = percentFromLeft;
        target.ValueLabel.Text = FloatSetting.DisplayValue;
    }

    private void BindToggle(BoolSetting boolSetting, ToggleVisuals visuals)
    {
        visuals.Label.Text = boolSetting.Name;
        visuals.IsChecked = boolSetting.State;
    }

    private void BindSlider(FloatSetting floatSetting, SliderVisuals visuals)
    {
        visuals.Label.Text = floatSetting.Name;
        visuals.ValueLabel.Text = floatSetting.DisplayValue;
        visuals.FillBar.Size.X.Percent = (floatSetting.Value - floatSetting.Min) / (floatSetting.Max - floatSetting.Min);
    }

    private void BindDropdown(MultiOptionSetting multiOptionSetting, DropdownVisuals visuals)
    {
        visuals.Label.Text = multiOptionSetting.Name;
        visuals.SelectedLabel.Text = multiOptionSetting.CurrentSelection;
        visuals.Collapse();
    }
}

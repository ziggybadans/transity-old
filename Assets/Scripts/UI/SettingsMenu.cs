using System;
using System.Collections;
using System.Collections.Generic;
using Nova;
using NovaSamples.UIControls;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenu : MonoBehaviour
{
    [SerializeReference]
    private UIBlock Root = null;

    [System.Serializable]
    private class SliderSettings
    {
        public FloatSetting Setting = new();
        public ItemView SliderItemView = null;
    }

    [SerializeField]
    private List<SliderSettings> sliders = new();

    private void Start()
    {
        // State changing
        Root.AddGestureHandler<Gesture.OnDrag, SliderVisuals>(HandleSliderDragged);

        foreach (var settings in sliders)
        {
            // Temporary
            BindSlider(settings.Setting, settings.SliderItemView.Visuals as SliderVisuals);
        }
    }

    private void SaveSettings() {
        foreach (var settings in sliders) {
            SettingsManager.Instance.SetMapGenValue(settings.Setting.settingType, settings.Setting.Value);
        }
    }

    private void HandleSliderDragged(Gesture.OnDrag evt, SliderVisuals target)
    {
        SliderSettings settings = sliders.Find(s => s.SliderItemView.Visuals == target);
        if (settings != null)
        {
            Vector3 currentPointerPos = evt.PointerPositions.Current;

            float localXPos = target.SliderBackground.transform.InverseTransformPoint(currentPointerPos).x;
            float sliderWidth = target.SliderBackground.CalculatedSize.X.Value;

            float distanceFromLeft = localXPos + .5f * sliderWidth;
            float percentFromLeft = Mathf.Clamp01(distanceFromLeft / sliderWidth);

            settings.Setting.Value = settings.Setting.Min + percentFromLeft * (settings.Setting.Max - settings.Setting.Min);

            target.FillBar.Size.X.Percent = percentFromLeft;
            target.ValueLabel.Text = settings.Setting.DisplayValue;
        }
    }

    private void BindSlider(FloatSetting floatSetting, SliderVisuals visuals)
    {
        visuals.Label.Text = floatSetting.Name;
        visuals.ValueLabel.Text = floatSetting.DisplayValue;
        visuals.FillBar.Size.X.Percent = (floatSetting.Value - floatSetting.Min) / (floatSetting.Max - floatSetting.Min);
    }
}

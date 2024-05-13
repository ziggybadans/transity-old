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
    private UIBlock _rootUI = null;

    [System.Serializable]
    private class SliderSettings
    {
        public FloatSetting _sliderSetting = new();
        public ItemView _sliderItem = null;
    }

    [SerializeField]
    private List<SliderSettings> _sliders = new();

    private void Start()
    {
        // State changing
        _rootUI.AddGestureHandler<Gesture.OnDrag, SliderVisuals>(HandleSliderDragged);

        foreach (var settings in _sliders)
        {
            // Temporary
            BindSlider(settings._sliderSetting, settings._sliderItem.Visuals as SliderVisuals);
        }
    }

    public void SaveSettings() {
        foreach (var settings in _sliders) {
            SettingsManager.Instance.SetMapGenValue(settings._sliderSetting.settingType, settings._sliderSetting.Value);
        }
    }

    private void HandleSliderDragged(Gesture.OnDrag evt, SliderVisuals target)
    {
        SliderSettings settings = _sliders.Find(s => s._sliderItem.Visuals == target);
        if (settings != null)
        {
            Vector3 currentPointerPos = evt.PointerPositions.Current;

            float localXPos = target.sliderBG.transform.InverseTransformPoint(currentPointerPos).x;
            float sliderWidth = target.sliderBG.CalculatedSize.X.Value;

            float distanceFromLeft = localXPos + .5f * sliderWidth;
            float percentFromLeft = Mathf.Clamp01(distanceFromLeft / sliderWidth);

            settings._sliderSetting.Value = settings._sliderSetting.min + percentFromLeft * (settings._sliderSetting.max - settings._sliderSetting.min);

            target.fillBar.Size.X.Percent = percentFromLeft;
            target.valueLabel.Text = settings._sliderSetting.DisplayValue;
        }
    }

    private void BindSlider(FloatSetting floatSetting, SliderVisuals visuals)
    {
        visuals.label.Text = floatSetting.name;
        visuals.valueLabel.Text = floatSetting.DisplayValue;
        visuals.fillBar.Size.X.Percent = (floatSetting.Value - floatSetting.min) / (floatSetting.max - floatSetting.min);
    }
}

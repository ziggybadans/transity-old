using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Setting {
    public string Name;
}

[System.Serializable]
public class BoolSetting : Setting
{
    public bool State;
}

[System.Serializable]
public class FloatSetting : Setting {
    [SerializeField]
    private float value;
    public float Min;
    public float Max;
    public string ValueFormat = "{0:0.0}";

    public float Value {
        get => Mathf.Clamp(value, Min, Max);
        set => this.value = Mathf.Clamp(value, Min, Max);
    }

    public string DisplayValue => string.Format(ValueFormat, Value);
}

[System.Serializable]
public class MultiOptionSetting : Setting {
    private const string NothingSelected = "None";

    public string[] Options = new string[0];
    public int SelectedIndex = 0;

    public string CurrentSelection => SelectedIndex >= 0 && SelectedIndex < Options.Length ? Options[SelectedIndex] : NothingSelected;
}
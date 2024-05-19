using UnityEngine;

[System.Serializable]
public abstract class Setting {
    public string name;
}

[System.Serializable]
public class FloatSetting : Setting {
    [SerializeField]
    private float value;
    public float min;
    public float max;
    public string valueFormat = "{0:0.0}";
    public SettingsTypes settingType;

    public float Value {
        get => Mathf.Clamp(value, min, max);
        set => this.value = Mathf.Clamp(value, min, max);
    }

    public string DisplayValue => string.Format(valueFormat, Value);
}
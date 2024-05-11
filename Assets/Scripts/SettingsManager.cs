using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SettingsTypes {
    NumCities, NumTowns, NumRurals
}

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager SettingsInstance { get; private set; }

    private Dictionary<SettingsTypes, int> _mapGenValues = new();

    private void Awake() {
        if (SettingsInstance == null) {
            SettingsInstance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        _mapGenValues.Add(SettingsTypes.NumCities, 0);
        _mapGenValues.Add(SettingsTypes.NumTowns, 0);
        _mapGenValues.Add(SettingsTypes.NumRurals, 0);
    }

    public void SetMapGenValue(SettingsTypes settingName, float settingValue) {
        _mapGenValues[settingName] = (int)settingValue;
        Debug.Log("Updated setting " + settingName.ToString() + " to " + settingValue);
    }

    public int GetMapGenValue(SettingsTypes settingName) {
        if (_mapGenValues.TryGetValue(settingName, out int value)) {
            return value;
        } else {
            return 0;
        }
    }
}

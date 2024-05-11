using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SettingsTypes {
    numCities, numTowns, numRurals
}

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    private Dictionary<SettingsTypes, float> mapGenValues = new();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        mapGenValues.Add(SettingsTypes.numCities, 0);
        mapGenValues.Add(SettingsTypes.numTowns, 0);
        mapGenValues.Add(SettingsTypes.numRurals, 0);
    }

    public void SetMapGenValue(SettingsTypes settingName, float settingValue) {
        mapGenValues[settingName] = settingValue;
        Debug.Log("Updated setting " + settingName.ToString() + " to " + settingValue);
    }

    public float GetMapGenValue(SettingsTypes settingName) {
        if (mapGenValues.TryGetValue(settingName, out float value)) {
            return value;
        } else {
            return 0;
        }
    }
}

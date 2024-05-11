using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Collection")]
public class SettingsCollection : ScriptableObject
{
    public string Category = null;

    [SerializeReference]
    [SerializeField]
    public List<Setting> Settings = new List<Setting>();
}

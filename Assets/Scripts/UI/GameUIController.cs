using System.Collections;
using System.Collections.Generic;
using Nova;
using NovaSamples.UIControls;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeReference]
    private UnityEngine.UI.Button toolboxButton;
    [SerializeField]
    private Sprite openSprite, closeSprite;
    private bool open = false;

    private void OnEnable() {
        toolboxButton.onClick.AddListener(GameManager.Instance.UpdateGameStateCreate);
        toolboxButton.onClick.AddListener(SwitchIcon);
    }

    private void SwitchIcon() {
        if (open) {
            toolboxButton.gameObject.GetComponent<Image>().sprite = openSprite;
            open = false;
        }
        else
        {
            toolboxButton.gameObject.GetComponent<Image>().sprite = closeSprite;
            open = true;
        }
    }
}

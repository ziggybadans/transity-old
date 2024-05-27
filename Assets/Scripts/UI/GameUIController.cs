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
    [SerializeReference]
    private UnityEngine.UI.Button lineButton;
    [SerializeField]
    private Sprite toolboxSprite, lineSprite, closeSprite;
    private bool toolboxOpen, lineOpen = false;

    private void OnEnable()
    {
        toolboxButton.onClick.AddListener(GameManager.Instance.UpdateGameStateConnection);
        toolboxButton.onClick.AddListener(SwitchIconToolbox);

        lineButton.onClick.AddListener(GameManager.Instance.UpdateGameStateLine);
        lineButton.onClick.AddListener(SwitchIconLine);
    }

    private void SwitchIconToolbox()
    {
        if (!toolboxOpen)
        {
            toolboxButton.gameObject.GetComponent<Image>().sprite = closeSprite;
            toolboxOpen = true;
            lineButton.onClick.RemoveAllListeners();
        }
        else
        {
            toolboxButton.gameObject.GetComponent<Image>().sprite = toolboxSprite;
            toolboxOpen = false;
            lineButton.onClick.AddListener(GameManager.Instance.UpdateGameStateLine);
            lineButton.onClick.AddListener(SwitchIconLine);
        }
    }

    private void SwitchIconLine()
    {
        if (!lineOpen)
        {
            lineButton.gameObject.GetComponent<Image>().sprite = closeSprite;
            lineOpen = true;
            toolboxButton.onClick.RemoveAllListeners();
        }
        else
        {
            lineButton.gameObject.GetComponent<Image>().sprite = lineSprite;
            lineOpen = false;
            toolboxButton.onClick.AddListener(GameManager.Instance.UpdateGameStateConnection);
            toolboxButton.onClick.AddListener(SwitchIconToolbox);
        }
    }
}

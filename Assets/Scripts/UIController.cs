using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    [SerializeField] private Button playButton;

    private void Start() {
        if (playButton != null) {
            playButton.onClick.AddListener(OnPlayClicked);
        }
    }

    void OnPlayClicked() {
        GameManager.instance.LoadGameplayScene("SampleScene");
    }
}

using System.Collections;
using System.Collections.Generic;
using Nova;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeReference]
    private static UIController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<UIBlock2D> views = new();
    public UIBlock2D mainMenu;

    private void Start()
    {
        foreach (UIBlock2D view in views)
        {
            if (view != mainMenu) {
                view.gameObject.SetActive(false);
            } else {
                view.gameObject.SetActive(true);
            }
        }
    }

    public void SwitchView(UIBlock2D newView)
    {
        foreach (UIBlock2D view in views)
        {
            view.gameObject.SetActive(false);
        }
        newView.gameObject.SetActive(true);
    }
}

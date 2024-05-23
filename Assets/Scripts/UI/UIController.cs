using System.Collections.Generic;
using Nova;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private List<UIBlock2D> _views = new();
    [SerializeField]
    private UIBlock2D _mainMenu;

    private void Start()
    {
        foreach (UIBlock2D view in _views)
        {
            if (view != _mainMenu) {
                view.gameObject.SetActive(false);
            } else {
                view.gameObject.SetActive(true);
            }
        }
    }

    private void SwitchView(UIBlock2D newView)
    {
        foreach (UIBlock2D view in _views)
        {
            view.gameObject.SetActive(false);
        }
        newView.gameObject.SetActive(true);
    }
}

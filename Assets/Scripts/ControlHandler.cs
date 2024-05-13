using System;
using UnityEngine;

public class ControlHandler : MonoBehaviour
{
    public static ControlHandler Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private static event Action UpdateProbabilities;
    public static event Action DrawConnection, MaintainConnection, CreateConnection, DeleteConnection;
    private bool drawing;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DrawConnection?.Invoke();
            drawing = true;
        }
        if (Input.GetMouseButton(0) && drawing)
        {
            MaintainConnection?.Invoke();
        }
        if (Input.GetMouseButtonUp(0) && drawing)
        {
            CreateConnection?.Invoke();
            drawing = false;
        }
        if (Input.GetMouseButtonDown(1) && !drawing) DeleteConnection?.Invoke();

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            SetDebugMode();
            UpdateProbabilities?.Invoke();
        }
    }

    private void SetDebugMode()
    {
        GameManager GameInstance = GameManager.Instance;
        if (GameInstance.DebugMode == 3) GameInstance.DebugMode = 0;
        else GameInstance.DebugMode++;
    }
}

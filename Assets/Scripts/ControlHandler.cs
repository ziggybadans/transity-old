using System;
using UnityEngine;

public class ControlHandler : MonoBehaviour
{
    public static ControlHandler Instance;
    [SerializeField]
    private bool debug;

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

    public static event Action CreateConnection, MaintainConnection, FinishConnection, DeleteConnection;
    private bool drawing = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateConnection?.Invoke();
            drawing = true;
        }
        if (Input.GetMouseButton(0) && drawing)
        {
            MaintainConnection?.Invoke();
        }
        if (Input.GetMouseButtonUp(0) && drawing)
        {
            FinishConnection?.Invoke();
            drawing = false;
        }
        if (Input.GetMouseButtonDown(1) && !drawing) DeleteConnection?.Invoke();

        if (Input.GetKeyDown(KeyCode.M))
        {
            SetDebugMode();
        }
    }

    private void SetDebugMode()
    {
        GameManager GameInstance = GameManager.Instance;
        if (GameInstance.DebugMode == 3) GameInstance.SetDebugMode(0);
        else GameInstance.SetDebugMode(GameInstance.DebugMode + 1);
    }
}

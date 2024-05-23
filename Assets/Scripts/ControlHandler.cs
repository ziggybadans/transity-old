using System;
using UnityEngine;
using UnityEngine.AI;

public class ControlHandler : MonoBehaviour
{
    public static ControlHandler Instance;
    [SerializeField]
    public bool drawing;

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

    public static event Action CreateConnection, FinishConnection, CancelConnection, DeleteConnection;

    private void Update()
    {
        if (GameManager.Instance.state == GameState.Create)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!drawing) CreateConnection?.Invoke();
                    else FinishConnection?.Invoke();
            }
            if (Input.GetMouseButtonDown(1)) {
                if (drawing) CancelConnection?.Invoke();
            }
            //if (Input.GetMouseButtonDown(1)) DeleteConnection?.Invoke();
        }

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

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
    public static event Action CreateLine, FinishLine, CancelLine, DeleteLine;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GameManager.Instance.state == GameState.Connection)
            {
                if (!drawing) CreateConnection?.Invoke();
                else FinishConnection?.Invoke();
            }
            else if (GameManager.Instance.state == GameState.Line)
            {
                if (!drawing) CreateLine?.Invoke();
                else FinishLine?.Invoke();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (drawing)
            {
                switch (GameManager.Instance.state)
                {
                    case GameState.Connection:
                        CancelConnection?.Invoke();
                        break;
                    case GameState.Line:
                        CancelLine?.Invoke();
                        break;
                }
            }
            else
            {
                switch (GameManager.Instance.state)
                {
                    case GameState.Connection:
                        DeleteConnection?.Invoke();
                        break;
                    case GameState.Line:
                        DeleteLine?.Invoke();
                        break;
                }
            }
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

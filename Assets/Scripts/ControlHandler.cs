using UnityEngine;

public class ControlHandler : MonoBehaviour
{
    [SerializeReference]
    private MapGenerator mapGenerator;
    private ConnectionHandler connectionHandler;
    private bool drawing;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            connectionHandler.DrawConnection();
            drawing = true;
        }
        if (Input.GetMouseButton(0) && drawing)
        {
            connectionHandler.MaintainConnection();
        }
        if (Input.GetMouseButtonUp(0) && drawing)
        {
            connectionHandler.CreateConnection();
            drawing = false;
        }
        if (Input.GetMouseButtonDown(1) && !drawing) connectionHandler.DeleteConnection();

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            SetDebugMode();
            mapGenerator.UpdateProbabilities();
        }
    }

    private void SetDebugMode()
    {
        GameManager GameInstance = GameManager.Instance;
        if (GameInstance.DebugMode == 3) GameInstance.DebugMode = 0;
        else GameInstance.DebugMode++;
    }
}

using Unity.VisualScripting;
using UnityEngine;

public class ConnectionHandler : MonoBehaviour
{
    public static ConnectionHandler Instance;

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

        ControlHandler.CreateConnection += CreateConnection;
    }

    [SerializeField]
    private float LINE_WIDTH = 0.5f;
    private GameObject currentConnectionObject;
    private LineRenderer currentConnectionObjectLr;
    private Settlement lastChangedSettlement;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void SetMesh(LineRenderer lr)
    {
        MeshCollider connectionMesh = currentConnectionObject.AddComponent<MeshCollider>();
        Mesh mesh = new();
        lr.BakeMesh(mesh, cam, true);
        connectionMesh.sharedMesh = mesh;
    }

    private Vector3 GetCorrectZ(Vector3 pos)
    {
        float x = pos.x;
        float y = pos.y;

        return new Vector3(x, y, -10f);
    }

    public void CreateConnection()
    {
        RaycastHit2D clickPosHit = Raycast();
        if (clickPosHit.collider != null && clickPosHit.collider.CompareTag("Settlement"))
        {
            currentConnectionObject = new GameObject("Connection");
            currentConnectionObjectLr = currentConnectionObject.AddComponent<LineRenderer>();
            currentConnectionObject.AddComponent<Connection>();

            currentConnectionObjectLr.positionCount = 2;
            currentConnectionObjectLr.SetPosition(0, GetCorrectZ(clickPosHit.collider.transform.position));
            currentConnectionObjectLr.SetPosition(1, GetCorrectZ(clickPosHit.transform.position));
            currentConnectionObjectLr.startWidth = LINE_WIDTH;
            currentConnectionObjectLr.endWidth = LINE_WIDTH;
            currentConnectionObjectLr.material = GameManager.Instance.ConnectionMaterial;

            currentConnectionObject.tag = "Connection";

            ControlHandler.MaintainConnection += MaintainConnection;
            ControlHandler.FinishConnection += FinishConnection;
            ControlHandler.DeleteConnection -= DeleteConnection;
        }
    }

    public void MaintainConnection()
    {
        RaycastHit2D dragPosHit = Raycast();
        if (dragPosHit.collider != null && dragPosHit.collider.CompareTag("Settlement") && lastChangedSettlement == null)
        {
            Debug.Log("Hit settlement!");
            if (currentConnectionObject.GetComponent<Connection>().ContainsStop(dragPosHit.collider.GetComponent<Settlement>()))
            {
                Debug.Log("Settlement already in connection! Removing...");
                if (currentConnectionObjectLr.GetPosition(currentConnectionObjectLr.positionCount - 2) == dragPosHit.collider.transform.position)
                {
                    currentConnectionObject.GetComponent<Connection>().RemoveStop(dragPosHit.collider.GetComponent<Settlement>());
                    currentConnectionObjectLr.positionCount--;
                }
                else if (dragPosHit.collider.transform.position == currentConnectionObjectLr.GetPosition(0))
                {
                    currentConnectionObject.GetComponent<Connection>().LOOP = true;
                    FinishLoop();
                }
            }
            else
            {
                Debug.Log("Settlement not yet in connection! Adding...");
                currentConnectionObject.GetComponent<Connection>().AddStop(dragPosHit.collider.GetComponent<Settlement>());
                currentConnectionObjectLr.SetPosition(currentConnectionObjectLr.positionCount - 1, dragPosHit.collider.transform.position);
                currentConnectionObjectLr.positionCount++;
            }
            lastChangedSettlement = dragPosHit.collider.GetComponent<Settlement>();
        }
        else
        {
            Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
            currentConnectionObjectLr.SetPosition(currentConnectionObjectLr.positionCount - 1, GetCorrectZ(currentPos));

            if (lastChangedSettlement != null)
            {
                if (Mathf.Abs(currentPos.x) - Mathf.Abs(lastChangedSettlement.transform.position.x) > GridManager.Instance.GRID_CELL_SIZE
                    || Mathf.Abs(currentPos.y) - Mathf.Abs(lastChangedSettlement.transform.position.y) > GridManager.Instance.GRID_CELL_SIZE)
                {
                    lastChangedSettlement = null;
                }
            }
        }
    }

    public void FinishConnection()
    {
        ControlHandler.MaintainConnection -= MaintainConnection;
        ControlHandler.FinishConnection -= FinishConnection;

        if (currentConnectionObjectLr.positionCount >= 2)
        {
            currentConnectionObjectLr.positionCount--;
            SetMesh(currentConnectionObjectLr);
            //currentConnectionObject.AddComponent<TransportSpawning>();
        }
        else Destroy(currentConnectionObject);

        ControlHandler.DeleteConnection += DeleteConnection;
    }

    public void FinishLoop() {
        ControlHandler.MaintainConnection -= MaintainConnection;
        ControlHandler.FinishConnection -= FinishConnection;

        currentConnectionObjectLr.positionCount--;
        SetMesh(currentConnectionObjectLr);
        //currentConnectionObject.AddComponent<TransportSpawning>();

        ControlHandler.DeleteConnection += DeleteConnection;
    }

    public void DeleteConnection()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Connection"))
            {
                Connection selectedConnection = hit.collider.GetComponent<Connection>();
                selectedConnection.DestroyAllEntities();
                Destroy(hit.collider.gameObject);
            }
        }
    }

    private RaycastHit2D Raycast()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        return hit;
    }
}

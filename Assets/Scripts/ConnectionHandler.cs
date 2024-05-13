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
        ControlHandler.DrawConnection += DrawConnection;
        ControlHandler.MaintainConnection += MaintainConnection;
        ControlHandler.DeleteConnection += DeleteConnection;
    }

    [SerializeField]
    private float LINE_WIDTH = 0.5f;
    private GameObject currentConnectionObject;
    private LineRenderer currentConnectionObjectLr;
    private Vector2 startPos, endPos;
    private Settlement startTown, endTown;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void SetupConnectionVisuals(LineRenderer lr, Vector3 startPos, Vector3 endPos)
    {
        ConfigureLineRenderer(lr, startPos, endPos);
        SetMesh(lr);
    }

    private void ConfigureLineRenderer(LineRenderer lr, Vector3 startPos, Vector3 endPos)
    {
        lr.positionCount = 2;
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
        lr.startWidth = LINE_WIDTH;
        lr.endWidth = LINE_WIDTH;
        lr.material = GameManager.Instance.ConnectionMaterial;
    }

    private void SetMesh(LineRenderer lr)
    {
        MeshCollider connectionMesh = currentConnectionObject.AddComponent<MeshCollider>();
        Mesh mesh = new();
        lr.BakeMesh(mesh, cam, true);
        connectionMesh.sharedMesh = mesh;
    }

    public void DrawConnection()
    {
        RaycastHit2D clickPosHit = Raycast();
        if (clickPosHit.collider != null && clickPosHit.collider.CompareTag("Settlement"))
        {
            currentConnectionObject = new GameObject("Connection");
            currentConnectionObjectLr = currentConnectionObject.AddComponent<LineRenderer>();

            startPos = clickPosHit.collider.transform.position;
            startTown = clickPosHit.collider.GetComponent<Settlement>();

            ConfigureLineRenderer(currentConnectionObjectLr, startPos, startPos);
        }
    }

    public void MaintainConnection()
    {
        Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
        currentPos.z = 0f;
        currentConnectionObjectLr.SetPosition(1, currentPos);
    }

    public void CreateConnection()
    {
        RaycastHit2D liftPosHit = Raycast();
        if (liftPosHit.collider != null && liftPosHit.collider.CompareTag("Settlement"))
        {
            endPos = liftPosHit.collider.transform.position;
            endTown = liftPosHit.collider.GetComponent<Settlement>();

            if (startTown != endTown)
            {
                currentConnectionObjectLr.SetPosition(1, endPos);

                Connection currentConnection = currentConnectionObject.AddComponent<Connection>();
                currentConnection.SetupConnection(startTown, endTown);
                currentConnectionObject.tag = "Connection";

                SetupConnectionVisuals(currentConnectionObjectLr, startPos, endPos);
            }
            else Destroy(currentConnectionObject);
        }
        else Destroy(currentConnectionObject);
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

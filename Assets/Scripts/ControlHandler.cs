using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControlHandler : MonoBehaviour
{
    [Range(1, 4)]
    public int numEntites = 2;
    [Range(0.5f, 4)]
    public float entitySpeed = 1;
    [Range(1, 6)]
    public int capacity = 6;
    public Material lineMaterial;
    public GameObject entityPrefab;
    public MapGenerator mapGenerator;

    private Camera cam;
    private int debugMode = 0;
    private float lineWidth = 0.5f;
    private bool drawing;
    private GameObject currentConnectionObject;
    private LineRenderer currentConnectionObjectLr;
    private Vector2 startPos, endPos;
    private Settlement startTown, endTown;
    private List<Connection> connections = new();

    public int GetDebugMode() { return debugMode; }

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DrawConnection();
        }
        if (drawing)
        {
            Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
            currentPos.z = 0f;
            currentConnectionObjectLr.SetPosition(1, currentPos);
        }
        if (Input.GetMouseButtonUp(0) && drawing)
        {
            drawing = false;
            CreateConnection();
        }
        if (Input.GetMouseButtonDown(1) && !drawing)
        {
            DeleteConnection();
        }

        if (Input.GetKeyDown("`"))
        {
            if (debugMode == 3)
            {
                debugMode = 0;
            }
            else
            {
                debugMode++;
            }
            mapGenerator.UpdateProbabilities();
        }
    }

    private void ConfigureLineRenderer(LineRenderer lr, Vector3 startPos, Vector3 endPos)
    {
        lr.positionCount = 2;
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = lineMaterial;
    }

    private void SetupConnectionVisuals(LineRenderer lr, Vector3 startPos, Vector3 endPos)
    {
        ConfigureLineRenderer(lr, startPos, endPos);

        MeshCollider connectionMesh = currentConnectionObject.AddComponent<MeshCollider>();
        Mesh mesh = new();
        lr.BakeMesh(mesh, cam, true);
        connectionMesh.sharedMesh = mesh;
    }

    private void DrawConnection()
    {
        Vector2 clickPos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(clickPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Settlement"))
        {
            drawing = true;

            currentConnectionObject = new GameObject("Connection");
            currentConnectionObjectLr = currentConnectionObject.AddComponent<LineRenderer>();

            startPos = hit.collider.transform.position;
            startTown = hit.collider.GetComponent<Settlement>();

            ConfigureLineRenderer(currentConnectionObjectLr, startPos, startPos);
        }
    }

    private void CreateConnection()
    {
        Vector2 liftPos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(liftPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Settlement"))
        {
            endPos = hit.collider.transform.position;
            endTown = hit.collider.GetComponent<Settlement>();
            if (startTown != endTown)
            {
                currentConnectionObjectLr.SetPosition(1, endPos);

                Connection currentConnection = currentConnectionObject.AddComponent<Connection>();
                currentConnection.SetupConnection(startPos, endPos, startTown, endTown, numEntites, entityPrefab, entitySpeed, capacity);
                currentConnectionObject.tag = "Connection";

                SetupConnectionVisuals(currentConnectionObjectLr, startPos, endPos);

                connections.Add(currentConnection);
            }
            else
            {
                Destroy(currentConnectionObject);
            }
        }
        else
        {
            Destroy(currentConnectionObject);
        }
    }

    private void DeleteConnection()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Connection"))
            {
                Connection selectedConnection = hit.collider.GetComponent<Connection>();
                selectedConnection.DestroyAllEntities();
                connections.Remove(hit.collider.GetComponent<Connection>());
                Destroy(hit.collider.gameObject);
            }
        }
    }
}

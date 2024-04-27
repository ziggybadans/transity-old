using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControlHandler : MonoBehaviour
{
    public Material lineMaterial;
    private Camera cam;
    public GameObject entityPrefab;
    public float lineWidth = 0.5f;
    public int numEntites = 2;

    private bool drawing;
    private GameObject currentConnectionObject;
    private LineRenderer currentConnectionObjectLr;
    private Vector2 startPos, endPos;
    private Settlement startTown, endTown;
    private List<Connection> connections = new();

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

            currentConnectionObjectLr.positionCount = 2;
            startPos = hit.collider.transform.position;
            startTown = hit.collider.GetComponent<Settlement>();
            currentConnectionObjectLr.SetPosition(0, startPos);
            currentConnectionObjectLr.SetPosition(1, startPos);

            currentConnectionObjectLr.startWidth = lineWidth;
            currentConnectionObjectLr.endWidth = lineWidth;
            currentConnectionObjectLr.material = lineMaterial;
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
            currentConnectionObjectLr.SetPosition(1, endPos);

            Connection currentConnection = currentConnectionObject.AddComponent<Connection>();
            currentConnection.SetupConnection(startPos, endPos, startTown, endTown, numEntites, entityPrefab);
            currentConnectionObject.tag = "Connection";

            currentConnectionObjectLr.positionCount = 2;
            currentConnectionObjectLr.SetPosition(0, startPos);
            currentConnectionObjectLr.SetPosition(1, endPos);
            currentConnectionObjectLr.startWidth = lineWidth;
            currentConnectionObjectLr.endWidth = lineWidth;
            currentConnectionObjectLr.material = lineMaterial;

            MeshCollider connectionMesh = currentConnectionObject.AddComponent<MeshCollider>();
            Mesh mesh = new Mesh();
            currentConnectionObjectLr.BakeMesh(mesh, cam, true);
            connectionMesh.sharedMesh = mesh;

            connections.Add(currentConnection);
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

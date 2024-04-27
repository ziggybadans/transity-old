using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControlHandler : MonoBehaviour
{
    public Material lineMaterial;
    public Camera cam;
    public float lineWidth = 0.5f;
    public GameObject entityPrefab;
    public int numEntites = 2;
    public float entitySpeed = 2f;
    private GameObject currentConnection;
    private Vector2 startPos;
    private Vector2 endPos;
    private bool drawing;
    private List<GameObject> connections = new();

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
            LineRenderer connectionLineRenderer = currentConnection.GetComponent<LineRenderer>();
            Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
            currentPos.z = 0f;
            connectionLineRenderer.SetPosition(1, currentPos);
        }
        if (Input.GetMouseButtonUp(0) && drawing)
        {
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

            currentConnection = new GameObject("Connection");
            LineRenderer connectionLineRenderer = currentConnection.AddComponent<LineRenderer>();
            connectionLineRenderer.gameObject.AddComponent<MeshCollider>();

            connectionLineRenderer.positionCount = 2;
            startPos = hit.collider.transform.position;
            connectionLineRenderer.SetPosition(0, startPos);
            connectionLineRenderer.SetPosition(1, startPos);

            connectionLineRenderer.startWidth = lineWidth;
            connectionLineRenderer.endWidth = lineWidth;
            connectionLineRenderer.useWorldSpace = false;
            connectionLineRenderer.material = lineMaterial;
            connectionLineRenderer.tag = "Connection";
        }
    }

    private void CreateConnection()
    {
        drawing = false;
        LineRenderer connectionLineRenderer = currentConnection.GetComponent<LineRenderer>();

        Vector2 liftPos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(liftPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Settlement"))
        {
            endPos = hit.collider.transform.position;
            connectionLineRenderer.SetPosition(1, endPos);

            Mesh mesh = new();
            connectionLineRenderer.BakeMesh(mesh, cam, true);
            connectionLineRenderer.GetComponent<MeshCollider>().sharedMesh = mesh;

            connections.Add(currentConnection);
            SpawnEntities(startPos, endPos);
        }
        else
        {
            Destroy(connectionLineRenderer.gameObject);
        }
    }

    private void DeleteConnection()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Connection"))
            {
                connections.Remove(hit.collider.gameObject);
                Destroy(hit.collider.gameObject);
                Debug.Log("Remaining connections:" + connections.Count.ToString());
            }
        }
    }

    private void SpawnEntities(Vector3 startPos, Vector3 endPos) {
        Vector3 direction = (endPos - startPos).normalized;
        float lineLength = Vector3.Distance(startPos, endPos);
        float spacing = lineLength / (numEntites + 1);

        for (int i = 1; i <= numEntites; i++) {
            Vector3 spawnPos = startPos + (direction * spacing * i);
            GameObject entity = Instantiate(entityPrefab, spawnPos, Quaternion.identity);
            bool movingForward;
            if (i % 2 == 0) {
                movingForward = false;
            } else {
                movingForward = true;
            }
            entity.GetComponent<EntityHandler>().SetupMovement(startPos, endPos, entitySpeed, movingForward);
        }
    }
}

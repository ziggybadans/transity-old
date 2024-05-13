using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControlHandler : MonoBehaviour
{
    [SerializeReference]
    private Material lineMaterial;
    [SerializeReference]
    private GameObject entityPrefab;
    [SerializeReference]
    private MapGenerator mapGenerator;

    private Camera cam;
    [SerializeField]
    private float LINE_WIDTH = 0.5f;
    private float CAPACITY = 6;
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
        if (Input.GetMouseButtonDown(0)) DrawConnection();
        if (Input.GetMouseButton(0) && drawing)
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
        if (Input.GetMouseButtonDown(1) && !drawing) DeleteConnection();

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
        lr.material = lineMaterial;
    }

    private void SetMesh(LineRenderer lr)
    {
        MeshCollider connectionMesh = currentConnectionObject.AddComponent<MeshCollider>();
        Mesh mesh = new();
        lr.BakeMesh(mesh, cam, true);
        connectionMesh.sharedMesh = mesh;
    }

    private void DrawConnection()
    {
        RaycastHit2D clickPosHit = Raycast();
        if (clickPosHit.collider != null && clickPosHit.collider.CompareTag("Settlement"))
        {
            drawing = true;

            currentConnectionObject = new GameObject("Connection");
            currentConnectionObjectLr = currentConnectionObject.AddComponent<LineRenderer>();

            startPos = clickPosHit.collider.transform.position;
            startTown = clickPosHit.collider.GetComponent<Settlement>();

            ConfigureLineRenderer(currentConnectionObjectLr, startPos, startPos);
        }
    }

    private void CreateConnection()
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

                connections.Add(currentConnection);
            } else Destroy(currentConnectionObject);
        }
        else Destroy(currentConnectionObject);
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

    private RaycastHit2D Raycast() {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        return hit;
    }
}

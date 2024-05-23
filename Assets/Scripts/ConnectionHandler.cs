using System;
using System.Collections.Generic;
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
        ControlHandler.FinishConnection += FinishConnection;
        ControlHandler.CancelConnection += CancelConncetion;
    }

    [SerializeField]
    private float LINE_WIDTH = 0.5f;
    private GameObject currentConnectionObject;
    private LineRenderer currentConnectionObjectLr;
    private Node startNode, endNode;
    private bool drawing;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void SetMesh()
    {
        MeshCollider connectionMesh = currentConnectionObject.AddComponent<MeshCollider>();
        Mesh mesh = new();
        currentConnectionObjectLr.BakeMesh(mesh, cam, true);
        connectionMesh.sharedMesh = mesh;
    }

    private void SetupConnection()
    {
        currentConnectionObject = new GameObject("Connection");
        currentConnectionObjectLr = currentConnectionObject.AddComponent<LineRenderer>();
        currentConnectionObject.AddComponent<Connection>();
        currentConnectionObject.layer = 2;

        currentConnectionObjectLr.positionCount = 2;
        currentConnectionObjectLr.SetPosition(0, startNode.transform.position);
        currentConnectionObjectLr.startWidth = LINE_WIDTH;
        currentConnectionObjectLr.endWidth = LINE_WIDTH;
        currentConnectionObjectLr.material = GameManager.Instance.CreatingConnectionMaterial;
    }

    private void CancelConncetion()
    {
        Destroy(currentConnectionObject);
        startNode = null;
        endNode = null;
        drawing = false;
        currentConnectionObject = null;
        currentConnectionObjectLr = null;
        ControlHandler.Instance.drawing = false;
    }

    public void CreateConnection()
    {
        RaycastHit2D raycast = Raycast();
        if (raycast.collider != null)
        {
            Debug.Log("Raycast hit collider: " + raycast.collider.gameObject.name);

            if (raycast.collider.TryGetComponent<Node>(out var nodeComponent))
            {
                Debug.Log("Node component found with nodeType: " + nodeComponent.nodeType);

                startNode = nodeComponent;
                SetupConnection();
                startNode.connections.Add(currentConnectionObject.GetComponent<Connection>());
                drawing = true;
                ControlHandler.Instance.drawing = true;
            }
            else
            {
                Debug.LogWarning("Collider does not have a Node component: " + raycast.collider.gameObject.name);
            }
        }
        else
        {
            Debug.LogWarning("Raycast did not hit any collider.");
        }
    }

    private void Update()
    {
        if (drawing)
        {
            RaycastHit2D raycast = Raycast();
            if (raycast.collider != null && raycast.collider.TryGetComponent<Cell>(out var cell))
            {
                Vector3 currentPos = new(
                    cell.transform.position.x,
                    cell.transform.position.y,
                    -7f
                );
                currentConnectionObjectLr.SetPosition(1, currentPos);
            }
        }
    }

    public void FinishConnection()
    {
        RaycastHit2D raycast = Raycast();
        if (raycast.collider != null)
        {
            if (raycast.collider.TryGetComponent<Settlement>(out var settlement)
                || raycast.collider.TryGetComponent<Connection>(out var connection))
            {
                endNode = raycast.collider.GetComponent<Node>();
                endNode.connections.Add(currentConnectionObject.GetComponent<Connection>());
            }
            else
            {
                endNode = new GameObject("Node").AddComponent<Node>();
                SpriteRenderer endNodeSprite = endNode.AddComponent<SpriteRenderer>();
                endNodeSprite.sprite = GameManager.Instance.NodeSprite;
                endNodeSprite.color = Color.black;
                endNode.AddComponent<BoxCollider2D>();
                endNode.transform.localScale *= 0.5f;
                Vector3 currentPos = RoundVec3(cam.ScreenToWorldPoint(Input.mousePosition));
                endNode.gameObject.transform.position = new Vector3(
                    GridManager.Instance.GetCellFromPos(currentPos).transform.position.x,
                    GridManager.Instance.GetCellFromPos(currentPos).transform.position.y,
                    -7f
                );
                endNode.nodeType = NodeType.Connection;
            }
            currentConnectionObjectLr.SetPosition(1, endNode.transform.position);
            currentConnectionObjectLr.material = GameManager.Instance.ConnectionMaterial;
            endNode.connections.Add(currentConnectionObject.GetComponent<Connection>());
            SetMesh();
            drawing = false;
            ControlHandler.Instance.drawing = false;
            currentConnectionObject.AddComponent<TransportSpawning>();
        }
    }

    private RaycastHit2D Raycast()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 100f);
        return hit;
    }

    private Vector3Int RoundVec3(Vector3 vector)
    {
        return new Vector3Int(
            Mathf.RoundToInt(vector.x),
            Mathf.RoundToInt(vector.y),
            -10
        );
    }
}

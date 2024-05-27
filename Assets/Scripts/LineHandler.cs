using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LineHandler : MonoBehaviour
{
    public static LineHandler Instance;
    private Camera cam;

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

        ControlHandler.CreateLine += CreateLine;
        ControlHandler.FinishLine += AddToLine;
        ControlHandler.CancelLine += CancelLine;
    }

    private Line line;
    private LineRenderer lineLr;
    [SerializeField]
    private Node firstNode, lastNode;
    private float LINE_WIDTH = 0.5f;

    private void Start() { cam = Camera.main; }

    private RaycastHit2D Raycast()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 100f);
        return hit;
    }

    private RaycastHit2D Raycast(int mask)
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 100f, mask);
        return hit;
    }

    private void CreateLine()
    {
        RaycastHit2D raycast = Raycast();
        if (raycast.collider != null && raycast.collider.TryGetComponent<Node>(out var node))
        {
            if (node.nodeType == NodeType.Settlement)
            {
                Debug.Log("Setting up line with " + node.ToString());
                SetupLine(node);
                Debug.Log("Adding node to path of " + line);
                line.nodes.Add(node);
                line.stops.Add(node.GetComponent<Settlement>());
                line.stopVariety.Add(node.GetComponent<Settlement>().Type);
                node.lines.Add(line);
                firstNode = node;
                lastNode = node;
                ControlHandler.Instance.drawing = true;
            }
        }
    }

    private void SetupLine(Node node)
    {
        line = new GameObject("Line").AddComponent<Line>();
        lineLr = line.AddComponent<LineRenderer>();
        line.gameObject.layer = 2;

        lineLr.positionCount = 2;
        lineLr.SetPosition(0, node.transform.position);
        lineLr.startWidth = LINE_WIDTH;
        lineLr.endWidth = LINE_WIDTH;
        lineLr.material = GameManager.Instance.CreatingLineMaterial;
    }

    private void SetMesh()
    {
        MeshCollider lineMesh = line.AddComponent<MeshCollider>();
        Mesh mesh = new();
        lineLr.BakeMesh(mesh, cam, true);
        lineMesh.sharedMesh = mesh;
    }

    private void Update()
    {
        if (ControlHandler.Instance.drawing && GameManager.Instance.state == GameState.Line)
        {
            RaycastHit2D raycast = Raycast();
            if (raycast.collider != null && raycast.collider.TryGetComponent<Cell>(out var cell))
            {
                Vector3 currentPos = new(
                    cell.transform.position.x,
                    cell.transform.position.y,
                    -7f
                );
                lineLr.SetPosition(lineLr.positionCount - 1, currentPos);
            }
        }
    }

    private void AddToLine()
    {
        RaycastHit2D raycast = Raycast();
        if (raycast.collider != null && raycast.collider.TryGetComponent<Node>(out var node))
        {
            Debug.Log("Hit node!");
            if (firstNode != node)
            {
                Connection connection = null;
                if (lastNode.connections.Count > 0 && node.connections.Count > 0)
                {
                    foreach (Connection conn in lastNode.connections)
                    {
                        if (node.connections.Contains(conn)) connection = conn;
                    }
                }
                if (connection != null)
                {
                    if (node.nodeType == NodeType.Settlement)
                    {
                        if (lastNode != node) line.stops.Add(node.GetComponent<Settlement>());
                        if (!line.stopVariety.Contains(node.GetComponent<Settlement>().Type))
                        {
                            line.stopVariety.Add(node.GetComponent<Settlement>().Type);
                        }

                        if (lastNode == node)
                        {
                            lineLr.material = GameManager.Instance.LineMaterial;
                            SetMesh();
                            ControlHandler.Instance.drawing = false;
                            line.AddComponent<TransportSpawning>();
                        }
                        //else lastNode = node;
                    }

                    if (lastNode != node)
                    {
                        lineLr.SetPosition(lineLr.positionCount - 1, node.transform.position);
                        lineLr.positionCount += 1;
                        line.nodes.Add(node);
                        lastNode = node;
                    }
                }
            }
            else
            {
                lineLr.SetPosition(lineLr.positionCount - 1, node.transform.position);
                line.LOOP = true;
                lineLr.material = GameManager.Instance.LineMaterial;
                SetMesh();
                ControlHandler.Instance.drawing = false;
                line.AddComponent<TransportSpawning>();
            }
        }
    }

    private void CancelLine()
    {
        ControlHandler.Instance.drawing = false;
        Destroy(line.gameObject);
        lineLr = null;
        firstNode = null;
        lastNode = null;
    }
}
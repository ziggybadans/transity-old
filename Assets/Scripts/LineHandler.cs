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
        ControlHandler.FinishLine += FinishLine;
        ControlHandler.CancelLine += CancelLine;
    }

    private Line line;
    private LineRenderer lineLr;
    private Node startNode, endNode;
    private float LINE_WIDTH = 0.25f;

    private void Start()
    {
        cam = Camera.main;
    }

    private RaycastHit2D Raycast()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 100f);
        return hit;
    }

    private void CreateLine()
    {
        RaycastHit2D raycast = Raycast();
        if (raycast.collider != null && raycast.collider.TryGetComponent<Node>(out var node))
        {
            if (node.nodeType == NodeType.Settlement)
            {
                startNode = node;
                SetupLine();
                startNode.lines.Add(line);
                ControlHandler.Instance.drawing = true;
            }
        }
    }

    private void SetupLine()
    {
        line = new GameObject("Line").AddComponent<Line>();
        lineLr = line.AddComponent<LineRenderer>();
        line.gameObject.layer = 2;

        lineLr.positionCount = 2;
        lineLr.SetPosition(0, startNode.transform.position);
        lineLr.startWidth = LINE_WIDTH;
        lineLr.endWidth = LINE_WIDTH;
        lineLr.material = GameManager.Instance.CreatingConnectionMaterial;
    }

    private void Update()
    {
        if (ControlHandler.Instance.drawing)
        {
            RaycastHit2D raycast = Raycast();
            if (raycast.collider != null && raycast.collider.TryGetComponent<Cell>(out var cell))
            {
                Vector3 currentPos = new(
                    cell.transform.position.x,
                    cell.transform.position.y,
                    -7f
                );
                lineLr.SetPosition(1, currentPos);
            }
        }
    }

    // This method backtracks from our destination to our origin settlement to create the path of the line
    private void FinishLine()
    {
        RaycastHit2D raycast = Raycast();
        if (raycast.collider != null && raycast.collider.TryGetComponent<Node>(out var node))
        {
            endNode = node;
            // Firstly, check to see if this is a settlement, as all lines must start and end
            // with settlements.
            if (endNode.nodeType == NodeType.Settlement)
            {
                // We create three variables: 
                // - endOfTrack to see whether we have reached our chosen destination settlement
                // - currentNode
                // - currentConnection
                bool endOfTrack = false;
                bool result = true;
                Node currentNode = endNode;
                List<Node> nodesChecked = new();
                Connection currentConnection;
                // Here, we check to see if the settlement has more than one connection;
                // if so, we will deduce which connection connects to our origin settlement
                if (currentNode.connections.Count > 1)
                {
                    foreach (Connection connection in currentNode.connections)
                    {
                    }
                }
                else
                {

                }
                currentConnection = node.connections[0];

                while (!endOfTrack)
                {
                    if (currentNode.connections.Count > 1)
                    {
                        Node rootNode = currentNode;
                        List<Node> oldNodesChecked = nodesChecked;
                        Connection rightConnection;
                        foreach (Connection connection in currentNode.connections)
                        {
                            currentNode = rootNode;
                            nodesChecked = oldNodesChecked;
                            bool endOfTrack1 = false;
                            while (!endOfTrack1)
                            {
                                Node newNode = currentConnection.nodes[0];
                                nodesChecked.Add(currentNode);

                                if (currentNode != newNode) currentNode = currentConnection.nodes[0];
                                else currentNode = currentConnection.nodes[1];

                                if (nodesChecked.Contains(newNode)) endOfTrack1 = true;
                                else endOfTrack1 = false;
                            }
                            if (currentNode == startNode) {
                                rightConnection = connection;
                                break;
                            }
                        }
                        
                    }
                    else
                    {
                        Node newNode = currentConnection.nodes[0];
                        nodesChecked.Add(currentNode);

                        if (currentNode != newNode) currentNode = currentConnection.nodes[0];
                        else currentNode = currentConnection.nodes[1];

                        if (nodesChecked.Contains(newNode)) endOfTrack = true;
                        else endOfTrack = false;
                    }
                    if (currentNode == startNode) result = true;
                        else result = false;
                }

                if (endOfTrack && !result)
                {
                    Debug.Log("Missing connection in your line!");
                }
            }
        }
    }

    private float CalculateDistance(Node currentNode, Node checkNode)
    {
        float distanceX = Mathf.Abs(currentNode.transform.position.x - checkNode.transform.position.x);
        float distanceY = Mathf.Abs(currentNode.transform.position.y - checkNode.transform.position.y);
        float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY) / GridManager.Instance.GRID_CELL_SIZE;

        return distance;
    }

    private void CancelLine()
    {
        throw new NotImplementedException();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragIndicatorScript : MonoBehaviour
{
    Vector3 startPos;
    Vector3 endPos;
    Camera camera;
    LineRenderer currentLine;
    List<Connection> lines = new List<Connection>();
    Vector3 camOffset = new Vector3(0, 0, 10);

    public GameObject squarePrefab;
    public Material connectionMat;
    public float spawnInterval = 1f;
    public float moveSpeed = 2f;
    private Dictionary<Connection, Coroutine> spawnCoroutines = new Dictionary<Connection, Coroutine>();

    private void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Town"))
            {
                currentLine = new GameObject("Line").AddComponent<LineRenderer>();
                currentLine.gameObject.AddComponent<MeshCollider>();
                currentLine.positionCount = 2;
                startPos = hit.collider.transform.position;
                currentLine.SetPosition(0, startPos);
                currentLine.useWorldSpace = true;
                currentLine.material = connectionMat;
                currentLine.startColor = Color.black;
                currentLine.endColor = Color.black;
                currentLine.tag = "Connection";
            }
        }

        if (Input.GetMouseButton(0) && currentLine != null)
        {
            endPos = camera.ScreenToWorldPoint(Input.mousePosition) + camOffset;
            currentLine.SetPosition(1, endPos);
        }

        if (Input.GetMouseButtonUp(0) && currentLine != null)
        {
            Vector2 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Town"))
            {
                Vector2Int currentLinePos = Vector2Int.RoundToInt(new Vector2(currentLine.GetPosition(0).x, currentLine.GetPosition(0).y));
                Vector2Int currentMousePos = Vector2Int.RoundToInt(new Vector2(hit.collider.transform.position.x, hit.collider.transform.position.y));
                if (currentLinePos != currentMousePos)
                {
                    bool connectionExistsAlready = false;

                    foreach (Connection existingConnection in lines)
                    {
                        Vector2Int existingLineStartPos = Vector2Int.RoundToInt(existingConnection.StartPos);
                        Vector2Int existingLineEndPos = Vector2Int.RoundToInt(existingConnection.EndPos);
                        if ((existingLineStartPos == currentLinePos && existingLineEndPos == currentMousePos) || (existingLineStartPos == currentMousePos && existingLineEndPos == currentLinePos))
                        {
                            connectionExistsAlready = true;
                            break;
                        }
                    }

                    if (!connectionExistsAlready)
                    {
                        currentLine.SetPosition(1, hit.collider.transform.position);
                        Mesh lineBakedMesh = new Mesh();
                        currentLine.BakeMesh(lineBakedMesh, true);
                        currentLine.GetComponent<MeshCollider>().sharedMesh = lineBakedMesh;

                        Connection newConnection = new Connection(currentLine.GetPosition(0), currentLine.GetPosition(1), currentLine);
                        lines.Add(newConnection);
                        OnConnectionFormed(newConnection);
                    }
                    else
                    {
                        LineRenderer currentLR = currentLine.GetComponent<LineRenderer>();
                        Connection connectionToRemove = lines.Find(c => c.LineRenderer == currentLR);
                        lines.Remove(connectionToRemove);
                        Destroy(currentLine.gameObject);
                    }
                }
            }
            else
            {
                LineRenderer currentLR = currentLine.GetComponent<LineRenderer>();
                Connection connectionToRemove = lines.Find(c => c.LineRenderer == currentLR);
                lines.Remove(connectionToRemove);
                Destroy(currentLine.gameObject);
            }
            currentLine = null;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Town"))
            {
                Vector2Int townPos = Vector2Int.RoundToInt(hit.collider.transform.position);
                Debug.Log("Hit town at: " + townPos.ToString());

                List<Connection> connectionsToRemove = new List<Connection>();

                foreach (Connection existingConnection in lines)
                {
                    if (townPos == existingConnection.StartPos || townPos == existingConnection.EndPos)
                    {
                        connectionsToRemove.Add(existingConnection);
                    }
                }

                foreach (Connection connectionToRemove in connectionsToRemove)
                {
                    lines.Remove(connectionToRemove);
                    Destroy(connectionToRemove.LineRenderer.gameObject);
                }

                connectionsToRemove.Clear();
            }

            Vector3 mousePos3 = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -transform.position.z));
            RaycastHit hit3D;

            if (Physics.Raycast(mousePos3, transform.TransformDirection(Vector3.forward), out hit3D, Mathf.Infinity))
            {
                if (hit3D.collider.CompareTag("Connection"))
                {
                    Debug.Log("Hit connection at: " + hit3D.collider.transform.position.ToString());
                    GameObject selectedLine = hit3D.collider.gameObject;
                    LineRenderer clickedLine = selectedLine.GetComponent<LineRenderer>();

                    Connection connectionToRemove = lines.Find(c => c.LineRenderer == clickedLine);
                    lines.Remove(connectionToRemove);
                    Destroy(selectedLine);
                    OnConnectionBroken(connectionToRemove);
                }
            }
        }
    }

    private void OnConnectionFormed(Connection connection)
    {
        Coroutine spawnCoroutine = StartCoroutine(SpawnSquares(connection));
        spawnCoroutines[connection] = spawnCoroutine;
    }

    private IEnumerator SpawnSquares(Connection connection)
    {
        while (true)
        {
            GameObject square = Instantiate(squarePrefab, connection.StartPos, Quaternion.identity);
            square.GetComponent<SquareScript>().connection = connection;
            StartCoroutine(MoveSquare(square, connection.StartPos, connection.EndPos));
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator MoveSquare(GameObject square, Vector2 startPosition, Vector2 endPosition)
    {
        float t = 0f;
        while (t < 1f && square != null)
        {
            t += moveSpeed * Time.deltaTime;
            square.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
        Destroy(square);
    }

    private void OnConnectionBroken(Connection connection)
    {
        if (spawnCoroutines.ContainsKey(connection))
        {
            StopCoroutine(spawnCoroutines[connection]);
            spawnCoroutines.Remove(connection);
        }
        GameObject[] squares = GameObject.FindGameObjectsWithTag("Car");
        foreach (GameObject square in squares)
        {
            if (square != null)
            {
                SquareScript squareScript = square.GetComponent<SquareScript>();
                if (squareScript != null && squareScript.connection == connection)
                {
                    Destroy(square);
                }
            }
        }
    }
}

public class Connection
{
    Vector2 startPos;
    Vector2 endPos;
    LineRenderer lineRenderer;

    public Vector2 StartPos { get { return startPos; } }
    public Vector2 EndPos { get { return endPos; } }
    public LineRenderer LineRenderer { get { return lineRenderer; } }

    public Connection(Vector2 startPos, Vector2 endPos, LineRenderer lineRenderer)
    {
        this.startPos = startPos;
        this.endPos = endPos;
        this.lineRenderer = lineRenderer;
    }
}

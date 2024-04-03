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
                startPos = camera.ScreenToWorldPoint(Input.mousePosition) + camOffset;
                currentLine.SetPosition(0, startPos);
                currentLine.useWorldSpace = true;
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
                currentLine.SetPosition(1, hit.collider.transform.position);
                Mesh lineBakedMesh = new Mesh();
                currentLine.BakeMesh(lineBakedMesh, true);
                currentLine.GetComponent<MeshCollider>().sharedMesh = lineBakedMesh;

                lines.Add(new Connection(currentLine.GetPosition(0), currentLine.GetPosition(1), currentLine));
            }
            else
            {
                LineRenderer currentLR = currentLine.GetComponent<LineRenderer>();
                Connection connectionToRemove = lines.Find(c => c.LineRenderer == currentLR);
                //int index = lines.IndexOf(currentLR);
                lines.Remove(connectionToRemove);
                Destroy(currentLine.gameObject);
            }
            currentLine = null;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -transform.position.z));
            RaycastHit hit;

            if (Physics.Raycast(mousePos, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Connection"))
                {
                    GameObject selectedLine = hit.collider.gameObject;
                    LineRenderer clickedLine = selectedLine.GetComponent<LineRenderer>();

                    Connection connectionToRemove = lines.Find(c => c.LineRenderer == clickedLine);
                    //int index = lines.IndexOf(clickedLine);
                    lines.Remove(connectionToRemove);
                    Destroy(selectedLine);
                }
            }
        }
    }
}

public class Connection {
    Vector2 startPos;
    Vector2 endPos;
    LineRenderer lineRenderer;

    public Vector2 StartPos { get { return startPos; }}
    public Vector2 EndPos { get { return endPos; }}
    public LineRenderer LineRenderer { get { return lineRenderer; }}

    public Connection(Vector2 startPos, Vector2 endPos, LineRenderer lineRenderer) {
        this.startPos = startPos;
        this.endPos = endPos;
        this.lineRenderer = lineRenderer;
    }
}

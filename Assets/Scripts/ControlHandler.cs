using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControlHandler : MonoBehaviour
{
    public Material lineMaterial;
    private LineRenderer lineRenderer;
    public Camera cam;
    private Vector2 startPos;
    private Vector2 endPos;
    private bool drawing;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(clickPos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Settlement"))
            {
                drawing = true;
                lineRenderer = new GameObject("Connection").AddComponent<LineRenderer>();
                lineRenderer.gameObject.AddComponent<MeshCollider>();
                lineRenderer.positionCount = 2;
                startPos = hit.collider.transform.position;
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, startPos);
                lineRenderer.SetWidth(0.5f, 0.5f);
                lineRenderer.useWorldSpace = false;
                lineRenderer.material = lineMaterial;
                lineRenderer.tag = "Connection";
            }
        }
        if (drawing)
        {
            Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
            currentPos.z = 0f;
            lineRenderer.SetPosition(1, currentPos);
        }
        if (Input.GetMouseButtonUp(0) && drawing)
        {
            drawing = false;

            Vector2 liftPos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(liftPos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Settlement"))
            {
                endPos = hit.collider.transform.position;
                lineRenderer.SetPosition(1, endPos);
                Mesh mesh = new();
                lineRenderer.BakeMesh(mesh, cam, true);
                lineRenderer.GetComponent<MeshCollider>().sharedMesh = mesh;
            }
            else
            {
                Destroy(lineRenderer.gameObject);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Connection"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }
}

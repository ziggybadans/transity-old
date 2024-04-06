using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadtreeTest : MonoBehaviour
{
    public Rect mapArea; // Define the map area in the Inspector
    public int numPoints = 100; // Number of random points to generate
    private Quadtree quadtree;
    public Rect queryRange;

    private void Start()
    {
        quadtree = new Quadtree(mapArea);

        // Generate random points and insert them into the Quadtree
        for (int i = 0; i < numPoints; i++)
        {
            float x = Random.Range(mapArea.xMin, mapArea.xMax);
            float y = Random.Range(mapArea.yMin, mapArea.yMax);
            quadtree.Insert(new Vector2(x, y));
        }

        // Test the Query method
        List<Vector2> pointsInRange = quadtree.Query(queryRange);
        Debug.Log($"Points in range: {pointsInRange.Count}");
    }

    private void OnDrawGizmos()
    {
        if (quadtree != null)
        {
            DrawQuadtree(quadtree);
        }

        // Draw the quadtree boundary rectangle
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(mapArea.center, mapArea.size);

        // Draw the query range rectangle
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(queryRange.center, queryRange.size);

        // Draw the points within the query range
        Gizmos.color = Color.yellow;
        List<Vector2> pointsInRange = quadtree.Query(queryRange);
        foreach (Vector2 point in pointsInRange) {
            Gizmos.DrawSphere(point, 0.05f);
        }
    }

    private void DrawQuadtree(Quadtree quadtree)
    {
        // Draw the boundary of the Quadtree node
        Gizmos.DrawWireCube(quadtree.boundary.center, quadtree.boundary.size);

        // Draw the points inside the Quadtree node
        Gizmos.color = Color.red;
        foreach (Vector2 point in quadtree.points)
        {
            Gizmos.DrawSphere(point, 0.1f);
        }

        // Recursively draw the child Quadtree nodes
        if (quadtree.children != null)
        {
            foreach (Quadtree child in quadtree.children)
            {
                DrawQuadtree(child);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public float cellSize = 2f;
    private int width = 100;
    private int height = 100;
    private Rect[,] cellBounds;

    private void Start()
    {
        InitializeCellBounds();
    }

    private void OnRenderObject()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        Gizmos.color = Color.gray;

        // Draw vertical lines
        for (int x = 0; x <= width; x++)
        {
            Vector3 start = new Vector3(x * cellSize, 0f, 0f);
            Vector3 end = new Vector3(x * cellSize, height * cellSize, 0f);
            Gizmos.DrawLine(start, end);
        }

        // Draw horizontal lines
        for (int y = 0; y <= height; y++)
        {
            Vector3 start = new Vector3(0f, y * cellSize, 0f);
            Vector3 end = new Vector3(width * cellSize, y * cellSize, 0f);
            Gizmos.DrawLine(start, end);
        }
    }

    private void InitializeCellBounds()
    {
        cellBounds = new Rect[width, height];
        Vector3 gridOrigin = Vector3.zero;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 cellMin = new Vector2(gridOrigin.x + x * cellSize, gridOrigin.y + y * cellSize);
                Vector2 cellMax = new Vector2(gridOrigin.x + (x + 1) * cellSize, gridOrigin.y + (y + 1) * cellSize);
                cellBounds[x, y] = new Rect(cellMin, cellMax - cellMin);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Range(1, 100)]
    public float cellSize = 1f;
    public int width = 100;
    public int height = 100;

    public void ChangeZoom(int zoom)
    {
        if (zoom < 1)
        {
            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        Vector3 gridOrigin = Vector3.zero;

        // Draw vertical lines
        for (int x = 0; x <= width; x++)
        {
            Vector3 start = new Vector3(gridOrigin.x + x * cellSize, gridOrigin.y, 0f);
            Vector3 end = new Vector3(gridOrigin.x + x * cellSize, gridOrigin.y + height * cellSize, 0f);
            Gizmos.DrawLine(start, end);
        }

        // Draw horizontal lines
        for (int y = 0; y <= height; y++)
        {
            Vector3 start = new Vector3(gridOrigin.x, gridOrigin.y + y * cellSize, 0f);
            Vector3 end = new Vector3(gridOrigin.x + width * cellSize, gridOrigin.y + y * cellSize, 0f);
            Gizmos.DrawLine(start, end);
        }
    }
}

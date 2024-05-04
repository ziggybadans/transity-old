using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 34;
    public int gridHeight = 19;
    public float gridCellSize = 2f;
    private Cell[,] gridArray;
    private GameObject cell;
    private void Start()
    {
        gridArray = new Cell[gridWidth, gridHeight];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Debug.DrawLine(GetBorderFromPosition(x, y), GetBorderFromPosition(x, y + 1), Color.gray, 1000f);
                Debug.DrawLine(GetBorderFromPosition(x, y), GetBorderFromPosition(x + 1, y), Color.gray, 1000f);
                cell = new GameObject("Cell");
                cell.AddComponent<Cell>();
                Instantiate(cell, GetCellFromPosition(x, y), Quaternion.identity, gameObject.transform);
                cell.transform.position = GetCellFromPosition(x, y);
                gridArray[x, y] = cell.GetComponent<Cell>();
            }
        }
        Debug.DrawLine(GetBorderFromPosition(0, gridHeight), GetBorderFromPosition(gridWidth, gridHeight), Color.gray, 1000f);
        Debug.DrawLine(GetBorderFromPosition(gridWidth, 0), GetBorderFromPosition(gridWidth, gridHeight), Color.gray, 1000f);
    }

    private Vector3 GetBorderFromPosition(int x, int y)
    {
        return new Vector3(x, y) * gridCellSize;
    }

    private Vector3 GetCellFromPosition(int x, int y)
    {
        int cellX = (int)(x * gridCellSize + (gridCellSize / 2f));
        int cellY = (int)(y * gridCellSize + (gridCellSize / 2f));

        return new Vector3(cellX, cellY);
    }
}

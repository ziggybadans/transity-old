using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 34;
    public int gridHeight = 19;
    public float gridCellSize = 2f;
    public Cell[,] gridArray;
    public Sprite cellSprite;
    private GameObject cell;
    private void Start()
    {
        gridArray = new Cell[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Debug.DrawLine(GetBorderFromPosition(x, y), GetBorderFromPosition(x, y + 1), Color.gray, 1000f);
                Debug.DrawLine(GetBorderFromPosition(x, y), GetBorderFromPosition(x + 1, y), Color.gray, 1000f);
                
                cell = new GameObject("Cell");
                cell.AddComponent<Cell>();
                cell.AddComponent<SpriteRenderer>().sprite = cellSprite;
                cell.transform.parent = transform;
                cell.transform.position = GetPositionForCell(x, y);
                cell.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
                cell.GetComponent<Cell>().cellSize = gridCellSize;
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

    private Vector3 GetPositionForCell(int x, int y)
    {
        int cellX = (int)(x * gridCellSize + (gridCellSize / 2f));
        int cellY = (int)(y * gridCellSize + (gridCellSize / 2f));

        return new Vector3(cellX, cellY, -5f);
    }

    public Cell GetCellFromPosition(Vector2Int pos) {
        return gridArray[pos.x, pos.y];
    }
}

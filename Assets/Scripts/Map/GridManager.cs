using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

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
    }

    public int GRID_WIDTH = 34;
    public int GRID_HEIGHT = 19;
    public float GRID_CELL_SIZE = 2f;
    
    [SerializeReference]
    private static Sprite s_cellSprite;
    private Cell[,] _gridArray;
    private GameObject _cell;

    public Cell[,] GetCells() { return _gridArray; }

    private void Start()
    {
        _gridArray = new Cell[GRID_WIDTH, GRID_HEIGHT];

        for (int x = 0; x < GRID_WIDTH; x++)
        {
            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                Debug.DrawLine(GetBorderFromPosition(x, y), GetBorderFromPosition(x, y + 1), Color.gray, 1000f);
                Debug.DrawLine(GetBorderFromPosition(x, y), GetBorderFromPosition(x + 1, y), Color.gray, 1000f);

                CreateNewCell(x, y);
            }
        }
        Debug.DrawLine(GetBorderFromPosition(0, GRID_HEIGHT), GetBorderFromPosition(GRID_WIDTH, GRID_HEIGHT), Color.gray, 1000f);
        Debug.DrawLine(GetBorderFromPosition(GRID_WIDTH, 0), GetBorderFromPosition(GRID_WIDTH, GRID_HEIGHT), Color.gray, 1000f);
    }

    private void CreateNewCell(int x, int y)
    {
        _cell = new GameObject("Cell");
        _cell.AddComponent<Cell>();
        _cell.AddComponent<SpriteRenderer>().sprite = s_cellSprite;
        _cell.transform.parent = transform;
        _cell.transform.position = GetPositionForCell(x, y);
        _cell.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        _gridArray[x, y] = _cell.GetComponent<Cell>();
    }

    public Vector3 GetBorderFromPosition(int x, int y)
    {
        return new Vector3(x, y) * GRID_CELL_SIZE;
    }

    public Vector3 GetPositionForCell(int x, int y)
    {
        int cellX = (int)(x * GRID_CELL_SIZE + (GRID_CELL_SIZE / 2f));
        int cellY = (int)(y * GRID_CELL_SIZE + (GRID_CELL_SIZE / 2f));

        return new Vector3(cellX, cellY, -5f);
    }

    public Cell GetCellFromPosition(Vector2Int pos)
    {
        return _gridArray[pos.x, pos.y];
    }
}

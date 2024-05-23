using System;
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

        MapGenerator.OnMapGenerationFinish += UpdateDebugOverlay;
        GameManager.Instance.OnDebugModeChanged += UpdateDebugOverlay;
    }

    public int GRID_WIDTH = 34;
    public int GRID_HEIGHT = 19;
    public float GRID_CELL_SIZE = 2f;

    [SerializeReference]
    private Sprite _cellSprite;
    private Cell[,] _gridArray;
    private GameObject _cell;
    private Camera cam;

    public Cell[,] GetCells() { return _gridArray; }

    private void Start()
    {
        cam = Camera.main;

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

        AdjustCamera();
    }

    private void AdjustCamera()
    {
        float totalGridWidth = GRID_WIDTH * GRID_CELL_SIZE;
        float totalGridHeight = GRID_HEIGHT * GRID_CELL_SIZE;

        float aspectRatio = cam.aspect;

        float orthoSizeFromHeight = totalGridHeight / 2f;
        float orthoSizeFromWidth = totalGridWidth / (2f * aspectRatio);

        cam.orthographicSize = Mathf.Max(orthoSizeFromHeight, orthoSizeFromWidth) + 0.5f;
        cam.transform.position = new Vector3(totalGridWidth / 2f, totalGridHeight / 2f, -50f);
    }

    private void CreateNewCell(int x, int y)
    {
        _cell = new GameObject("Cell");
        _cell.AddComponent<Cell>();
        _cell.AddComponent<SpriteRenderer>().sprite = _cellSprite;
        _cell.AddComponent<BoxCollider2D>();
        _cell.transform.parent = transform;
        _cell.transform.position = new Vector3(CalculateCellPos(x), CalculateCellPos(y), -5f);
        _cell.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        _gridArray[x, y] = _cell.GetComponent<Cell>();
    }

    public Vector3 GetBorderFromPosition(int x, int y)
    {
        return new Vector3(x, y) * GRID_CELL_SIZE;
    }

    public int CalculateCellPos(int pos)
    {
        int cellPos = (int)(pos * GRID_CELL_SIZE + (GRID_CELL_SIZE / 2f));
        return cellPos;
    }

    public Vector3 CalculateCellVector(Vector2 pos)
    {
        int cellPosX = (int)((pos.x - (GRID_CELL_SIZE / 2f)) / 2f);
        int cellPosY = (int)((pos.y - (GRID_CELL_SIZE / 2f)) / 2f);
        return new Vector3(cellPosX, cellPosY, -10f);
    }

    public int CalculateArrayIndex(int pos)
    {
        int cellPos = (int)((pos - (GRID_CELL_SIZE / 2f)) / 2f);
        return cellPos;
    }

    public Cell GetCellFromPos(Vector2 pos)
    {
        return _gridArray[CalculateArrayIndex((int)pos.x), CalculateArrayIndex((int)pos.y)];
    }

    private void UpdateDebugOverlay()
    {
        int debugMode = GameManager.Instance.DebugMode;
        float probability;
        if (debugMode > 0 && debugMode <= 3)
        {
            SettlementType view = debugMode switch
            {
                1 => SettlementType.City,
                2 => SettlementType.Town,
                3 => SettlementType.Rural,
                _ => throw new InvalidOperationException("Invalid debugMode value")
            };

            foreach (Cell cell in _gridArray) {
                probability = CheckCellProbability(cell, view);
                Color cellColor = Color.Lerp(Color.red, Color.green, probability);
                cell.GetComponent<Renderer>().material.color = cellColor;
            }
        } else {
            foreach (Cell cell in _gridArray) cell.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
        }
    }

    private float CheckCellProbability(Cell cell, SettlementType settlementType)
    {
        float value = settlementType switch
        {
            SettlementType.City => cell.GetProbability(settlementType) * 2,
            _ => cell.GetProbability(settlementType),
        };
        return value;
    }
}

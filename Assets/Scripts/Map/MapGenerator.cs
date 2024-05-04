using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    [Range(5, 15)]
    public int minTowns = 10;
    [Range(10, 30)]
    public int maxTowns = 20;
    [Min(1)]
    public int numCities = 1;
    [Min(2f)]
    public int minDistance = 2;
    public Settlement cityPrefab;
    public Settlement townPrefab;
    public Settlement ruralPrefab;
    public GameObject entityPrefab;
    public GameObject textPrefab;
    public Transform gridParent;
    private Camera cam;
    public GridManager grid;
    private int numCellsX, numCellsY;
    public List<Settlement> settlements = new List<Settlement>();
    private float[,] spawnProbabilities;

    private void Start()
    {
        cam = Camera.main;

        numCellsX = grid.gridWidth;
        numCellsY = grid.gridHeight;

        spawnProbabilities = new float[numCellsX, numCellsY];

        UpdateProbabilities();
        GenerateMap();
    }

    private void GenerateMap()
    {
        // 0 = first city, 1 = other cities, 2 = no more cities
        int cityLimit = 0;
        bool townJustCompleted;
        for (int i = 1; i <= numCities; i++)
        {
            townJustCompleted = false;
            if (cityLimit < 2)
            {
                for (int x = 0; x < numCellsX; x++)
                {
                    for (int y = 0; y < numCellsY; y++)
                    {
                        Cell currentCell = grid.GetCellFromPosition(new Vector2Int(x, y));
                        float spawnProbability = currentCell.settlementSpawnProbability;

                        float r = UnityEngine.Random.Range(0.25f, 1);
                        if (spawnProbability > r)
                        {
                            Settlement city = Instantiate(cityPrefab, currentCell.transform.position + (Vector3.forward * -2f), Quaternion.identity, currentCell.transform);
                            city.Type = SettlementType.City;
                            city.entityPrefab = entityPrefab;
                            city.map = gameObject;
                            settlements.Add(city);
                            currentCell.settlements.Add(city);

                            UpdateProbabilities();
                            cityLimit = 1;
                            townJustCompleted = true;
                            break;
                        }
                        else if (cityLimit == 1 || townJustCompleted)
                        {
                            break;
                        }
                    }
                    if (townJustCompleted)
                    {
                        break;
                    }
                }
            }
        }
    }

    private void UpdateProbabilities()
    {
        // Iterating through x coordinate of grid
        for (int x = 0; x < numCellsX; x++)
        {
            // Iterating through y coordinate of grid
            for (int y = 0; y < numCellsY; y++)
            {
                // This is the cell we're currently checking
                Cell currentCell = grid.GetCellFromPosition(new Vector2Int(x, y));
                if (currentCell.HasSettlement())
                {
                    currentCell.settlementSpawnProbability = 0f;
                }
                else
                {
                    currentCell.settlementSpawnProbability = 1f;
                    foreach (Cell cell in grid.gridArray)
                    {
                        if (cell == currentCell) {
                            continue;
                        }

                        if (cell.HasSettlement())
                        {
                            float distanceX = Mathf.Abs(currentCell.transform.position.x - cell.transform.position.x);
                            float distanceY = Mathf.Abs(currentCell.transform.position.y - cell.transform.position.y);
                            float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

                            float probability = Mathf.Max(0, ((50f / Mathf.Pow(1f + Mathf.Exp(-0.5f * (distance - 10f)), 1f)) - 1f) / 100f);
                            currentCell.settlementSpawnProbability *= probability;
                        }
                    }
                }

                float clampedProbability = currentCell.settlementSpawnProbability * 2;
                Color cellColor = Color.Lerp(Color.red, Color.green, clampedProbability);
                currentCell.GetComponent<Renderer>().material.color = cellColor;
            }
        }
    }
}

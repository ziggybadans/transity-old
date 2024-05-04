using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
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
                        Vector2Int currentCell = GetCellFromPosition(new Vector2Int(x, y));
                        float spawnProbability = spawnProbabilities[x, y];

                        int r = UnityEngine.Random.Range(0, 100);
                        if (spawnProbability > r)
                        {
                            Settlement city = Instantiate(cityPrefab, new Vector3(currentCell.x, currentCell.y, -2f), Quaternion.identity);
                            city.Type = SettlementType.City;
                            city.entityPrefab = entityPrefab;
                            city.map = gameObject;
                            settlements.Add(city);

                            UpdateProbabilities();
                            cityLimit = 1;
                            townJustCompleted = true;
                            break;
                        } else if (cityLimit == 1 || townJustCompleted)
                        {
                            break;
                        }
                    }
                    if (townJustCompleted) {
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
                Vector2Int currentCell = GetCellFromPosition(new Vector2Int(x, y));

                if (settlements.Count > 0) {
                    foreach (Settlement settlement in settlements) {
                        
                    }
                }
                spawnProbabilities[x, y] = 100f;
                GameObject textInstance = Instantiate(textPrefab, new Vector3(currentCell.x, currentCell.y, -2f), Quaternion.identity, gridParent);
                TextMeshProUGUI textComponent = textInstance.GetComponent<TextMeshProUGUI>();
                textComponent.text = spawnProbabilities[x, y].ToString("F2");
            }
        }
    }

    private Vector2 GetRandomPosition()
    {
        int randomCellX = UnityEngine.Random.Range(0, numCellsX * 2);
        int randomCellY = UnityEngine.Random.Range(0, numCellsY * 2);

        Vector2 townPosition = GetCellFromPosition(new Vector2Int(randomCellX, randomCellY));
        return townPosition;
    }

    private Vector2Int GetCellFromPosition(Vector2Int position)
    {
        int cellX = (int)(position.x * grid.gridCellSize + (grid.gridCellSize / 2f));
        int cellY = (int)(position.y * grid.gridCellSize + (grid.gridCellSize / 2f));

        return new Vector2Int(cellX, cellY);
    }
}

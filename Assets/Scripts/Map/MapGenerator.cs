using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                        Cell currentCell = grid.GetCellFromPosition(new Vector2Int(x, y));
                        float spawnProbability = currentCell.settlementSpawnProbability;

                        int r = UnityEngine.Random.Range(0, 100);
                        if (spawnProbability > r)
                        {
                            Settlement city = Instantiate(cityPrefab, currentCell.transform.position, Quaternion.identity, currentCell.transform);
                            city.Type = SettlementType.City;
                            city.entityPrefab = entityPrefab;
                            city.map = gameObject;
                            settlements.Add(city);
                            currentCell.settlements.Add(city);

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
                Cell currentCell = grid.GetCellFromPosition(new Vector2Int(x, y));
                if (currentCell.HasSettlement()) {
                    currentCell.settlementSpawnProbability = 0f;
                } else {
                    currentCell.settlementSpawnProbability = 100f;
                }

                /*
                TextMeshProUGUI textComponent;
                if (currentCell.debugProbability.Count == 0) {
                    GameObject textInstance = Instantiate(textPrefab, currentCell.transform.position, Quaternion.identity, gridParent);
                    textComponent = textInstance.GetComponent<TextMeshProUGUI>();
                    currentCell.debugProbability.Add(textComponent);
                } else {
                    textComponent = currentCell.debugProbability.First();
                }
                textComponent.text = currentCell.settlementSpawnProbability.ToString("F2");
                */

                float clampedProbability = Mathf.Clamp01(currentCell.settlementSpawnProbability);
                Color cellColor = Color.Lerp(Color.red, Color.green, clampedProbability);
                currentCell.GetComponent<Renderer>().material.color = cellColor;
            }
        }
    }
}

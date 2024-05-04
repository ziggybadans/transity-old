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
    public int numTriesCities = 1;
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
        for (int i = 1; i <= numTriesCities; i++)
        {
            int x = UnityEngine.Random.Range(0, numCellsX - 1);
            int y = UnityEngine.Random.Range(0, numCellsY - 1);
            Cell currentCell = grid.GetCellFromPosition(new Vector2Int(x, y));
            float spawnProbability = currentCell.settlementSpawnProbability;

            float r = UnityEngine.Random.Range(0, 100) / 100f;
            if (spawnProbability > r && !currentCell.HasSettlement())
            {
                Settlement city = Instantiate(cityPrefab, currentCell.transform.position + (Vector3.forward * -2f), Quaternion.identity, currentCell.transform);
                city.Type = SettlementType.City;
                city.entityPrefab = entityPrefab;
                city.map = gameObject;
                settlements.Add(city);
                currentCell.settlements.Add(city);
            }

            UpdateProbabilities();
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
                        if (cell == currentCell)
                        {
                            continue;
                        }

                        if (cell.HasSettlement())
                        {
                            float distanceX = Mathf.Abs(currentCell.transform.position.x - cell.transform.position.x);
                            float distanceY = Mathf.Abs(currentCell.transform.position.y - cell.transform.position.y);
                            float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

                            float probability = Mathf.Max(0, ((100f / Mathf.Pow(1f + Mathf.Exp(-0.5f * (distance - 10f)), 1f)) - 1f) / 100f);
                            if (probability > 0)
                            {
                                currentCell.settlementSpawnProbability *= probability / 2;
                            }
                            else
                            {
                                currentCell.settlementSpawnProbability *= probability;
                            }
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

using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering.UI;

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
    private Camera cam;
    public GridManager grid;
    public HeatmapRenderer heatmapRenderer;
    public List<Settlement> settlements = new List<Settlement>();

    private void Start()
    {
        cam = Camera.main;

        GenerateMap();
    }

    private void GenerateMap()
    {
        // Collapse the number of towns from the range given
        int numTowns = Random.Range(minTowns, maxTowns);

        // Spawning cities
        for (int i = 1; i <= numCities; i++)
        {
            // Always place at least one city
            if (i == 1)
            {
                Settlement city = Instantiate(cityPrefab, GetRandomPosition(), Quaternion.identity);
                city.Type = SettlementType.City;
                city.entityPrefab = entityPrefab;
                city.map = gameObject;
                settlements.Add(city);
            }
            // Exponentially lower chance of another city when there are multiple
            else if (i > 1)
            {
                float[,] spawnProbabilities = new float[grid.numCellsX, grid.numCellsY];
                // Iterating through x coordinate of grid
                for (int x = -grid.numCellsX; x <= grid.numCellsX; x++)
                {
                    // Iterating through y coordinate of grid
                    for (int y = -grid.numCellsY; y <= grid.numCellsY; y++)
                    {
                        // This is the cell we're currently checking
                        Vector2Int currentCell = new Vector2Int(x, y);

                        // A baseline for our probability that will decrease over the course of the check
                        float combinedProbability = 1f;
                        // Checking through each city
                        foreach (Settlement settlement in settlements)
                        {
                            // Get the distance from the city to the current cell
                            Vector2Int cityPos = GetCellFromPosition(settlement.gameObject.transform.position);
                            int distanceX = Mathf.Abs(currentCell.x - cityPos.x);
                            int distanceY = Mathf.Abs(currentCell.y - cityPos.y);
                            float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

                            // Calculate the probability based on custom equation, must be above 4 squares away, ceiling probability of 50%
                            float probability = (50f / Mathf.Pow(1f + Mathf.Exp(-0.5f * (distance - 10f)), 1f)) - 1f;
                            combinedProbability *= (100f - probability) / 100f;
                        }

                        float spawnProbability = (1f - combinedProbability) * 100f;
                        spawnProbabilities[x, y] = spawnProbability / 100f;
                        // If we do make it over the probability threshold, then we spawn the city, as long as it is within the bounds of the map
                        int r = Random.Range(0, 100);
                        if (spawnProbability > r)
                        {
                            int targetX = currentCell.x;
                            int targetY = currentCell.y;

                            if (targetX >= 0 && targetX < grid.numCellsX && targetY >= 0 && targetY < grid.numCellsY)
                            {
                                Settlement city = Instantiate(cityPrefab, new Vector2(targetX, targetY), Quaternion.identity);
                                city.Type = SettlementType.City;
                                city.entityPrefab = entityPrefab;
                                city.map = gameObject;
                                settlements.Add(city);
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                break;
            }
        }

        /*
        for (int i = 1; i < numTowns; i++)
        {
            if (Random.value < 0.75f)
            {
                SettlementType type = SettlementType.RuralTown;
                Settlement ruralTown = Instantiate(ruralPrefab, GetRandomPosition(), Quaternion.identity);
                ruralTown.Type = type;
                ruralTown.entityPrefab = entityPrefab;
                ruralTown.map = gameObject;
                settlements.Add(ruralTown);
            }
            else if (Random.value < 0.5f)
            {
                SettlementType type = SettlementType.RegularTown;
                Settlement town = Instantiate(townPrefab, GetRandomPosition(), Quaternion.identity);
                town.Type = type;
                town.entityPrefab = entityPrefab;
                town.map = gameObject;
                settlements.Add(town);
            }
            else
            {
                i--;
            }
        }
        */

        /*
        // Adjust positions to ensure minimum distance between towns
        for (int i = 0; i < settlements.Count; i++)
        {
            Settlement town = settlements[i];
            Vector2 position = town.transform.position;

            for (int j = 0; j < settlements.Count; j++)
            {
                if (i == j) continue;

                Settlement otherTown = settlements[j];
                Vector2 otherPosition = otherTown.transform.position;

                float distance = Vector2.Distance(position, otherPosition);
                if (distance < minDistanceBetweenTowns)
                {
                    Vector2 direction = (position - otherPosition).normalized;
                    position = otherPosition + direction * minDistanceBetweenTowns;
                    town.transform.position = position;
                }
            }
        }
        */
    }

    private Vector2 GetRandomPosition()
    {
        int randomCellX = Random.Range(0, grid.numCellsX * 2);
        int randomCellY = Random.Range(0, grid.numCellsY * 2);

        float townX = randomCellX * grid.gridCellSize + (grid.gridCellSize / 2f);
        float townY = randomCellY * grid.gridCellSize + (grid.gridCellSize / 2f);

        Vector2 townPosition = new Vector2(townX, townY);
        return townPosition;
    }

    private Vector2Int GetCellFromPosition(Vector2 position)
    {
        int cellX = Mathf.FloorToInt(position.x / grid.gridCellSize);
        int cellY = Mathf.FloorToInt(position.y / grid.gridCellSize);

        return new Vector2Int(cellX, cellY);
    }
}

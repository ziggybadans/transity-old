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
    [Min(2f)]
    public int minDistance = 2;
    public Settlement cityPrefab;
    public Settlement townPrefab;
    public Settlement ruralPrefab;
    public GameObject entityPrefab;
    private Camera cam;
    public GridManager grid;
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

        // Always place at least one city
        for (int i = 1; i < numTowns; i++)
        {
            if (i == 1)
            {
                Settlement city = Instantiate(cityPrefab, GetRandomPosition(), Quaternion.identity);
                city.Type = SettlementType.City;
                city.entityPrefab = entityPrefab;
                city.map = gameObject;
                settlements.Add(city);
            }
            else if (i > 1)
            {
                Vector2Int firstCityPos = GetCellFromPosition(settlements[0].gameObject.transform.position);
                for (int x = -grid.numCellsX; x <= grid.numCellsX; x++)
                {
                    for (int y = -grid.numCellsY; y <= grid.numCellsY; y++)
                    {
                        if (x == 0 && y == 0)
                        {
                            continue;
                        }

                        int r = Random.Range(0, 100);
                        float distance = Mathf.Sqrt(x * x + y * y);
                        float equation = (50f / Mathf.Pow(1f + Mathf.Exp(-0.5f * (distance - 10f)), 1f)) - 1f;
                        if (equation > r)
                        {
                            int targetX = firstCityPos.x + x;
                            int targetY = firstCityPos.y + y;

                            if (targetX >= 0 && targetX < grid.numCellsX && targetY >= 0 && targetY < grid.numCellsY)
                            {
                                Settlement city = Instantiate(cityPrefab, new Vector2(targetX, targetY), Quaternion.identity);
                                city.Type = SettlementType.City;
                                city.entityPrefab = entityPrefab;
                                city.map = gameObject;
                                settlements.Add(city);
                            }
                        }
                    }
                }
            }
            else
            {
                break;
            }
        }

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

using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Min(1)]
    public int numTriesCities = 1;
    [Min(1)]
    public int numTriesTowns = 3;
    [Min(1)]
    public int numTriesRural = 5;
    public Settlement cityPrefab;
    public Settlement townPrefab;
    public Settlement ruralPrefab;
    public GameObject passengerEntityPrefab;
    public GameObject textPrefab;
    public Transform gridParent;
    public GridManager grid;
    public ControlHandler controlHandler;
    private Camera cam;
    private int numCellsX, numCellsY;
    private List<Settlement> settlements = new();

    public List<Settlement> GetSettlements() { return settlements; }

    private void Start()
    {
        cam = Camera.main;

        numCellsX = grid.gridWidth;
        numCellsY = grid.gridHeight;

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
            float spawnProbability = currentCell.citySpawnProbability;

            float r = UnityEngine.Random.Range(0, 100) / 100f;
            if (spawnProbability > r && !currentCell.HasSettlement())
            {
                Settlement city = Instantiate(cityPrefab, currentCell.transform.position + (Vector3.forward * -2f), Quaternion.identity, currentCell.transform);
                city.Type = SettlementType.City;
                city.entityPrefab = passengerEntityPrefab;
                city.map = gameObject;
                settlements.Add(city);
                currentCell.settlement = city;
            }

            UpdateProbabilities();
        }

        for (int i = 1; i <= numTriesTowns; i++)
        {
            int x = UnityEngine.Random.Range(0, numCellsX - 1);
            int y = UnityEngine.Random.Range(0, numCellsY - 1);
            Cell currentCell = grid.GetCellFromPosition(new Vector2Int(x, y));
            float spawnProbability = currentCell.townSpawnProbability;

            float r = UnityEngine.Random.Range(0, 100) / 100f;
            if (spawnProbability > r && !currentCell.HasSettlement())
            {
                Settlement town = Instantiate(townPrefab, currentCell.transform.position + (Vector3.forward * -2f), Quaternion.identity, currentCell.transform);
                town.Type = SettlementType.RegularTown;
                town.entityPrefab = passengerEntityPrefab;
                town.map = gameObject;
                settlements.Add(town);
                currentCell.settlement = town;
            }

            UpdateProbabilities();
        }

        for (int i = 1; i <= numTriesTowns; i++)
        {
            int x = UnityEngine.Random.Range(0, numCellsX - 1);
            int y = UnityEngine.Random.Range(0, numCellsY - 1);
            Cell currentCell = grid.GetCellFromPosition(new Vector2Int(x, y));
            float spawnProbability = currentCell.ruralSpawnProbability;

            float r = UnityEngine.Random.Range(0, 100) / 100f;
            if (spawnProbability > r && !currentCell.HasSettlement())
            {
                Settlement rural = Instantiate(ruralPrefab, currentCell.transform.position + (Vector3.forward * -2f), Quaternion.identity, currentCell.transform);
                rural.Type = SettlementType.RuralTown;
                rural.entityPrefab = passengerEntityPrefab;
                rural.map = gameObject;
                settlements.Add(rural);
                currentCell.settlement = rural;
            }

            UpdateProbabilities();
        }
    }

    public void UpdateProbabilities()
    {
        bool debugMessageSent = false;
        for (int x = 0; x < numCellsX; x++)
        {
            for (int y = 0; y < numCellsY; y++)
            {
                Cell currentCell = grid.GetCellFromPosition(new Vector2Int(x, y));
                if (currentCell.HasSettlement())
                {
                    currentCell.citySpawnProbability = 0f;
                    currentCell.townSpawnProbability = 0f;
                    currentCell.ruralSpawnProbability = 0f;
                }
                else
                {
                    currentCell.citySpawnProbability = 1f;
                    currentCell.townSpawnProbability = 1f;
                    currentCell.ruralSpawnProbability = 1f;

                    foreach (Cell cell in grid.GetCells())
                    {
                        if (cell == currentCell)
                        {
                            continue;
                        }

                        if (cell.HasSettlement())
                        {
                            float distanceX = Mathf.Abs(currentCell.transform.position.x - cell.transform.position.x);
                            float distanceY = Mathf.Abs(currentCell.transform.position.y - cell.transform.position.y);
                            float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY) / grid.gridCellSize;

                            if (cell.settlement.Type == SettlementType.City)
                            {
                                float cityProbability = Mathf.Max(0, ((100f / Mathf.Pow(1f + Mathf.Exp(-0.5f * (distance - 10f)), 1f)) - 1f) / 100f);

                                if (cityProbability > 0)
                                {
                                    currentCell.citySpawnProbability *= cityProbability / 2;
                                }
                                else
                                {
                                    currentCell.citySpawnProbability *= cityProbability;
                                }

                                float townProbability = 25f;
                                if (distance < 4)
                                {
                                    townProbability = Mathf.Pow(distance + 1, 3);
                                }
                                else if (distance > 4 && distance < 15)
                                {
                                    townProbability = -1f * Mathf.Pow((0.5f * distance) - 2, 2) + 75;
                                }
                                if (currentCell.townSpawnProbability < 1f)
                                {
                                    currentCell.townSpawnProbability = Mathf.Max(0.25f, townProbability / 100f, currentCell.townSpawnProbability);
                                }
                                else
                                {
                                    currentCell.townSpawnProbability = Mathf.Max(0.25f, townProbability / 100f);
                                }
                            }
                            else if (cell.settlement.Type == SettlementType.RegularTown)
                            {
                                if (distance <= 3)
                                {
                                    float townProbability = Mathf.Max(0, (100f / Mathf.Pow(1f + Mathf.Exp(-0.5f * (distance - 10f)), 0.25f)) - 1f);
                                    currentCell.townSpawnProbability = Mathf.Min(townProbability / 100f, currentCell.townSpawnProbability);
                                    float cityProbability = Mathf.Max(0, ((100f / Mathf.Pow(1f + Mathf.Exp(-0.5f * (distance - 10f)), 0.25f)) - 1f) / 0.75f);
                                    currentCell.citySpawnProbability = Mathf.Min(cityProbability / 100f, currentCell.citySpawnProbability);
                                }
                            }
                            else if (cell.settlement.Type == SettlementType.RuralTown)
                            {

                            }
                        }
                        currentCell.ruralSpawnProbability = currentCell.citySpawnProbability * 2 * currentCell.townSpawnProbability;
                    }
                }

                int debugMode = controlHandler.GetDebugMode();
                float clampedProbability;
                if (debugMode == 1)
                {
                    clampedProbability = currentCell.citySpawnProbability * 2;
                    if (!debugMessageSent) Debug.Log("Debug mode: City Heatmap");
                    debugMessageSent = true;
                }
                else if (debugMode == 2)
                {
                    clampedProbability = currentCell.townSpawnProbability;
                    if (!debugMessageSent) Debug.Log("Debug mode: Town Heatmap");
                    debugMessageSent = true;
                }
                else if (debugMode == 3)
                {
                    clampedProbability = currentCell.ruralSpawnProbability;
                    if (!debugMessageSent) Debug.Log("Debug mode: Rural Heatmap");
                    debugMessageSent = true;
                }
                else
                {
                    clampedProbability = 0f;
                    if (!debugMessageSent) Debug.Log("Debug mode: Off");
                    debugMessageSent = true;
                }
                if (debugMode > 0)
                {
                    Color cellColor = Color.Lerp(Color.red, Color.green, clampedProbability);
                    currentCell.GetComponent<Renderer>().material.color = cellColor;
                }
                else
                {
                    currentCell.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
                }
            }
        }
    }
}

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

    private void Start()
    {
        cam = Camera.main;

        numCellsX = grid.gridWidth - 1;
        numCellsY = grid.gridHeight - 1;

        GenerateMap();
    }

    private void GenerateMap()
    {
        Debug.Log(numCellsX + "," + numCellsY);
        // Iterating through x coordinate of grid
        for (int x = 0; x <= numCellsX; x++)
        {
            // Iterating through y coordinate of grid
            for (int y = 0; y <= numCellsY; y++)
            {
                // This is the cell we're currently checking
                Vector2Int currentCell = new Vector2Int(x, y);

                float spawnProbability = 100;
                GameObject textInstance = Instantiate(textPrefab, new Vector3(GetCellFromPosition(currentCell).x, GetCellFromPosition(currentCell).y, -2f), Quaternion.identity, gridParent);
                TextMeshProUGUI textComponent = textInstance.GetComponent<TextMeshProUGUI>();
                textComponent.text = spawnProbability.ToString("F2");
            }
        }
    }

    private Vector2 GetRandomPosition()
    {
        int randomCellX = UnityEngine.Random.Range(0, numCellsX * 2);
        int randomCellY = UnityEngine.Random.Range(0, numCellsY * 2);

        Vector2 townPosition = GetCellFromPosition(new Vector2(randomCellX, randomCellY));
        return townPosition;
    }

    private Vector2 GetCellFromPosition(Vector2 position)
    {
        float cellX = position.x * grid.gridCellSize + (grid.gridCellSize / 2f);
        float cellY = position.y * grid.gridCellSize + (grid.gridCellSize / 2f);

        return new Vector2(cellX, cellY);
    }
}

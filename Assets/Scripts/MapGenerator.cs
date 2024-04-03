using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth = 100;
    public int mapHeight = 100;
    public int townCount = 10;
    public float townSize = 1f;
    public float minTownDistance = 5f;

    public GameObject townPrefab;
    public Camera mainCamera;

    private Vector2[] GenerateTownPositions()
    {
        HashSet<Vector2> positions = new HashSet<Vector2>();
        while (positions.Count < townCount)
        {
            Vector2 newPosition;
            bool isValidPosition;
            do
            {
                float x = Random.Range(0, mapWidth);
                float y = Random.Range(0, mapHeight);

                newPosition = new Vector2(x, y);
                isValidPosition = true;
                foreach (Vector2 existingPosition in positions)
                {
                    if (Vector2.Distance(newPosition, existingPosition) < minTownDistance)
                    {
                        isValidPosition = false;
                        break;
                    }
                }
            } while (!isValidPosition);
            if (!positions.Contains(newPosition))
            {
                GameObject town = Instantiate(townPrefab, newPosition, Quaternion.identity);
                town.transform.localScale = Vector3.one * townSize;
                positions.Add(newPosition);
            }
        }
        return positions.ToArray();
    }

    private void Start()
    {
        Vector2[] townPositions = GenerateTownPositions();

        Vector2 minBounds = townPositions[0];
        Vector2 maxBounds = townPositions[0];
        foreach (Vector2 position in townPositions)
        {
            minBounds = Vector2.Min(minBounds, position);
            maxBounds = Vector2.Max(maxBounds, position);

        }

        Vector2 mapCenter = (minBounds + maxBounds) / 2f;

        float mapWidth = maxBounds.x - minBounds.x;
        float mapHeight = maxBounds.y - minBounds.y;
        float aspectRatio = (float)Screen.width / Screen.height;
        float orthoSize = Mathf.Max(mapWidth / aspectRatio, mapHeight) / 2f;

        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(mapCenter.x, mapCenter.y, mainCamera.transform.position.z);
            mainCamera.orthographicSize = orthoSize * 1.1f;
        }
        else
        {
            Debug.LogError("Main Camera is not assigned!");
        }
    }

    private void OnValidate() {
        mapWidth = Mathf.Max(1, mapWidth);
        mapHeight = Mathf.Max(1, mapHeight);
        townCount = Mathf.Max(1, townCount);
        townSize = Mathf.Max(0.1f, townSize);
        minTownDistance = Mathf.Max(0f, minTownDistance);
    }
}

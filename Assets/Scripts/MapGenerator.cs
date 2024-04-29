using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int minTowns = 3;
    public int maxTowns = 5;
    public float minDistanceBetweenTowns = 2f;
    public float mapWidth = 10f;
    public float mapHeight = 10f;
    public Settlement cityPrefab;
    public Settlement townPrefab;
    public Settlement ruralPrefab;
    public GameObject entityPrefab;
    private Camera cam;
    public List<Settlement> settlements = new List<Settlement>();

    private void Start()
    {
        cam = Camera.main;

        GenerateMap();
    }

    private void GenerateMap()
    {
        int numTowns = Random.Range(minTowns, maxTowns);

        // Ensure there is at least one city
        Settlement city = Instantiate(cityPrefab, GetRandomPosition(), Quaternion.identity);
        city.Type = SettlementType.City;
        city.entityPrefab = entityPrefab;
        city.map = gameObject;
        settlements.Add(city);

        // Generate the remaining towns
        for (int i = 1; i < numTowns; i++)
        {
            if (i <= 2 && Random.value < 0.5f)
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
                SettlementType type = SettlementType.RuralTown;
                Settlement ruralTown = Instantiate(ruralPrefab, GetRandomPosition(), Quaternion.identity);
                ruralTown.Type = type;
                ruralTown.entityPrefab = entityPrefab;
                ruralTown.map = gameObject;
                settlements.Add(ruralTown);
            }
        }

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

        // Calculate the bounds of the map
        Bounds mapBounds = new Bounds(settlements[0].transform.position, Vector3.zero);
        foreach (Settlement settlement in settlements)
        {
            mapBounds.Encapsulate(settlement.transform.position);
        }

        // Calculate the centre of the map
        Vector2 mapCenter = mapBounds.center;

        // Adjust the camera position to center the map
        Vector3 cameraPosition = cam.transform.position;
        cameraPosition.x = mapCenter.x;
        cameraPosition.y = mapCenter.y;
        cam.transform.position = cameraPosition;

        // Adjust the camera orthographic size to fit the map
        float aspectRatio = (float)Screen.width / Screen.height;
        float mapWidth = mapBounds.size.x * 1.3f;
        float mapHeight = mapBounds.size.y * 1.3f;
        float requiredSize = Mathf.Max(mapWidth / aspectRatio, mapHeight) * 0.5f;
        cam.orthographicSize = Mathf.Max(requiredSize, cam.orthographicSize);
    }

    private Vector2 GetRandomPosition()
    {
        float x = Random.Range(0f, mapWidth);
        float y = Random.Range(0f, mapHeight);
        return new Vector2(x, y);
    }
}

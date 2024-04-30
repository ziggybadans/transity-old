using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Range(5, 15)]
    public int minTowns = 10;
    [Range(10, 30)]
    public int maxTowns = 20;
    [Min(2f)]
    public float minDistanceBetweenTowns = 2f;
    [Range(10f, 100f)]
    public float mapWidth = 10f;
    [Range(10f, 100f)]
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
        SetupView();
    }

    private void GenerateMap()
    {
        int numTowns = Random.Range(minTowns, maxTowns);

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
            else if (Random.value < (0.5f / (i - 1)))
            {
                Settlement city = Instantiate(cityPrefab, GetRandomPosition(), Quaternion.identity);
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
    }

    private void SetupView()
    {
        // Calculate the bounds of the map
        Bounds mapBounds = new(settlements[0].transform.position, Vector3.zero);
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
    }

    private Vector2 GetRandomPosition()
    {
        float x = Random.Range(0f, mapWidth);
        float y = Random.Range(0f, mapHeight);
        return new Vector2(x, y);
    }
}

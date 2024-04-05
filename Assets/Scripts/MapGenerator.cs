using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Vector2 mapSize = new Vector2(10, 10);
    public Vector2Int numCities = new Vector2Int(2, 4);
    public Vector2Int numTowns = new Vector2Int(1, 3);
    public Vector2Int numRural = new Vector2Int(5, 15);
    public float clumpingFactor = 0.5f;

    public GameObject townPrefab;
    public GameObject cityPrefab;
    public GameObject ruralPrefab;
    public Camera mainCamera;

    [System.Serializable]
    public class Town
    {
        public Vector2 position;
        public TownSize size;
    }

    public enum TownSize
    {
        Rural, Town, City
    }

    public List<Town> Generate()
    {
        List<Town> towns = new List<Town>();

        int cityCount = UnityEngine.Random.Range(numCities.x, numCities.y);
        for (int i = 0; i < cityCount; i++)
        {
            Vector2 cityPos = new Vector2(UnityEngine.Random.Range(-mapSize.x / 2f, mapSize.x / 2f), UnityEngine.Random.Range(-mapSize.y / 2f, mapSize.y / 2f));

            Town city = new Town()
            {
                position = cityPos,
                size = TownSize.City
            };
            towns.Add(city);

            int adjacentCount = Random.Range(numTowns.x, numTowns.y);
            for (int j = 0; j < adjacentCount; j++)
            {
                Vector2 adjacentPos = cityPos + Random.insideUnitCircle * clumpingFactor;

                Town adjacentTown = new Town()
                {
                    position = adjacentPos,
                    size = TownSize.Town
                };
                towns.Add(adjacentTown);
            }
        }

        int ruralCount = UnityEngine.Random.Range(numRural.x, numRural.y);
        for (int i = 0; i < ruralCount; i++)
        {
            Vector2 ruralPos = new Vector2(UnityEngine.Random.Range(-mapSize.x / 2f, mapSize.x / 2f), UnityEngine.Random.Range(-mapSize.y / 2f, mapSize.y / 2f));

            Town ruralTown = new Town()
            {
                position = ruralPos,
                size = TownSize.Rural
            };
            towns.Add(ruralTown);
        }

        return towns;
    }

    public void SpawnTowns(List<Town> towns)
    {
        foreach (Town town in towns)
        {
            if (town.size == TownSize.Town)
            {
                Instantiate(townPrefab, new Vector3(town.position.x, town.position.y, 0), Quaternion.identity, transform);
            }
            else if (town.size == TownSize.City)
            {
                Instantiate(cityPrefab, new Vector3(town.position.x, town.position.y, 0), Quaternion.identity, transform);
            }
            else if (town.size == TownSize.Rural)
            {
                Instantiate(ruralPrefab, new Vector3(town.position.x, town.position.y, 0), Quaternion.identity, transform);
            }
        }
    }

    public Bounds CalculateBounds(List<Town> towns)
    {
        if (towns.Count == 0) return new Bounds(Vector3.zero, Vector3.zero);

        Vector3 minBounds = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 maxBounds = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        foreach (Town town in towns)
        {
            Vector3 pos = new Vector3(town.position.x, town.position.y, 0);
            minBounds = Vector3.Min(minBounds, pos);
            maxBounds = Vector3.Max(maxBounds, pos);
        }

        Vector3 center = (minBounds + maxBounds) / 2;
        Vector3 size = maxBounds - minBounds;
        return new Bounds(center, size);
    }

    public void AdaptCamera(Bounds bounds)
    {
        float aspect = (float)Screen.width / Screen.height;

        float size = Mathf.Max(bounds.size.x / aspect, bounds.size.y) / 2;

        mainCamera.transform.position = new Vector3(bounds.center.x, bounds.center.y, -10);
        mainCamera.orthographicSize = size * 1.1f;
    }

    private void Start()
    {
        List<Town> towns = Generate();
        SpawnTowns(towns);
        Bounds bounds = CalculateBounds(towns);
        AdaptCamera(bounds);
    }
}

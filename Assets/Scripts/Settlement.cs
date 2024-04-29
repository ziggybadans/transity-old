using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public enum SettlementType
{
    City,
    RegularTown,
    RuralTown
}

public class Settlement : MonoBehaviour
{
    public GameObject entityPrefab;
    public GameObject map;
    public float spawnInterval = 3f;
    public List<Passenger> passengers = new();
    public SettlementType Type { get; set; }

    private void Start()
    {
        StartCoroutine(SpawnEntitiesCoroutine());
    }

    private IEnumerator SpawnEntitiesCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEntity();
        }
    }

    private void SpawnEntity()
    {
        Vector2 spawnPosition = CalculateSpawnPosition();
        GameObject newEntity = Instantiate(entityPrefab, spawnPosition, Quaternion.identity);
        newEntity.transform.position = new Vector3(newEntity.transform.position.x, newEntity.transform.position.y, -3f);

        Passenger newPassenger = SetupNewPassenger(newEntity);
        SetPassengerSprite(newPassenger);
        SetPassengerOpacity(newPassenger);

        passengers.Add(newPassenger);
    }

    private Vector2 CalculateSpawnPosition()
    {
        Vector2 spawnPosition = transform.position;
        int passengerCount = passengers.Count;

        if (passengerCount == 0)
        {
            spawnPosition += (Vector2.left * GetComponent<SpriteRenderer>().bounds.size.x) + (Vector2.up * 0.5f);
        }
        else
        {
            Passenger lastPassenger = passengers[passengerCount - 1];
            spawnPosition = lastPassenger.transform.position + (Vector3.left * lastPassenger.GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);

            if (passengerCount == 5)
            {
                Vector2 originalSpawnPos = transform.position;
                spawnPosition = originalSpawnPos + (Vector2.left * GetComponent<SpriteRenderer>().bounds.size.x) + (Vector2.down * 0.1f);
            }
        }

        return spawnPosition;
    }

    private Passenger SetupNewPassenger(GameObject newEntity)
    {
        Passenger newPassenger = newEntity.GetComponent<Passenger>();
        newPassenger.origin = this;

        List<Settlement> settlements = map.GetComponent<MapGenerator>().settlements;
        Settlement randomSettlement = settlements[UnityEngine.Random.Range(0, settlements.Count)];
        while (randomSettlement == newPassenger.origin)
        {
            randomSettlement = settlements[UnityEngine.Random.Range(0, settlements.Count)];
        }
        newPassenger.destination = randomSettlement;

        return newPassenger;
    }

    private void SetPassengerSprite(Passenger passenger)
    {
        if (passenger.destination.Type == SettlementType.City)
        {
            passenger.GetComponent<SpriteRenderer>().sprite = passenger.citySprite;
        }
        else if (passenger.destination.Type == SettlementType.RegularTown)
        {
            passenger.GetComponent<SpriteRenderer>().sprite = passenger.townSprite;
        }
        else if (passenger.destination.Type == SettlementType.RuralTown)
        {
            passenger.GetComponent<SpriteRenderer>().sprite = passenger.ruralSprite;
        }
    }

    private void SetPassengerOpacity(Passenger passenger)
    {
        int passengerCount = passengers.Count;
        float opacity = 1f;

        if (passengerCount >= 7)
        {
            opacity = Mathf.Clamp01(1f - ((passengerCount - 6) * 0.25f));
        }

        passenger.GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, opacity);
    }

    public Passenger AlightPassenger()
    {
        if (passengers.Count > 0)
        {
            Passenger passengerAlighting = passengers[0];
            passengerAlighting.gameObject.SetActive(false);
            passengers.Remove(passengerAlighting);

            for (int i = 0; i < passengers.Count; i++)
            {
                Passenger currentPassenger = passengers[i];
                Vector3 newPosition = CalculatePassengerPosition(i);
                currentPassenger.transform.position = newPosition;
                SetPassengerOpacity(currentPassenger);
            }

            return passengerAlighting;
        }

        return null;
    }

    private Vector3 CalculatePassengerPosition(int index)
    {
        if (index == 0)
        {
            return transform.position + (Vector3.left * GetComponent<SpriteRenderer>().bounds.size.x) + (Vector3.up * 0.5f);
        }
        else if (index < 5)
        {
            return passengers[index - 1].transform.position + (Vector3.left * passengers[index - 1].GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);
        }
        else if (index == 5)
        {
            return transform.position + (Vector3.left * GetComponent<SpriteRenderer>().bounds.size.x) + (Vector3.down * 0.1f);
        }
        else
        {
            return passengers[index - 1].transform.position + (Vector3.left * passengers[index - 1].GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);
        }
    }

    public int GetPassengersWaiting()
    {
        return passengers.Count;
    }
}
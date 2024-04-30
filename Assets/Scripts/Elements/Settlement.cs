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
        //StartCoroutine(SpawnEntitiesCoroutine());
    }

    private void Update()
    {
        SetPassengerOpacity();
        CalculatePosition();
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
        GameObject newEntity = Instantiate(entityPrefab, new Vector3(transform.position.x, transform.position.y, -3f), Quaternion.identity);

        Passenger newPassenger = SetupNewPassenger(newEntity);
        SetPassengerSprite(newPassenger);

        passengers.Add(newPassenger);
    }

    private void CalculatePosition()
    {
        for (int i = 0; i < passengers.Count; i++)
        {
            if (i == 0)
            {
                passengers[i].gameObject.transform.position = new Vector2(transform.position.x, transform.position.y) + (Vector2.left * GetComponent<SpriteRenderer>().bounds.size.x) + (Vector2.up * 0.5f);
            }
            else
            {
                passengers[i].gameObject.transform.position = passengers[i - 1].transform.position + (Vector3.left * passengers[i - 1].GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);
                if (i == 5)
                {
                    passengers[i].gameObject.transform.position = new Vector2(transform.position.x, transform.position.y) + (Vector2.left * GetComponent<SpriteRenderer>().bounds.size.x) + (Vector2.down * 0.1f);
                }
            }
        }
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

    private void SetPassengerOpacity()
    {
        int passengerCount = passengers.Count;
        if (passengerCount <= 10)
        {
            for (int i = 0; i < passengerCount; i++)
            {
                passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            }
        }
        else if (passengerCount == 11)
        {
            for (int i = 0; i < 9; i++)
            {
                passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            }
            passengers[9].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.75f);
            for (int i = 10; i < passengerCount; i++)
            {
                passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0f);
            }
        }
        else if (passengerCount == 12)
        {
            for (int i = 0; i < 8; i++)
            {
                passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            }
            passengers[9].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.5f);
            passengers[8].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.75f);
            for (int i = 10; i < passengerCount; i++)
            {
                passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0f);
            }
        }
        else if (passengerCount >= 13)
        {
            for (int i = 0; i < 7; i++)
            {
                passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            }
            passengers[9].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.25f);
            passengers[8].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.5f);
            passengers[7].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.75f);
            for (int i = 10; i < passengerCount; i++)
            {
                passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0f);
            }
        }
    }

    public Passenger AlightPassenger(Transport transport)
    {
        if (passengers.Count > 0)
        {
            int i = 0;
            while (i < passengers.Count)
            {
                if (passengers[i].destination == transport.startTown || passengers[i].destination == transport.endTown)
                {
                    Passenger passengerAlighting = passengers[i];
                    passengers.Remove(passengerAlighting);
                    return passengerAlighting;
                }
                else
                {
                    i++;
                }
            }
        }

        return null;
    }

    public int GetPassengersWaiting()
    {
        return passengers.Count;
    }
}
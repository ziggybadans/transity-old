using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SettlementType { City, Town, Rural }

public class Settlement : MonoBehaviour
{
    public Cell parentCell;
    public GameObject entityPrefab;
    public GameObject map;
    [Min(0.1f)]
    private readonly float spawnInterval = 3f;
    private List<Passenger> passengers = new();
    public SettlementType Type { get; set; }

    private void Start()
    {
        StartCoroutine(SpawnEntitiesCoroutine());
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
            passengers[i].gameObject.transform.position = i switch
            {
                0 => (Vector3)(GetPassengerInitialPos() + (Vector2.up * 0.5f)),
                5 => (Vector3)(GetPassengerInitialPos() + (Vector2.down * 0.1f)),
                _ => passengers[i - 1].transform.position +
                        (1.3f * passengers[i - 1].GetComponent<SpriteRenderer>().bounds.size.x * Vector3.left),
            };
        }
    }

    private Vector2 GetPassengerInitialPos() {
        Vector2 vector2 = new Vector2(transform.position.x, transform.position.y) + (Vector2.left * GetComponent<SpriteRenderer>().bounds.size.x);
        return vector2;
    }

    private Passenger SetupNewPassenger(GameObject newEntity)
    {
        Passenger newPassenger = newEntity.GetComponent<Passenger>();
        newPassenger.origin = this;

        List<Settlement> settlements = new();
        foreach (Cell cell in GridManager.Instance.GetCells()) {
            if (cell.Settlement != null) settlements.Add(cell.Settlement);
        }
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
        passenger.GetComponent<SpriteRenderer>().sprite = passenger.destination.Type switch
        {
            SettlementType.City => GameManager.Instance.CitySettlementSprite,
            SettlementType.Town => GameManager.Instance.TownSettlementSprite,
            SettlementType.Rural => GameManager.Instance.RuralSettlementSprite,
            _ => GameManager.Instance.RuralSettlementSprite,
        };
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
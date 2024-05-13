using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SettlementType { City, Town, Rural }

public class Settlement : MonoBehaviour
{
    public Cell ParentCell;
    public GameObject EntityPrefab;
    public GameObject Map;
    [Min(0.1f)]
    private readonly float r_spawnInterval = 3f;
    private List<Passenger> _passengers = new();
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
            yield return new WaitForSeconds(r_spawnInterval);
            SpawnEntity();
        }
    }

    private void SpawnEntity()
    {
        GameObject newEntity = Instantiate(EntityPrefab, new Vector3(transform.position.x, transform.position.y, -3f), Quaternion.identity);

        Passenger newPassenger = SetupNewPassenger(newEntity);
        SetPassengerSprite(newPassenger);

        _passengers.Add(newPassenger);
    }

    private void CalculatePosition()
    {
        for (int i = 0; i < _passengers.Count; i++)
        {
            _passengers[i].gameObject.transform.position = i switch
            {
                0 => (Vector3)(GetPassengerInitialPos() + (Vector2.up * 0.5f)),
                5 => (Vector3)(GetPassengerInitialPos() + (Vector2.down * 0.1f)),
                _ => _passengers[i - 1].transform.position +
                        (1.3f * _passengers[i - 1].GetComponent<SpriteRenderer>().bounds.size.x * Vector3.left),
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
        newPassenger.Origin = this;

        List<Settlement> settlements = new();
        foreach (Cell cell in GridManager.Instance.GetCells()) {
            if (cell.Settlement != null) settlements.Add(cell.Settlement);
        }
        Settlement randomSettlement = settlements[UnityEngine.Random.Range(0, settlements.Count)];
        while (randomSettlement == newPassenger.Origin)
        {
            randomSettlement = settlements[UnityEngine.Random.Range(0, settlements.Count)];
        }
        newPassenger.Destination = randomSettlement;

        return newPassenger;
    }

    private void SetPassengerSprite(Passenger passenger)
    {
        passenger.GetComponent<SpriteRenderer>().sprite = passenger.Destination.Type switch
        {
            SettlementType.City => GameManager.Instance.CitySettlementSprite,
            SettlementType.Town => GameManager.Instance.TownSettlementSprite,
            SettlementType.Rural => GameManager.Instance.RuralSettlementSprite,
            _ => GameManager.Instance.RuralSettlementSprite,
        };
    }

    private void SetPassengerOpacity()
    {
        int passengerCount = _passengers.Count;
        if (passengerCount <= 10)
        {
            for (int i = 0; i < passengerCount; i++)
            {
                _passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            }
        }
        else if (passengerCount == 11)
        {
            for (int i = 0; i < 9; i++)
            {
                _passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            }
            _passengers[9].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.75f);
            for (int i = 10; i < passengerCount; i++)
            {
                _passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0f);
            }
        }
        else if (passengerCount == 12)
        {
            for (int i = 0; i < 8; i++)
            {
                _passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            }
            _passengers[9].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.5f);
            _passengers[8].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.75f);
            for (int i = 10; i < passengerCount; i++)
            {
                _passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0f);
            }
        }
        else if (passengerCount >= 13)
        {
            for (int i = 0; i < 7; i++)
            {
                _passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            }
            _passengers[9].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.25f);
            _passengers[8].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.5f);
            _passengers[7].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.75f);
            for (int i = 10; i < passengerCount; i++)
            {
                _passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0f);
            }
        }
    }

    public Passenger AlightPassenger(Transport transport)
    {
        if (_passengers.Count > 0)
        {
            int i = 0;
            while (i < _passengers.Count)
            {
                if (_passengers[i].Destination == transport._startTown || _passengers[i].Destination == transport._endTown)
                {
                    Passenger passengerAlighting = _passengers[i];
                    _passengers.Remove(passengerAlighting);
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
        return _passengers.Count;
    }
}
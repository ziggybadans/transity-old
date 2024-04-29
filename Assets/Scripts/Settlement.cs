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
        StartCoroutine("SpawnEntitiesCoroutine");
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
        Vector2 spawnPosition = transform.position;
        bool lineCutoff = false;

        if (passengers.Count == 0)
        {
            spawnPosition += (Vector2.left * GetComponent<SpriteRenderer>().bounds.size.x) + (Vector2.up * 0.5f);
        }
        if (passengers.Count > 0 && passengers.Count < 5)
        {
            Passenger lastEntity = passengers[passengers.Count - 1];
            spawnPosition = lastEntity.gameObject.transform.position + (Vector3.left *
                lastEntity.GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);
        }
        if (passengers.Count == 5)
        {
            spawnPosition += Vector2.left * GetComponent<SpriteRenderer>().bounds.size.x + (Vector2.down * 0.1f);
        }
        if (passengers.Count > 5)
        {
            Passenger lastEntity = passengers[passengers.Count - 1];
            spawnPosition = lastEntity.gameObject.transform.position + (Vector3.left *
                lastEntity.GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);
        }
        if (passengers.Count > 6)
        {
            Passenger lastEntity = passengers[passengers.Count - 1];
            spawnPosition = lastEntity.gameObject.transform.position + (Vector3.left *
                lastEntity.GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);
            lineCutoff = true;
        }

        GameObject newEntity = Instantiate(entityPrefab, spawnPosition, Quaternion.identity);
        Vector3 currentPos = newEntity.transform.position;
        currentPos.z = -3f;
        newEntity.transform.position = currentPos;
        Passenger newPassenger = newEntity.GetComponent<Passenger>();
        newPassenger.origin = this;
        List<Settlement> settlements = map.GetComponent<MapGenerator>().settlements;
        while (newPassenger.destination == null)
        {
            int randomValue = Mathf.RoundToInt(UnityEngine.Random.Range(0, settlements.Count - 1));
            if (settlements[randomValue] == newPassenger.origin)
            {
                if (randomValue >= settlements.Count)
                {
                    newPassenger.destination = settlements[randomValue - 1];
                }
                else
                {
                    newPassenger.destination = settlements[randomValue + 1];
                }
            }
            else
            {
                newPassenger.destination = settlements[randomValue];
            }
        }
        if (newPassenger.destination.Type == SettlementType.City)
        {
            newPassenger.GetComponent<SpriteRenderer>().sprite = newPassenger.citySprite;
        }
        else if (newPassenger.destination.Type == SettlementType.RegularTown)
        {
            newPassenger.GetComponent<SpriteRenderer>().sprite = newPassenger.townSprite;
        }
        else if (newPassenger.destination.Type == SettlementType.RuralTown)
        {
            newPassenger.GetComponent<SpriteRenderer>().sprite = newPassenger.ruralSprite;
        }

        if (lineCutoff)
        {
            if (passengers.Count == 7)
            {
                newEntity.GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.75f);
            }
            else if (passengers.Count == 8)
            {
                newEntity.GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.5f);
            }
            else if (passengers.Count == 9)
            {
                newEntity.GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.25f);
            }
            else
            {
                newEntity.GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0f);
            }
        }
        else
        {
            newEntity.GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
        }
        passengers.Add(newEntity.GetComponent<Passenger>());
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
                if (i == 0)
                {
                    passengers[i].gameObject.transform.position = transform.position +
                        (Vector3.left * GetComponent<SpriteRenderer>().bounds.size.x) + (Vector3.up * 0.5f);
                    passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
                }
                else if (i > 0 && i < 5)
                {
                    passengers[i].gameObject.transform.position =
                        passengers[i - 1].gameObject.transform.position +
                        (Vector3.left * passengers[i - 1].GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);
                    passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
                }
                else if (i == 5)
                {
                    passengers[i].gameObject.transform.position = transform.position +
                        (Vector3.left * GetComponent<SpriteRenderer>().bounds.size.x) + (Vector3.down * 0.1f);
                    passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
                }
                else if (i == 6)
                {
                    passengers[i].gameObject.transform.position =
                        passengers[i - 1].gameObject.transform.position +
                        (Vector3.left * passengers[i - 1].GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);
                    passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
                }
                else if (i == 7)
                {
                    passengers[i].gameObject.transform.position =
                        passengers[i - 1].gameObject.transform.position +
                        (Vector3.left * passengers[i - 1].GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);
                    passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.75f);
                }
                else if (i == 8)
                {
                    passengers[i].gameObject.transform.position =
                        passengers[i - 1].gameObject.transform.position +
                        (Vector3.left * passengers[i - 1].GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);
                    passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.5f);
                }
                else if (i == 9)
                {
                    passengers[i].gameObject.transform.position =
                        passengers[i - 1].gameObject.transform.position +
                        (Vector3.left * passengers[i - 1].GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);
                    passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.25f);
                }
                else if (i > 9)
                {
                    passengers[i].gameObject.transform.position =
                        passengers[i - 1].gameObject.transform.position +
                        (Vector3.left * passengers[i - 1].GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);
                    passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0f);
                }
            }
            return passengerAlighting;
        }
        return null;
    }

    public int GetPassengersWaiting()
    {
        return passengers.Count;
    }
}
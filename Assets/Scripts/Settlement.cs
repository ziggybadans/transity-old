using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    public GameObject entityPrefab;
    public float spawnInterval = 5f;

    public List<Passenger> passengers = new();

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

        if (passengers.Count > 0)
        {
            Passenger lastEntity = passengers[passengers.Count - 1];
            spawnPosition = lastEntity.transform.position + (Vector3.left *
                lastEntity.GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);
        }
        else
        {
            spawnPosition += (Vector2.left * GetComponent<SpriteRenderer>().bounds.size.x) + (Vector2.up * 0.5f);
        }

        GameObject newEntity = Instantiate(entityPrefab, spawnPosition, Quaternion.identity);
        Passenger newPassenger = newEntity.GetComponent<Passenger>();
        newPassenger.origin = this;
        passengers.Add(newEntity.GetComponent<Passenger>());
    }

    public Passenger AlightPassenger()
    {
        if (passengers.Count > 0)
        {
            Passenger passenger = passengers[0];
            passengers.RemoveAt(0);
            passenger.gameObject.SetActive(false);
            return passenger;
        }
        return null;
    }

    public int GetPassengersWaiting()
    {
        return passengers.Count;
    }
}

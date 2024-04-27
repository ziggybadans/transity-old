using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Settlement : MonoBehaviour
{
    public GameObject entityPrefab;
    public Text textComponent;
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
            spawnPosition = lastEntity.gameObject.transform.position + (Vector3.left *
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
            Passenger passengerAlighting = passengers[0];
            passengerAlighting.gameObject.SetActive(false);
            passengers.Remove(passengerAlighting);
            for (int i = 0; i < passengers.Count; i++) {
                if (i == 0) {
                    passengers[i].gameObject.transform.position = transform.position +
                        (Vector3.left * GetComponent<SpriteRenderer>().bounds.size.x) + (Vector3.up * 0.5f);
                } else {
                    passengers[i].gameObject.transform.position = 
                        passengers[i - 1].gameObject.transform.position + 
                        (Vector3.left * passengers[i - 1].GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);
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

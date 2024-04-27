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

        GameObject newEntity = Instantiate(entityPrefab, spawnPosition, Quaternion.identity);
        newEntity.SetActive(false);
        Passenger newPassenger = newEntity.GetComponent<Passenger>();
        newPassenger.origin = this;
        passengers.Add(newEntity.GetComponent<Passenger>());
        textComponent.text = passengers.Count.ToString();
    }

    public Passenger AlightPassenger()
    {
        if (passengers.Count > 0)
        {
            Passenger passengerAlighting = passengers[0];
            passengers.Remove(passengerAlighting);
            textComponent.text = passengers.Count.ToString();
            return passengerAlighting;
        }
        return null;
    }

    public int GetPassengersWaiting()
    {
        return passengers.Count;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public GameObject entityPrefab;
    public float spawnInterval = 5f;

    private List<GameObject> entities = new();

    private void Start() {
        StartCoroutine("SpawnEntitiesCoroutine");
    }

    private IEnumerator SpawnEntitiesCoroutine() {
        while (true) {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEntity();
        }
    }

    private void SpawnEntity() {
        Vector2 spawnPosition = transform.position;

        if (entities.Count > 0) {
            GameObject lastEntity = entities[entities.Count - 1];
            spawnPosition = lastEntity.transform.position + (Vector3.left * 
                lastEntity.GetComponent<SpriteRenderer>().bounds.size.x * 1.3f);
        } else {
            spawnPosition += (Vector2.left * GetComponent<SpriteRenderer>().bounds.size.x) + (Vector2.up * 0.5f);
        }

        GameObject newEntity = Instantiate(entityPrefab, spawnPosition, Quaternion.identity);
        entities.Add(newEntity);
    }
}

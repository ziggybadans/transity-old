using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    private Settlement startTown, endTown;
    private List<GameObject> entities = new();
    private GameObject entityPrefab;
    private float ENTITY_SPEED = 1f;
    private int CAPACITY = 6;
    private int NUM_ENTITIES = 1;

    public void SetupConnection(Settlement startTown, Settlement endTown)
    {
        this.startTown = startTown;
        this.endTown = endTown;

        StartCoroutine(SpawnEntitiesCoroutine(this.startTown, this.endTown));
    }

    private IEnumerator SpawnEntitiesCoroutine(Settlement startTown, Settlement endTown)
    {
        Vector3 startPos = GetPosFromSettlement(startTown);
        Vector3 endPos = GetPosFromSettlement(endTown);

        Vector3 direction = (endPos - startPos).normalized;
        float lineLength = Vector3.Distance(startPos, endPos);
        float spacing = lineLength / (NUM_ENTITIES + 1);

        for (int i = 1; i <= NUM_ENTITIES; i++)
        {
            Vector3 spawnPos = startPos + (i * spacing * direction);
            spawnPos.z = -2f;
            GameObject entity = Instantiate(entityPrefab, spawnPos, Quaternion.identity);
            entities.Add(entity);

            SetupEntity(entity.GetComponent<Transport>(), i, startTown, endTown);

            yield return new WaitForSeconds(0.1f);
        }
    }

    private Vector3 GetPosFromSettlement(Settlement settlement) {
        return settlement.parentCell.transform.position;
    }

    private void SetupEntity(Transport entity, int i, Settlement startTown, Settlement endTown) {
        entity.movingForward = i % 2 != 0;
        entity.startTown = startTown;
        entity.endTown = endTown;
        entity.entitySpeed = ENTITY_SPEED;
        entity.capacity = CAPACITY;
    }

    public void DestroyAllEntities() {
        foreach (GameObject entity in entities) {
            Destroy(entity);
        }
        entities.Clear();
    }
}

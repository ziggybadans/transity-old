using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    private Vector2 startPos, endPos;
    private Settlement startTown, endTown;
    private int numEntites;
    private List<GameObject> entities = new();
    private GameObject entityPrefab;

    public void SetupConnection(Vector2 startPos, Vector2 endPos, Settlement startTown, Settlement endTown, int numEntites, GameObject entityPrefab)
    {
        this.startPos = startPos;
        this.endPos = endPos;
        this.startTown = startTown;
        this.endTown = endTown;
        this.numEntites = numEntites;
        this.entityPrefab = entityPrefab;

        StartCoroutine(SpawnEntitiesCoroutine(startPos, endPos));
    }

    private IEnumerator SpawnEntitiesCoroutine(Vector3 startPos, Vector3 endPos)
    {
        Vector3 direction = (endPos - startPos).normalized;
        float lineLength = Vector3.Distance(startPos, endPos);
        float spacing = lineLength / (numEntites + 1);

        for (int i = 1; i <= numEntites; i++)
        {
            Vector3 spawnPos = startPos + (direction * spacing * i);
            GameObject entity = Instantiate(entityPrefab, spawnPos, Quaternion.identity);
            entities.Add(entity);
            if (i % 2 == 0)
            {
                entity.GetComponent<Transport>().movingForward = false;
            }
            else
            {
                entity.GetComponent<Transport>().movingForward = true;
            }
            entity.GetComponent<Transport>().startPos = startPos;
            entity.GetComponent<Transport>().endPos = endPos;

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void DestroyAllEntities() {
        foreach (GameObject entity in entities) {
            Destroy(entity);
        }
        entities.Clear();
    }
}

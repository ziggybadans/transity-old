using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    private Settlement _startTown, _endTown;
    private List<GameObject> _entities = new();
    private GameObject _entityPrefab;
    private float ENTITY_SPEED = 1f;
    private int CAPACITY = 6;
    private int NUM_ENTITIES = 1;

    public void SetupConnection(Settlement startTown, Settlement endTown)
    {
        this._startTown = startTown;
        this._endTown = endTown;

        StartCoroutine(SpawnEntitiesCoroutine(this._startTown, this._endTown));
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
            GameObject entity = Instantiate(_entityPrefab, spawnPos, Quaternion.identity);
            _entities.Add(entity);

            SetupEntity(entity.GetComponent<Transport>(), i, startTown, endTown);

            yield return new WaitForSeconds(0.1f);
        }
    }

    private Vector3 GetPosFromSettlement(Settlement settlement) {
        return settlement.ParentCell.transform.position;
    }

    private void SetupEntity(Transport entity, int i, Settlement startTown, Settlement endTown) {
        entity.MovingForward = i % 2 != 0;
        entity._startTown = startTown;
        entity._endTown = endTown;
        entity.EntitySpeed = ENTITY_SPEED;
        entity.Capacity = CAPACITY;
    }

    public void DestroyAllEntities() {
        foreach (GameObject entity in _entities) {
            Destroy(entity);
        }
        _entities.Clear();
    }
}

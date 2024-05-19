using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportSpawning : MonoBehaviour, ISpawner
{
    private Connection connection;
    private void OnEnable() {
        connection = GetComponent<Connection>();
        connection.OnSpawningStart += Spawn;
    }

    private void OnDisable() {
        connection.OnSpawningStart -= Spawn;
    }

    public void Spawn()
    {
        Settlement[] settlements = connection.Towns;
        Vector3 startPos = GetPosFromSettlement(settlements[0]);
        Vector3 endPos = GetPosFromSettlement(settlements[1]);

        Vector3 direction = (endPos - startPos).normalized;
        float lineLength = Vector3.Distance(startPos, endPos);
        float spacing = lineLength / (connection.NUM_ENTITIES + 1);

        for (int i = 1; i <= connection.NUM_ENTITIES; i++)
        {
            Vector3 spawnPos = startPos + (i * spacing * direction);
            spawnPos.z = -2f;
            GameObject entity = Instantiate(GameManager.Instance.TransportPrefab, spawnPos, Quaternion.identity, connection.transform);
            connection.AddTransport(entity.GetComponent<Transport>());
            entity.transform.localScale = new Vector2(1.25f, 1.25f);

            SetupEntity(entity.GetComponent<Transport>(), i, settlements[0], settlements[1]);
        }
    }

    private Vector3 GetPosFromSettlement(Settlement settlement)
    {
        return settlement.ParentCell.transform.position;
    }

    private void SetupEntity(Transport entity, int i, Settlement startTown, Settlement endTown)
    {
        entity.movingForwards = i % 2 != 0;
        entity.boarding = false;
        entity._startTown = startTown;
        entity._endTown = endTown;
        entity.EntitySpeed = connection.ENTITY_SPEED;
        entity.Capacity = connection.CAPACITY;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportSpawning : MonoBehaviour
{
    private Line line;
    private Node depot;

    private void OnEnable() {
        line = GetComponent<Line>();
        depot = line.nodes[0];
        Spawn();
    }

    private void Spawn() {
        for (int i = 0; i < line.NUM_TRANSPORTS - line.activeTransports.Count; i++) {
            Vector3 spawnPos = depot.transform.position;
            Transport transport = Instantiate(GameManager.Instance.TransportPrefab, 
                                    spawnPos, Quaternion.identity, line.transform).GetComponent<Transport>();
            line.activeTransports.Add(transport);
            transport.transform.localScale = new Vector3(1.25f, 1.25f, 1f);

            SetupTransport(transport, line);
        }
    }

    private void SetupTransport(Transport transport, Line line)
    {
        transport.boarding = false;
        transport.line = line;
        transport.EntitySpeed = line.TRANSPORT_SPEED;
        transport.Capacity = line.TRANSPORT_CAPACITY;
    }
}
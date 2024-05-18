using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    private Settlement _startTown, _endTown;
    private List<Transport> _entities = new();
    internal float ENTITY_SPEED = 1f;
    internal int CAPACITY = 6;
    internal int NUM_ENTITIES = 1;

    public Settlement[] Towns = new Settlement[2];
    public void AddTransport(Transport transport) { _entities.Add(transport); }

    public event Action OnSpawningStart;

    public void SetupConnection(Settlement startTown, Settlement endTown)
    {
        _startTown = startTown;
        _endTown = endTown;

        Debug.Log("Towns are " + _startTown + " and " + _endTown);

        Towns[0] = _startTown;
        Towns[1] = _endTown;

        OnSpawningStart?.Invoke();
    }

    public void DestroyAllEntities() {
        foreach (Transport entity in _entities) {
            Destroy(entity.gameObject);
        }
        _entities.Clear();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    private List<Settlement> settlements = new();
    private List<Transport> _entities = new();
    internal float ENTITY_SPEED = 2f;
    internal int CAPACITY = 6;
    internal int NUM_ENTITIES = 1;
    internal bool LOOP;

    public Settlement[] Towns = new Settlement[2];
    public void AddTransport(Transport transport) { _entities.Add(transport); }

    public event Action OnSpawningStart;

    public void AddStop(Settlement settlement)
    {
        if (!settlements.Contains(settlement))
        {
            settlements.Add(settlement);
        }
    }

    public void RemoveStop(Settlement settlement)
    {
        if (settlements.Contains(settlement))
        {
            settlements.Remove(settlement);
        }
    }

    public bool ContainsStop(Settlement settlement) {
        if (settlements.Contains(settlement)) {
            return true;
        } else {
            return false;
        }
    }

    public void SetupConnection()
    {
        OnSpawningStart?.Invoke();
    }

    public void DestroyAllEntities()
    {
        foreach (Transport entity in _entities)
        {
            Destroy(entity.gameObject);
        }
        _entities.Clear();
    }
}

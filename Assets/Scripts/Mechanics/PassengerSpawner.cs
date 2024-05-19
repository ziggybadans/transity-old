using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerSpawner : MonoBehaviour, ISpawner
{
    private Settlement settlement;
    private float r_spawnInterval;
    private void OnEnable() {
        settlement = GetComponent<Settlement>();
        settlement.OnSpawningStart += Spawn;
    }

    private void OnDisable() {
        settlement.OnSpawningStart -= Spawn;
    }

    public void Spawn()
    {
        SettlementType settlementType = settlement.Type;
        r_spawnInterval = settlementType switch
        {
            SettlementType.City => 5f,
            SettlementType.Town => 10f,
            SettlementType.Rural => 15f,
            _ => 20f,
        };

        StartCoroutine(SpawnEntitiesCoroutine());
    }

    private IEnumerator SpawnEntitiesCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(r_spawnInterval);
            SpawnEntity();
        }
    }

    private void SpawnEntity()
    {
        GameObject newEntity = Instantiate(GameManager.Instance.PassengerPrefab, new Vector3(transform.position.x, transform.position.y, -3f), Quaternion.identity, settlement.transform);

        Passenger newPassenger = SetupNewPassenger(newEntity);
        SetPassengerSprite(newPassenger);

        settlement._passengers.Add(newPassenger);
    }

    private Passenger SetupNewPassenger(GameObject newEntity)
    {
        Passenger newPassenger = newEntity.GetComponent<Passenger>();
        newPassenger.Origin = settlement.Type;

        List<Settlement> settlements = new();
        foreach (Cell cell in GridManager.Instance.GetCells()) {
            if (cell.Settlement != null) settlements.Add(cell.Settlement);
        }
        Settlement randomSettlement = settlements[UnityEngine.Random.Range(0, settlements.Count)];
        while (randomSettlement.Type == newPassenger.Origin)
        {
            randomSettlement = settlements[UnityEngine.Random.Range(0, settlements.Count)];
        }
        newPassenger.Destination = randomSettlement.Type;

        return newPassenger;
    }

    private void SetPassengerSprite(Passenger passenger)
    {
        passenger.GetComponent<SpriteRenderer>().sprite = passenger.Destination switch
        {
            SettlementType.City => GameManager.Instance.CitySettlementSprite,
            SettlementType.Town => GameManager.Instance.TownSettlementSprite,
            SettlementType.Rural => GameManager.Instance.RuralSettlementSprite,
            _ => GameManager.Instance.RuralSettlementSprite,
        };
    }
}
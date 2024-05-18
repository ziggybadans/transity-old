using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerSpawner : MonoBehaviour, ISpawner
{
    private Settlement settlement;
    private readonly float r_spawnInterval = 3f;
    private void OnEnable() {
        settlement = GetComponent<Settlement>();
        settlement.OnSpawningStart += Spawn;
    }

    private void OnDisable() {
        settlement.OnSpawningStart -= Spawn;
    }

    public void Spawn()
    {
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
        GameObject newEntity = Instantiate(GameManager.Instance.PassengerPrefab, new Vector3(transform.position.x, transform.position.y, -3f), Quaternion.identity);

        Passenger newPassenger = SetupNewPassenger(newEntity);
        SetPassengerSprite(newPassenger);

        settlement._passengers.Add(newPassenger);
    }

    private Passenger SetupNewPassenger(GameObject newEntity)
    {
        Passenger newPassenger = newEntity.GetComponent<Passenger>();
        newPassenger.Origin = settlement;

        List<Settlement> settlements = new();
        foreach (Cell cell in GridManager.Instance.GetCells()) {
            if (cell.Settlement != null) settlements.Add(cell.Settlement);
        }
        Settlement randomSettlement = settlements[UnityEngine.Random.Range(0, settlements.Count)];
        while (randomSettlement == newPassenger.Origin)
        {
            randomSettlement = settlements[UnityEngine.Random.Range(0, settlements.Count)];
        }
        newPassenger.Destination = randomSettlement;

        return newPassenger;
    }

    private void SetPassengerSprite(Passenger passenger)
    {
        passenger.GetComponent<SpriteRenderer>().sprite = passenger.Destination.Type switch
        {
            SettlementType.City => GameManager.Instance.CitySettlementSprite,
            SettlementType.Town => GameManager.Instance.TownSettlementSprite,
            SettlementType.Rural => GameManager.Instance.RuralSettlementSprite,
            _ => GameManager.Instance.RuralSettlementSprite,
        };
    }
}
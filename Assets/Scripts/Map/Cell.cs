using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Settlement Settlement { get; set; }

    [SerializeField]
    private float _citySpawnProbability = 0.5f;
    [SerializeField]
    private float _townSpawnProbability = 0f;
    [SerializeField]
    private float _ruralSpawnProbability = 1f;

    private void Start() {
        float cellSize = GridManager.Instance.GRID_CELL_SIZE;
        float x = transform.position.x * cellSize + (cellSize / 2f);
        float y = transform.position.y * cellSize + (cellSize / 2f);
    }

    public void SetAllProbabilities(float value) {
        _citySpawnProbability = value;
        _townSpawnProbability = value;
        _ruralSpawnProbability = value;
    }

    public void SetProbability(SettlementType type, float value) {
        switch (type) {
            case SettlementType.City:
                _citySpawnProbability = value;
                break;
            case SettlementType.Town:
                _townSpawnProbability = value;
                break;
            case SettlementType.Rural:
                _ruralSpawnProbability = value;
                break;
            default:
                break;
        }
    }

    public float GetProbability(SettlementType type) {
        float value = type switch {
            SettlementType.City => _citySpawnProbability,
            SettlementType.Town => _townSpawnProbability,
            SettlementType.Rural => _ruralSpawnProbability,
            _ => _ruralSpawnProbability,
        };

        return value;
    }
}

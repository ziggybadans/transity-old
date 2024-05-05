using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Settlement settlement;
    public List<TextMeshProUGUI> debugProbability = new();
    public float citySpawnProbability, townSpawnProbability, ruralSpawnProbability;
    public float cellSize;

    private void Start() {
        int x = (int)(transform.position.x * cellSize + (cellSize / 2f));
        int y = (int)(transform.position.y * cellSize + (cellSize / 2f));
    }

    public bool HasSettlement() {
        if (settlement != null) {
            return true;
        } else {
            return false;
        }
    }
}

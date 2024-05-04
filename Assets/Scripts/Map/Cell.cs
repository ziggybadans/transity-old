using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public List<Settlement> settlements = new();
    public List<TextMeshProUGUI> debugProbability = new();
    public float settlementSpawnProbability;
    public float cellSize;
    public Vector3 centre;

    private void Start() {
        int x = (int)(transform.position.x * cellSize + (cellSize / 2f));
        int y = (int)(transform.position.y * cellSize + (cellSize / 2f));
        centre = new Vector3(x, y);
    }

    public bool HasSettlement() {
        if (settlements.Count > 0) {
            return true;
        } else {
            return false;
        }
    }
}

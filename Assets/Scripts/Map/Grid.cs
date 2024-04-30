using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public float cellSize = 1f;
    public int width = 100;
    public int height = 100;

    public Vector2 GetCellPositon(Vector2 worldPosition) {
        int x = Mathf.FloorToInt(worldPosition.x / cellSize);
        int y = Mathf.FloorToInt(worldPosition.y / cellSize);
        return new Vector2(x, y);
    }
}

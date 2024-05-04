using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    Grid grid;
    public int gridWidth = 34;
    public int gridHeight = 19;
    public float gridCellSize = 2f;
    private void Start() {
        grid = new Grid(gridWidth, gridHeight, gridCellSize);
    }
}

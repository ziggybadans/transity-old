using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public ContinentGrid continentGrid;

    private Grid currentGrid;

    public void ChangeZoom(int zoom) {
        if (zoom < 1) {
            currentGrid = continentGrid;
        }
    }
}

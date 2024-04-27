using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    private Vector2 startPos, endPos;
    private GameObject startTown, endTown;
    private List<GameObject> entities = new();

    public void SetupConnection(Vector2 startPos, Vector2 endPos)
    {
        this.startPos = startPos;
        this.endPos = endPos;
    }
}

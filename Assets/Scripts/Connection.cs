using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    private Vector2 startPos, endPos;
    private GameObject startTown, endTown;
    private int numEntites;
    private float entitySpeed;
    private bool movingForward;
    private List<GameObject> entities = new();
    private GameObject entityPrefab;

    public void SetupConnection(Vector2 startPos, Vector2 endPos, int numEntites, float entitySpeed, GameObject entityPrefab)
    {
        this.startPos = startPos;
        this.endPos = endPos;
        this.numEntites = numEntites;
        this.entitySpeed = entitySpeed;
        this.entityPrefab = entityPrefab;

        //SpawnEntities(startPos, endPos);
    }

    private void SpawnEntities(Vector2 startPos, Vector2 endPos)
    {
        Vector2 direction = (endPos - startPos).normalized;
        float lineLength = Vector3.Distance(startPos, endPos);
        float spacing = lineLength / (numEntites + 1);

        for (int i = 1; i <= numEntites; i++)
        {
            Vector2 spawnPos = startPos + (direction * spacing * i);
            GameObject entity = Instantiate(entityPrefab, spawnPos, Quaternion.identity);
            entities.Add(entity);
            if (i % 2 == 0)
            {
                movingForward = false;
            }
            else
            {
                movingForward = true;
            }
        }
    }

    private void Update()
    {
        if (movingForward)
        {
            transform.position = Vector2.MoveTowards(transform.position, endPos, entitySpeed * Time.deltaTime);
            if (transform.position.Equals(endPos)) movingForward = false;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, startPos, entitySpeed * Time.deltaTime);
            if (transform.position.Equals(startPos)) movingForward = true;
        }
    }
}

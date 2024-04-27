using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    public bool movingForward;
    public float entitySpeed = 1f;
    public Vector3 startPos;
    public Vector3 endPos;

    private void Update()
    {
        if (movingForward)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, entitySpeed * Time.deltaTime);
            if (transform.position.Equals(endPos)) movingForward = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, entitySpeed * Time.deltaTime);
            if (transform.position.Equals(startPos)) movingForward = true;
        }
    }
}
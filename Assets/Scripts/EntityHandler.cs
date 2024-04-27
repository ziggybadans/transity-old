using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHandler : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;
    private float speed;
    private bool movingForward;

    public void SetupMovement(Vector3 start, Vector3 end, float entitySpeed, bool initDirForward) {
        startPos = start;
        endPos = end;
        speed = entitySpeed;
        movingForward = initDirForward;
    }

    private void Update() {
        if (movingForward) {
            transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            if (transform.position == endPos) movingForward = false;
        } else {
            transform.position = Vector3.MoveTowards(transform.position, startPos, speed * Time.deltaTime);
            if (transform.position == startPos) movingForward = true;
        }
    }
}

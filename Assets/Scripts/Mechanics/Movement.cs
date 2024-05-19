using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Transport transport;
    public event Action OnStationArrived;

    private void OnEnable()
    {
        transport = GetComponent<Transport>();
    }

    private void Update()
    {
        if (!transport.boarding)
        {
            CheckSettlement();
            if (transport.movingForwards) MoveTransport(transport._startTown.transform.position);
                else MoveTransport(transport._endTown.transform.position);
        }
    }

    private void CheckSettlement()
    {
        Vector3 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if (pos == transport._startTown.transform.position || pos == transport._endTown.transform.position)
        {
            OnStationArrived?.Invoke();
        }
    }

    private void MoveTransport(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.SetPositionAndRotation(Vector3.MoveTowards(transform.position, targetPos, transport.EntitySpeed * Time.deltaTime), Quaternion.Euler(0f, 0f, angle));
    }
}

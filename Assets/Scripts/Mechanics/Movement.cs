using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Transport transport;
    public event Action OnStationArrived;
    private Settlement previousStop;

    private void OnEnable()
    {
        transport = GetComponent<Transport>();
    }

    private void Update()
    {
        if (!transport.boarding)
        {
            CheckNode();
            MoveTransport();
        }
    }

    private void CheckNode()
    {
        RaycastHit2D raycast = Raycast();
        if (raycast.collider != null && raycast.collider.TryGetComponent<Node>(out var node))
        {
            if (transport.transform.position == node.transform.position)
            {
                if (transport.line.nodes.Contains(node))
                {
                    if (node.nodeType == NodeType.Connection)
                    {
                        transport.currentNode = node;
                        transport.nextNode = transport.SetNextNode();
                    }
                    else if (node.nodeType == NodeType.Settlement)
                    {
                        RaycastHit2D raycastSettlement = Raycast();
                        if (raycast.collider != null && raycast.collider.TryGetComponent<Settlement>(out var settlement))
                        {
                            if (transport.line.stops.Contains(settlement) && settlement != previousStop)
                            {
                                OnStationArrived?.Invoke();
                                previousStop = settlement;
                            }
                        }

                    }

                }
            }
        }
    }

    private void MoveTransport()
    {
        Vector3 start = transport.currentNode.transform.position;
        Debug.Log("Start position is " + start.x + ", " + start.y);
        Vector3 end = transport.nextNode.transform.position;
        Debug.Log("End position is " + end.x + ", " + end.y);
        Vector3 direction = new(end.x - start.x, end.y - start.y);
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Debug.Log("Angle is " + angle);
        transform.SetPositionAndRotation(Vector3.MoveTowards(transform.position, transport.nextNode.transform.position, transport.EntitySpeed * Time.deltaTime), Quaternion.Euler(0f, 0f, angle));
    }

    private RaycastHit2D Raycast()
    {
        Vector2 transportPos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transportPos, Vector2.zero, 100f);
        return hit;
    }
}

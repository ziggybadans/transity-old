using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transport : MonoBehaviour
{
    public bool movingForward;
    private int boarding = 0;
    // 0 = moving, 1 = boarding, 2 = ready to depart
    public float entitySpeed;
    public int capacity;
    private Vector3 startPos, endPos;
    public Settlement startTown, endTown;
    private GameObject passengerObj;
    private List<Passenger> passengers = new();

    public List<Passenger> GetPassengers() { return passengers; }

    private void Start()
    {
        passengerObj = new GameObject("PassengerContainer");
        passengerObj.transform.parent = transform;
        passengerObj.transform.SetLocalPositionAndRotation(new Vector3(-0.25f, 0f, -3f), Quaternion.identity);
        passengerObj.transform.localScale *= 0.8f;
    }

    private void Update()
    {
        PositionPassengers();

        // Check to see if the transport is at a train
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if ((pos.Equals(startPos) && startTown != null) || (pos.Equals(endPos) && endTown != null))
        {
            // As long as it's not already boarding, the train can start boarding
            if (boarding == 0)
            {
                boarding = 1;
                StartCoroutine(BoardAndAlight(pos.Equals(startPos) ? startTown : endTown));
            }
        }
        else
        {
            // Reversing direction on the connection
            if (movingForward)
            {
                MoveTransport(endPos);
                if (transform.position.Equals(endPos)) movingForward = false;
            }
            else
            {
                MoveTransport(startPos);
                if (transform.position.Equals(startPos)) movingForward = true;
            }
        }

        if (boarding == 2)
        {
            if (movingForward)
            {
                MoveTransport(endPos);
                if (transform.position.Equals(endPos)) movingForward = false;
            }
            else
            {
                MoveTransport(startPos);
                if (transform.position.Equals(startPos)) movingForward = true;
            }
            if (!(hit.collider != null && hit.collider.CompareTag("Settlement")))
            {
                boarding = 0;
            }
        }
    }

    private void PositionPassengers()
    {
        for (int i = 0; i < passengers.Count; i++)
        {
            int row = i / 2;
            int column = i % 2;
            Vector3 passengerPosition = new(column * 0.2f, -row * 0.2f, 0f);

            passengers[i].transform.SetParent(passengerObj.transform, false);
            passengers[i].transform.SetLocalPositionAndRotation(passengerPosition, Quaternion.identity);
        }
    }

    private void MoveTransport(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.SetPositionAndRotation(Vector3.MoveTowards(transform.position, targetPos, entitySpeed * Time.deltaTime), Quaternion.Euler(0f, 0f, angle));
    }

    private IEnumerator BoardAndAlight(Settlement settlement)
    {
        // Wait for a maximum of 10 seconds
        float elapsedTime = 0f;
        while (elapsedTime < 10f)
        {
            // Check if there are passengers waiting to board
            int passengersWaiting = settlement.GetPassengersWaiting();
            if (passengersWaiting > 0 && passengers.Count < capacity)
            {
                // Board passengers one by one
                PassengersBoarding(settlement);
            }

            // Check if there are passengers on board
            if (passengers.Count > 0)
            {
                // Alight passengers one by one
                PassengersAlighting(settlement);
            }
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        // Reset boarding flag and resume movement
        boarding = 2;
    }

    private void PassengersBoarding(Settlement settlement)
    {
        Passenger passenger = settlement.AlightPassenger(this);
        if (passenger != null)
        {
            passengers.Add(passenger);
            passenger.gameObject.transform.localScale *= 0.35f;
            passenger.GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            Debug.Log("New passenger boarded! Number of passengers: " + passengers.Count);
        }
    }

    private void PassengersAlighting(Settlement settlement)
    {
        for (int i = passengers.Count - 1; i >= 0; i--)
        {
            if (passengers[i].origin != settlement)
            {
                Destroy(passengers[i].gameObject);
                passengers.RemoveAt(i);
            }
        }
    }
}
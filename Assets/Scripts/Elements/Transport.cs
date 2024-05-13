using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transport : MonoBehaviour
{
    public bool MovingForward;
    private int _boarding = 0;
    // 0 = moving, 1 = boarding, 2 = ready to depart
    public float EntitySpeed;
    public int Capacity;
    private Vector3 _startPos, _endPos;
    public Settlement _startTown, _endTown;
    private GameObject _passengerObj;
    private List<Passenger> _passengers = new();

    public List<Passenger> GetPassengers() { return _passengers; }

    private void Start()
    {
        _passengerObj = new GameObject("PassengerContainer");
        _passengerObj.transform.parent = transform;
        _passengerObj.transform.SetLocalPositionAndRotation(new Vector3(-0.25f, 0f, -3f), Quaternion.identity);
        _passengerObj.transform.localScale *= 0.8f;
    }

    private void Update()
    {
        PositionPassengers();

        // Check to see if the transport is at a train
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if ((pos.Equals(_startPos) && _startTown != null) || (pos.Equals(_endPos) && _endTown != null))
        {
            // As long as it's not already boarding, the train can start boarding
            if (_boarding == 0)
            {
                _boarding = 1;
                StartCoroutine(BoardAndAlight(pos.Equals(_startPos) ? _startTown : _endTown));
            }
        }
        else
        {
            // Reversing direction on the connection
            if (MovingForward)
            {
                MoveTransport(_endPos);
                if (transform.position.Equals(_endPos)) MovingForward = false;
            }
            else
            {
                MoveTransport(_startPos);
                if (transform.position.Equals(_startPos)) MovingForward = true;
            }
        }

        if (_boarding == 2)
        {
            if (MovingForward)
            {
                MoveTransport(_endPos);
                if (transform.position.Equals(_endPos)) MovingForward = false;
            }
            else
            {
                MoveTransport(_startPos);
                if (transform.position.Equals(_startPos)) MovingForward = true;
            }
            if (!(hit.collider != null && hit.collider.CompareTag("Settlement")))
            {
                _boarding = 0;
            }
        }
    }

    private void PositionPassengers()
    {
        for (int i = 0; i < _passengers.Count; i++)
        {
            int row = i / 2;
            int column = i % 2;
            Vector3 passengerPosition = new(column * 0.2f, -row * 0.2f, 0f);

            _passengers[i].transform.SetParent(_passengerObj.transform, false);
            _passengers[i].transform.SetLocalPositionAndRotation(passengerPosition, Quaternion.identity);
        }
    }

    private void MoveTransport(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.SetPositionAndRotation(Vector3.MoveTowards(transform.position, targetPos, EntitySpeed * Time.deltaTime), Quaternion.Euler(0f, 0f, angle));
    }

    private IEnumerator BoardAndAlight(Settlement settlement)
    {
        // Wait for a maximum of 10 seconds
        float elapsedTime = 0f;
        while (elapsedTime < 10f)
        {
            // Check if there are passengers waiting to board
            int passengersWaiting = settlement.GetPassengersWaiting();
            if (passengersWaiting > 0 && _passengers.Count < Capacity)
            {
                // Board passengers one by one
                PassengersBoarding(settlement);
            }

            // Check if there are passengers on board
            if (_passengers.Count > 0)
            {
                // Alight passengers one by one
                PassengersAlighting(settlement);
            }
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        // Reset boarding flag and resume movement
        _boarding = 2;
    }

    private void PassengersBoarding(Settlement settlement)
    {
        Passenger passenger = settlement.AlightPassenger(this);
        if (passenger != null)
        {
            _passengers.Add(passenger);
            passenger.gameObject.transform.localScale *= 0.35f;
            passenger.GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            Debug.Log("New passenger boarded! Number of passengers: " + _passengers.Count);
        }
    }

    private void PassengersAlighting(Settlement settlement)
    {
        for (int i = _passengers.Count - 1; i >= 0; i--)
        {
            if (_passengers[i].Origin != settlement)
            {
                Destroy(_passengers[i].gameObject);
                _passengers.RemoveAt(i);
            }
        }
    }
}
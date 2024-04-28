using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Transport : MonoBehaviour
{
    public bool movingForward;
    // 0 = moving, 1 = boarding, 2 = ready to depart
    public int boarding = 0;
    public float entitySpeed = 1f;
    public int capacity = 6;
    public Vector3 startPos;
    public Vector3 endPos;
    private GameObject childObj;

    public List<Passenger> passengers = new();

    private void Start()
    {
        childObj = new GameObject("PassengerContainer");
        childObj.transform.parent = transform;
        childObj.transform.localPosition = new Vector3(-0.25f, 0f, -3f);
        childObj.transform.localRotation = Quaternion.identity;
        childObj.transform.localScale *= 0.8f;
    }

    private void Update()
    {
        for (int i = 0; i < passengers.Count; i++)
        {
            int row = i / 2;
            int column = i % 2;
            Vector3 passengerPosition = new Vector3(column * 0.2f, -row * 0.2f, 0f);

            passengers[i].transform.SetParent(childObj.transform, false);
            passengers[i].transform.localPosition = passengerPosition;
            passengers[i].transform.localRotation = Quaternion.identity;
        }
        // Check to see if the transport is at a train
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        // If it is at a town, wait til it's at the very end of the connection
        if (hit.collider != null && hit.collider.CompareTag("Settlement") && (pos.Equals(startPos) || pos.Equals(endPos)))
        {
            // As long as it's not already boarding, the train can start boarding
            if (boarding == 0)
            {
                boarding = 1;
                StartCoroutine(BoardAndAlight(hit));
            }
        }
        else
        {
            // Reversing direction on the connection
            if (movingForward)
            {
                Vector3 direction = endPos - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
                transform.position = Vector3.MoveTowards(transform.position, endPos, entitySpeed * Time.deltaTime);
                if (transform.position.Equals(endPos)) movingForward = false;
            }
            else
            {
                Vector3 direction = startPos - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
                transform.position = Vector3.MoveTowards(transform.position, startPos, entitySpeed * Time.deltaTime);
                if (transform.position.Equals(startPos)) movingForward = true;
            }
        }

        if (boarding == 2)
        {
            if (movingForward)
            {
                Vector3 direction = endPos - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
                transform.position = Vector3.MoveTowards(transform.position, endPos, entitySpeed * Time.deltaTime);
                if (transform.position.Equals(endPos)) movingForward = false;
            }
            else
            {
                Vector3 direction = startPos - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
                transform.position = Vector3.MoveTowards(transform.position, startPos, entitySpeed * Time.deltaTime);
                if (transform.position.Equals(startPos)) movingForward = true;
            }
            if (!(hit.collider != null && hit.collider.CompareTag("Settlement")))
            {
                boarding = 0;
            }
        }
    }

    private IEnumerator BoardAndAlight(RaycastHit2D hit)
    {
        // Wait for a maximum of 10 seconds
        float elapsedTime = 0f;
        while (elapsedTime < 10f)
        {
            // Check if there are passengers waiting to board
            int passengersWaiting = hit.collider.GetComponent<Settlement>().GetPassengersWaiting();
            if (passengersWaiting > 0 && passengers.Count < capacity)
            {
                // Board passengers one by one
                PassengersBoarding(hit.collider.GetComponent<Settlement>());
            }

            // Check if there are passengers on board
            if (passengers.Count > 0)
            {
                // Alight passengers one by one
                PassengersAlighting(hit.collider.GetComponent<Settlement>());
            }
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        // Reset boarding flag and resume movement
        boarding = 2;
    }

    private void PassengersBoarding(Settlement settlement)
    {

        Passenger passenger = settlement.AlightPassenger();
        if (passenger != null)
        {
            passengers.Add(passenger);
            passenger.gameObject.transform.localScale *= 0.35f;
            passenger.GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            passenger.gameObject.SetActive(true);
            Debug.Log("New passenger boarded! Number of passengers: " + passengers.Count);
        }
    }

    private void PassengersAlighting(Settlement settlement)
    {
        foreach (Passenger passenger in passengers.ToArray())
        {
            if (passenger.origin != settlement)
            {
                Destroy(passenger.gameObject);
                passengers.Remove(passenger);
            }
        }
    }
}
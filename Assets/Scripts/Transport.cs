using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Transport : MonoBehaviour
{
    public bool movingForward;
    public bool boarding;
    public float entitySpeed = 1f;
    public Vector3 startPos;
    public Vector3 endPos;

    public List<GameObject> passengers = new();

    private void Update()
    {
        // Check to see if the transport is at a train
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        // If it is at a town, wait til it's at the very end of the connection
        if (hit.collider != null && hit.collider.CompareTag("Settlement") && (pos.Equals(startPos) || pos.Equals(endPos)))
        {
            // As long as it's not already boarding, the train can start boarding
            if (!boarding)
            {
                boarding = true;
                StartCoroutine(BoardAndAlight(hit));
            }
        }

        // If the train is not boarding, it can start moving
        if (!boarding)
        {
            // Reversing direction on the connection
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

    private IEnumerator BoardAndAlight(RaycastHit2D hit)
    {
        // Wait for a maximum of 10 seconds
        float elapsedTime = 0f;
        while (elapsedTime < 10f)
        {
            // Check if there are passengers waiting to board
            int passengersWaiting = hit.collider.GetComponent<Settlement>().GetPassengersWaiting();
            if (passengersWaiting > 0)
            {
                // Board passengers one by one
                StartCoroutine(PassengersBoarding(hit.collider.GetComponent<Settlement>()));
            }

            // Check if there are passengers on board
            if (passengers.Count > 0)
            {
                // Alight passengers one by one
                StartCoroutine(PassengersAlighting());
            }

            // Wait for 1 second before checking again
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        // Reset boarding flag and resume movement
        boarding = false;
    }

    private IEnumerator PassengersBoarding(Settlement settlement)
    {
        GameObject passenger = settlement.AlightPassenger();
        passengers.Add(passenger);
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PassengersAlighting()
    {
        GameObject passenger = passengers[0];
        passengers.RemoveAt(0);
        Destroy(passenger);
        yield return new WaitForSeconds(1f);
    }


}
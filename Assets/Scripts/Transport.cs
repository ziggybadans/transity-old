using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Transport : MonoBehaviour
{
    public bool movingForward;
    public bool boarding;
    int passengersWaiting;
    int counter;
    public float entitySpeed = 1f;
    public Vector3 startPos;
    public Vector3 endPos;

    public List<GameObject> passengers = new();

    private void Update()
    {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if (hit.collider != null && hit.collider.CompareTag("Settlement") && (pos.Equals(startPos) || pos.Equals(endPos)))
        {
            if (!passengers.Any())
            {
                passengersWaiting = hit.collider.GetComponent<Settlement>().HasPassengersWaiting();
                if (passengersWaiting > 0 && !boarding)
                {
                    boarding = true;
                    counter = passengersWaiting;
                    StartCoroutine("PassengersBoarding", hit);
                } else if (passengersWaiting <= 0 && boarding) {
                    boarding = false;
                }
            }
            else
            {
                boarding = true;
                counter = passengers.Count;
                StartCoroutine("PassengersAlighting");
            }
        }

        if (!boarding)
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

    private IEnumerator PassengersBoarding(RaycastHit2D hit)
    {
        while (passengersWaiting > 0)
        {
            GameObject passenger = hit.collider.GetComponent<Settlement>().AlightPassengers(counter);
            passengers.Add(passenger);
            yield return new WaitForSeconds(1f);
            counter--;
        }
    }

    private IEnumerator PassengersAlighting() {
        Destroy(passengers[counter]);
        passengers.RemoveAt(counter);
        yield return new WaitForSeconds(1f);
        counter--;
    }
}
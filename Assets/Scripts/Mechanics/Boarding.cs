using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boarding : MonoBehaviour
{
    private Transport transport;
    private void OnEnable() {
        transport = GetComponent<Transport>();
        transport.OnBoardingStart += HandleBoardingStart;
    }

    private void OnDisable() {
        transport.OnBoardingStart -= HandleBoardingStart;
    }

    private void HandleBoardingStart(object sender, TrainEventData e) {
        StartCoroutine(BoardingCoroutine(e));
    }

    private IEnumerator BoardingCoroutine(TrainEventData e)
    {
        Debug.Log("Capacity is " + e.capacity);
        float elapsedTime = 0f;
        while (elapsedTime <= 10f) {
            Debug.Log("Boarding...");
            //int passengersWaiting = e.currentSettlement.GetPassengersWaiting();
            if (e.passengersOnBoard.Count < e.capacity) {
                Debug.Log("Attempting to board passenger...");
                transport.BoardPassenger(e);
                transport.PositionPassengers();
            }
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }
        transport.boarding = false;
    }
}

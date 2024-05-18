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
        float elapsedTime = 0f;
        while (elapsedTime <= 10f) {
            Debug.Log("Boarding...");
            int passengersWaiting = e.currentSettlement.GetPassengersWaiting();
            if (passengersWaiting > 0 && e.passengersOnBoard.Count < e.capacity) {
                transport.BoardPassenger(e);
                transport.PositionPassengers();
            }
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }
    }
}

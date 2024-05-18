using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alighting : MonoBehaviour
{
    private Transport transport;
    private void OnEnable() {
        transport = GetComponent<Transport>();
        transport.OnBoardingStart += HandleAlightingStart;
    }

    private void OnDisable() {
        transport.OnBoardingStart -= HandleAlightingStart;
    }

    private void HandleAlightingStart(object sender, TrainEventData e) {
        StartCoroutine(PassengersAlighting(e));
    }

    private IEnumerator PassengersAlighting(TrainEventData e)
    {
        while (transport.GetPassengers().Count > 0) {
            Debug.Log("Alighting...");
            transport.AlightPassenger(e);
            yield return new WaitForSeconds(0.25f);
        }
    }
}

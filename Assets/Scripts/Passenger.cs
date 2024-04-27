using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    public Settlement origin, destination;

    public void SetupPassenger(Settlement startTown, Settlement endTown) {
        origin = startTown;
        destination = endTown;
    }
}

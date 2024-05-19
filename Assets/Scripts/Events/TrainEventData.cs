using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrainEventData
{
    public List<Passenger> passengersOnBoard;
    public int capacity;
    public Settlement currentSettlement;

    public TrainEventData(List<Passenger> passengers, int capacity, Settlement settlement) {
        passengersOnBoard = passengers;
        this.capacity = capacity;
        currentSettlement = settlement;
    }
}

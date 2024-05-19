using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Transport : MonoBehaviour
{
    [SerializeField]
    internal bool movingForwards;
    [SerializeField]
    internal bool boarding;
    [SerializeField]
    internal bool alighting;
    public float EntitySpeed;
    public int Capacity;
    public Settlement _startTown, _endTown;
    private GameObject _passengerObj;
    private List<Passenger> _passengers = new();

    public List<Passenger> GetPassengers() { return _passengers; }

    public event EventHandler<TrainEventData> OnAlightingStart, OnBoardingStart;

    private void Start()
    {
        _passengerObj = new GameObject("PassengerContainer");
        _passengerObj.transform.parent = transform;
        Vector3 newPosition = new(-0.25f, 0f, -3f);
        _passengerObj.transform.SetLocalPositionAndRotation(new Vector3(-0.25f, 0f, -3f), Quaternion.identity);
        _passengerObj.transform.localPosition = newPosition;
        _passengerObj.transform.localScale *= 0.8f;
    }

    private void OnEnable()
    {
        GetComponent<Movement>().OnStationArrived += ArriveAtStation;
    }

    private void OnDisable()
    {
        GetComponent<Movement>().OnStationArrived -= ArriveAtStation;
    }

    private void ArriveAtStation()
    {
        boarding = true;
        alighting = true;
        movingForwards = !movingForwards;
        StartCoroutine(StationCycleCoroutine(transform.position));
    }

    private IEnumerator StationCycleCoroutine(Vector2 pos)
    {
        Debug.Log("Cueing station loop...");
        Settlement currentSettlement = GridManager.Instance.GetCellFromPos(pos).Settlement;
        var eventData = new TrainEventData(_passengers, Capacity, currentSettlement);

        OnAlightingStart?.Invoke(this, eventData);
        yield return new WaitUntil(() => alighting == false);

        OnBoardingStart?.Invoke(this, eventData);
        yield return new WaitUntil(() => boarding == false);
    }

    internal void BoardPassenger(TrainEventData e)
    {
        Passenger passenger = e.currentSettlement.DepartPassenger(this);
        if (passenger != null)
        {
            _passengers.Add(passenger);
            passenger.gameObject.transform.localScale *= 0.35f;
            passenger.GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            Debug.Log("New passenger boarded! Destination is " + passenger.Destination.ToString() + ". Number of passengers: " + _passengers.Count);
        }
    }

    internal void AlightPassenger(TrainEventData e)
    {
        Debug.Log("Passenger leaving train!");
        Destroy(_passengers[0].gameObject);
        _passengers.RemoveAt(0);
    }

    internal void PositionPassengers()
    {
        for (int i = 0; i < _passengers.Count; i++)
        {
            int row = i / 2;
            int column = i % 2;
            Vector2 passengerPosition = new(column * 0.75f, -row * 0.75f);

            _passengers[i].transform.SetParent(_passengerObj.transform, false);
            _passengers[i].transform.localScale = new Vector2(0.25f, 0.25f);
            _passengers[i].transform.SetLocalPositionAndRotation(passengerPosition, Quaternion.identity);
            _passengers[i].transform.localPosition = passengerPosition;
        }
    }
}
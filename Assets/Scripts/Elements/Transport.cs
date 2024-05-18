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
        _passengerObj.transform.SetLocalPositionAndRotation(new Vector3(-0.25f, 0f, -3f), Quaternion.identity);
        _passengerObj.transform.localScale *= 0.8f;
    }

    private void OnEnable()
    {
        GetComponent<Movement>().OnStationArrived += Board;
    }

    private void OnDisable() {
        GetComponent<Movement>().OnStationArrived -= Board;
    }

    private void Board()
    {
        boarding = true;
        movingForwards = !movingForwards;
        StartCoroutine(StationLoopCoroutine(transform.position));
    }

    private IEnumerator StationLoopCoroutine(Vector2 pos)
    {
        while (boarding)
        {
            Debug.Log("Cueing station loop...");
            Settlement currentSettlement = GridManager.Instance.GetCellFromPos(pos).Settlement;
            var eventData = new TrainEventData(_passengers, Capacity, currentSettlement);

            // Notify alighting
            yield return AwaitAlighting(eventData);

            // Notify boarding
            OnBoardingStart?.Invoke(this, eventData);
            yield return new WaitForSeconds(10);

            boarding = false;
        }
    }

    private IEnumerator AwaitAlighting(TrainEventData eventData)
    {
        var tcs = new TaskCompletionSource<bool>();

        void handler(object sender, TrainEventData args)
        {
            // Unsubscribe to prevent memory leaks
            OnAlightingStart -= handler;
            tcs.SetResult(true);
        }

        OnAlightingStart += handler;
        OnAlightingStart?.Invoke(this, eventData);

        // Await the task
        yield return new WaitUntil(() => tcs.Task.IsCompleted);
    }

    internal void BoardPassenger(TrainEventData e)
    {
        Passenger passenger = e.currentSettlement.DepartPassenger(this);
        if (passenger != null)
        {
            _passengers.Add(passenger);
            passenger.gameObject.transform.localScale *= 0.35f;
            passenger.GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            Debug.Log("New passenger boarded! Number of passengers: " + _passengers.Count);
        }
    }

    internal void AlightPassenger(TrainEventData e)
    {
        if (_passengers[0].Origin != e.currentSettlement)
        {
            Destroy(_passengers[0].gameObject);
            _passengers.RemoveAt(0);
        }
    }

    internal void PositionPassengers()
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
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SettlementType { City, Town, Rural }

public class Settlement : MonoBehaviour
{
    public Cell ParentCell;

    internal List<Passenger> _passengers = new();
    public SettlementType Type { get; set; }

    public event Action OnSpawningStart;

    private void Start() {
        OnSpawningStart?.Invoke();
    }

    private void Update()
    {
        SetPassengerOpacity();
        CalculatePosition();
    }

    private void CalculatePosition()
    {
        for (int i = 0; i < _passengers.Count; i++)
        {
            _passengers[i].gameObject.transform.position = i switch
            {
                0 => (Vector3)(GetPassengerInitialPos() + (Vector2.up * 0.5f)),
                5 => (Vector3)(GetPassengerInitialPos() + (Vector2.down * 0.1f)),
                _ => _passengers[i - 1].transform.position +
                        (1.3f * _passengers[i - 1].GetComponent<SpriteRenderer>().bounds.size.x * Vector3.left),
            };
            _passengers[i].gameObject.transform.localScale = new Vector2(0.35f, 0.35f);
        }
    }

    private Vector2 GetPassengerInitialPos() {
        Vector2 vector2 = new Vector2(transform.position.x, transform.position.y) + (Vector2.left * GetComponent<SpriteRenderer>().bounds.size.x);
        return vector2;
    }

    private void SetPassengerOpacity()
    {
        int passengerCount = _passengers.Count;
        if (passengerCount <= 10)
        {
            for (int i = 0; i < passengerCount; i++)
            {
                _passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            }
        }
        else if (passengerCount == 11)
        {
            for (int i = 0; i < 9; i++)
            {
                _passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            }
            _passengers[9].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.75f);
            for (int i = 10; i < passengerCount; i++)
            {
                _passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0f);
            }
        }
        else if (passengerCount == 12)
        {
            for (int i = 0; i < 8; i++)
            {
                _passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            }
            _passengers[9].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.5f);
            _passengers[8].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.75f);
            for (int i = 10; i < passengerCount; i++)
            {
                _passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0f);
            }
        }
        else if (passengerCount >= 13)
        {
            for (int i = 0; i < 7; i++)
            {
                _passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            }
            _passengers[9].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.25f);
            _passengers[8].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.5f);
            _passengers[7].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0.75f);
            for (int i = 10; i < passengerCount; i++)
            {
                _passengers[i].GetComponent<SpriteRenderer>().material.color = new Color(0f, 0f, 0f, 0f);
            }
        }
    }

    public Passenger DepartPassenger(Transport transport)
    {
        if (_passengers.Count > 0)
        {
            Debug.Log("There are passengers at this station.");
            int i = 0;
            while (i < _passengers.Count)
            {
                if (transport.line.stopVariety.Contains(_passengers[i].Destination))
                {
                    Debug.Log("Passenger leaving station!");
                    Passenger passengerAlighting = _passengers[i];
                    _passengers.Remove(passengerAlighting);
                    return passengerAlighting;
                }
                else
                {
                    i++;
                }
            }
        }

        return null;
    }

    public int GetPassengersWaiting()
    {
        return _passengers.Count;
    }
}
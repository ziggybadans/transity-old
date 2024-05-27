using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public List<Node> nodes = new();
    public List<Connection> path = new();
    public List<SettlementType> stopVariety = new(); 
    public List<Settlement> stops = new();
    internal float TRANSPORT_SPEED = 2f;
    internal int TRANSPORT_CAPACITY = 6;
    internal int NUM_TRANSPORTS = 1;
    internal bool LOOP = false;
    public List<Transport> activeTransports = new();
}
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public NodeType nodeType;
    public List<Connection> connections = new();
    public List<Line> lines = new();
}

public enum NodeType { Settlement, Visible, Hidden }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    #region Properties
    int x, y;
    Node parent;
    public List<Node> adjacentSquares;
    public bool visited;    

    public void SetParent(Node _parent) { parent = _parent; }
    public Node GetParent() { return parent; }
    #endregion

    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
        adjacentSquares = new List<Node>();
        visited = false;
    }

    public int X()
    {
        return x;
    }

    public int Y()
    {
        return y;
    }

    public void SetAdjacentSquare(Node square)
    {
        adjacentSquares.Add(square);
    }
}

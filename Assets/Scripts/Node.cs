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
    public bool changeColor;
    public Color color;
    public Vector2 worldPosition;

    public void SetParent(Node _parent) { parent = _parent; }
    public Node GetParent() { return parent; }
    #endregion

    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
        adjacentSquares = new List<Node>();
        visited = false;
        changeColor = false;
        color = Color.None;

    }

    public void SetWorldPos(float x, float y)
    {
        worldPosition = new Vector2(x, y);
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

    public void SetColor(Color newColor)
    {
        color = newColor;
    }

    public Color GetColor()
    {
        return color;
    }
}

public enum Color
{
    White,
    Black,
    None
}


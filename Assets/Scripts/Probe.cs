using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probe 
{
    int x, y;
    Point direction;
    public bool done;
    public Color currentStateTurn;
    public List<Node> nodesToFlip = new List<Node>();
    public Probe(int directX, int directY, Color color)
    {
        direction = new Point(directX, directY);
        currentStateTurn = color;
    }

    public void Move()
    {
        x += direction.X;
        y += direction.Y;
    }

    public void SetNewStart(int startX, int startY)
    {
        x = startX;
        y = startY;
        nodesToFlip.Clear();
    }

    public void FlipNodes()
    {
        foreach (Node node in nodesToFlip)
        {
            node.SetColor(currentStateTurn);
        }
    }

    public void SwapTurnColor()
    {
        if(currentStateTurn == Color.White)
        {
            currentStateTurn = Color.Black;
        }
        else
        {
            currentStateTurn = Color.White;
        }
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }
}

public struct Point
{
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }
}

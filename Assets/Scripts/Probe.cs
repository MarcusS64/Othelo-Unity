using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probe 
{
    int x, y;
    Point direction;
    public Probe(int startX, int startY, Point direct)
    {
        x = startX;
        y = startY;
        direction = direct;
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

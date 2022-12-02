using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    #region Properties
    public int x, y;
    public List<Node> adjacentSquares;
    public bool visited;
    public bool changeColor;
    public Color color;
    public Vector2 worldPosition;
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

    //public void AdjacentNode(Node[,] graph)
    //{
    //    for (int i = -1; i <= 1; i++)
    //    {
    //        for (int j = -1; j <= 1; j++)
    //        {
    //            if (i == 0 && j == 0) continue;

    //            int x = this.x + i;
    //            int y = this.y + j;

    //            if (x >= 0 && x < GameFlow.width && y >= 0 && y < 32)
    //            {
    //                if (x == this.x || y == this.x)
    //                {
    //                    adjacentSquares.Add(graph[x, y]);
    //                }
    //            }
    //        }
    //    }
    //}

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


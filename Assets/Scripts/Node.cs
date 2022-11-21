using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    #region Properties
    int x, y;
    Node parent;
    int score;
    float height;
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
        height = 0f;
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

    public void SetAverageHeight(bool includeParent)
    {
        float totalHeight = 3 * height; //Bigger weight to the square we're on
        float numberOfSorroundingSquares = 1;
        foreach (Node square in adjacentSquares)
        {
            totalHeight += square.height + square.GetSorroundingHeight(this, includeParent);
            numberOfSorroundingSquares += 1 + square.adjacentSquares.Count;
        }
        height = totalHeight / numberOfSorroundingSquares;
    }

    public float GetSorroundingHeight(Node parent, bool includeParent)
    {
        float totalSorroundingHeight = 0f;
        foreach (Node square in adjacentSquares)
        {
            if (!includeParent)
            {
                if (square != parent) //Remove this for the cool effect
                    totalSorroundingHeight += square.height;
            }
            else
            {
                totalSorroundingHeight += square.height;
            }
            
        }
        return totalSorroundingHeight;
    }

    public bool SameSorroundingElevation(int radius)
    {
        bool sameElevation = true;
        radius--;
        foreach (Node node in adjacentSquares)
        {
            if (height != node.GetHeight())
            {
                sameElevation = false;
                return sameElevation;
            }
        }
        if(radius > 0)
        {
            foreach (Node node in adjacentSquares)
            {
                sameElevation = node.SameSorroundingElevation(radius);
                if (!sameElevation)
                {
                    return sameElevation;
                }

            }
        }
        
        return sameElevation;
    }

    public float GetHeight()
    {
        return height;
    }

    public void SetHeight(float newHeight, string message)
    {
        if (height > 0.5f)
        {
            Debug.Log(message);
        }
        height += newHeight;
    }

    public bool Sorrounded()
    {
        foreach (Node neighbour in adjacentSquares)
        {
            if (!neighbour.visited)
            {
                return false;
            }
        }
        return true;
    }
}

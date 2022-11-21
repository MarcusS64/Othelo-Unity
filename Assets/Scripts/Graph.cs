using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    #region Properties
    private Node[,] squares; //Was static
    private int graphWidth, graphHeight;
    public List<Node> Open { get; private set; }
    public List<Node> Closed { get; private set; }

    public Node CurrentNode { get; private set; }
    public int Generation { get; private set; }

    //public Node[] PlayerGoalSquares { get; private set; }
    //public Node[] OpponentGoalSquares { get; private set; }
    #endregion   

    public Graph(int N, int M, int generation)
    {
        SetProperties(N, M, generation);
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < M; j++)
            {
                squares[i, j] = new Node(i, j);

            }
        }

        ConnectSquares(N, M, true);
        ConnectSquares(N, M, false);
    }

    private void SetProperties(int N, int M, int generation)
    {
        Open = new List<Node>();
        Closed = new List<Node>();
        squares = new Node[N, M];
        graphWidth = N;
        graphHeight = M;
        Generation = generation;
    }

    private void ConnectSquares(int width, int height, bool horizontal) //Was static
    {
        if (horizontal) { width--; }
        else { height--; }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (horizontal)
                {
                    squares[i, j].SetAdjacentSquare(squares[i + 1, j]);
                    squares[i + 1, j].SetAdjacentSquare(squares[i, j]);

                }
                else //ie vertical
                {
                    squares[i, j].SetAdjacentSquare(squares[i, j + 1]);
                    squares[i, j + 1].SetAdjacentSquare(squares[i, j]);
                }
            }
        }
    }

    #region Pathfinding
    public Node FindSquare(int x, int y)
    {
        foreach (Node square in squares)
        {
            if (x == square.X() && y == square.Y())
            {
                return square;
            }
        }
        return null;
    }

    public void SetCurrentNode(Node square)
    {
        CurrentNode = square;
    }

    public void RepairLinks(Node first, Node removedFromFirst, Node second, Node removedFromSecond)
    {
        first.SetAdjacentSquare(removedFromFirst);
        removedFromFirst.SetAdjacentSquare(first);

        second.SetAdjacentSquare(removedFromSecond);
        removedFromSecond.SetAdjacentSquare(second);
    }
    #endregion

    public int GetWidth()
    {
        return graphWidth;
    }

    public int GetHeight()
    {
        return graphHeight;
    }

    public Node GetSquare(int i, int j)
    {
        return squares[i, j];
    }

}

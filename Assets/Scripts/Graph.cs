using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Properties
    private Node[,] squares; //Was static
    private int graphWidth, graphHeight;
    public List<Node> Open { get; private set; }
    public List<Node> Closed { get; private set; }

    public Node CurrentNode { get; private set; }
    public int Size { get; private set; }
    public int OffSet { get; private set; }
    public int Generation { get; private set; }
    int numberOfWallSquares;
    int numberOfFreeSquares;
    int numberOfDeadEnds;
    int numberOfLongHorizontalWall;
    int numberOfLongVeticalWall;
    float score;

    //public Node[] PlayerGoalSquares { get; private set; }
    //public Node[] OpponentGoalSquares { get; private set; }
    #endregion

    private void SetProperties(int N, int M, int size, int offset, int generation)
    {
        Open = new List<Node>();
        Closed = new List<Node>();
        squares = new Node[M, N];
        graphWidth = N;
        graphHeight = M;
        Size = size;
        OffSet = offset;
        Generation = generation;
    }

    public Graph(int N, int M, int size, int offset, int generation)
    {
        SetProperties(N, M, size, offset, generation);
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < M; j++)
            {
                if ((i + 1 == N) || (j + 1 == M) || (j == 0) || (i == 0))
                {
                    squares[j, i] = new Node(j, i);
                }
                else
                {
                    squares[j, i] = new Node(j, i);
                }

            }
        }

        ConnectSquares(N, M, true);
        ConnectSquares(N, M, false);
    }

    private void ConnectSquares(int N, int M, bool horizontal) //Was static
    {
        if (horizontal) { N = N - 1; }
        else { M = M - 1; }
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < M; j++)
            {
                if (horizontal)
                {
                    squares[j, i].SetAdjacentSquare(squares[j, i + 1]);
                    squares[j, i + 1].SetAdjacentSquare(squares[j, i]);

                }
                else //ie vertical
                {
                    squares[j, i].SetAdjacentSquare(squares[j + 1, i]);
                    squares[j + 1, i].SetAdjacentSquare(squares[j, i]);
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

    public Node GetSquare(int j, int i)
    {
        return squares[j, i];
    }

}

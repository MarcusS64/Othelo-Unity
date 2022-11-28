using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    #region Properties
    private Graph parent;
    public Node[,] squares; //Was static
    private int graphWidth, graphHeight;
    public List<Node> Open { get; private set; }
    public List<Node> Closed { get; private set; }

    public Node CurrentNode { get; private set; }
    public int Generation { get; private set; }
    public void SetParent(Graph _parent) { parent = _parent; }
    public Graph GetParent() { return parent; }
    public Node nextMove;
    public int whiteNodes, blackNodes;
    public bool visited;

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
        ConnectEverySquare();
        //ConnectSquares(N, M, true);
        //ConnectSquares(N, M, false);
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

    private void ConnectSquares(int width, int height, bool horizontal) //Does not connect the diagonal
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

    private void ConnectEverySquare() //Does connect the diagonal
    {
        for (int i = 0; i < graphWidth; i++)
        {
            for (int j = 0; j < graphHeight; j++)
            {
                for (int k = 0; k < GameFlow.coords.Length; k++)
                {
                    int x = i - GameFlow.coords[k].x;
                    int y = j - GameFlow.coords[k].y;
                    if (x < graphWidth && y < graphHeight && x >= 0 && y >= 0) //If not then there is no square to connect
                    {
                        squares[i, j].SetAdjacentSquare(squares[x, y]);
                        squares[x, y].SetAdjacentSquare(squares[i, j]);
                    }
                }
            }
        }
    }

    public float CountBlackNodes()
    {
        blackNodes = 0;

        for (int i = 0; i < graphWidth; i++)
        {
            for (int j = 0; j < graphHeight; j++)
            {
                if (squares[i, j].color == Color.Black) blackNodes++;
            }
        }

        return blackNodes;
    }

    public float CountWhiteNodes()
    {
        whiteNodes = 0;

        for (int i = 0; i < graphWidth; i++)
        {
            for (int j = 0; j < graphHeight; j++)
            {
                if (squares[i, j].color == Color.White) whiteNodes++;
            }
        }

        return whiteNodes;
    }

    public void ProbeGraph(int startX, int startY, Color turnColor)
    {
        List<Probe> probes = new List<Probe>();
        for (int i = 0; i < GameFlow.coords.Length; i++)
        {
            probes.Add(new Probe(GameFlow.coords[i].x, GameFlow.coords[i].x, turnColor)); //need the agent color here
        }

        bool allProbesDone = false;

        do
        {
            foreach (Probe probe in probes)
            {               
                if (!probe.done)
                {
                    probe.Move();
                    if (probe.GetX() < graphWidth && probe.GetX() >= 0 && probe.GetY() < graphHeight && probe.GetY() >= 0)
                    {
                        if (squares[probe.GetX(), probe.GetY()].color == Color.None)
                        {
                            probe.done = true;
                        }
                        else if (squares[probe.GetX(), probe.GetY()].color != probe.currentStateTurn)
                        {
                            probe.nodesToFlip.Add(squares[probe.GetX(), probe.GetY()]);
                        }
                        else
                        {
                            probe.FlipNodes();
                            probe.done = true;
                        }
                    }
                    else
                    {
                        probe.done = true;
                    }
                }                                
            }

            int number = 0;
            foreach (Probe probe in probes)
            {               
                if (probe.done)
                {
                    number++;
                }
            }
            if(number == probes.Count)
            {
                allProbesDone = true;
            }

        } while (!allProbesDone);
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

    public void SetMove(Node move)
    {
        nextMove = move;
    }
}

using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UIElements;

public class Graph
{
    #region Properties
    private Graph parent;
    public Node[,] squares;
    private int graphWidth, graphHeight;

    public int Depth { get; private set; }
    public void SetParent(Graph _parent) { parent = _parent; }
    public Graph GetParent() { return parent; }
    public List<Graph> children;
    public List<Node> possibleMoves;
    public Node nextMove;
    public int whiteNodes, blackNodes;
    public bool visited;
    public Color currentTurnColor;

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
        children = new List<Graph>();
        possibleMoves = new List<Node>();
    }

    private void SetProperties(int N, int M, int depth)
    {
        squares = new Node[N, M];
        graphWidth = N;
        graphHeight = M;
        Depth = depth;
    }

    public void SetTurnColor(Color color)
    {
        currentTurnColor = color;
    }

    private void ConnectEverySquare() //Does connect the diagonal
    {
        for (int i = 0; i < graphWidth; i++)
        {
            for (int j = 0; j < graphHeight; j++)
            {
                for (int k = 0; k < GameFlow.coords.Length; k++)//Loops through all the squares in the 8 directions around the square
                {
                    int x = i - GameFlow.coords[k].x;
                    int y = j - GameFlow.coords[k].y;
                    if (x < graphWidth && y < graphHeight && x >= 0 && y >= 0) //If not then there is no square to connect
                    {
                        squares[i, j].SetAdjacentSquare(squares[x, y]);
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
            probes.Add(new Probe(GameFlow.coords[i].x, GameFlow.coords[i].y, turnColor)); //need the agent color here
            probes[i].SetNewStart(startX, startY);
        }

        bool allProbesDone = false;
        int number = 0;

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
                            number++;
                        }
                        else if (squares[probe.GetX(), probe.GetY()].color != probe.currentStateTurn)
                        {
                            probe.nodesToFlip.Add(squares[probe.GetX(), probe.GetY()]);
                        }
                        else
                        {
                            probe.FlipNodes();
                            probe.done = true;
                            number++;
                        }
                    }
                    else
                    {
                        probe.done = true;
                        number++;
                    }
                }                                
            }

            if(number >= probes.Count)
            {
                allProbesDone = true;
            }

        } while (!allProbesDone);
    }

    public void FindAvailableMoves() //Should be done once per board state
    {
        for (int i = 0; i < graphWidth; i++)
        {
            for (int j = 0; j < graphHeight; j++)
            {
                if (squares[i, j].GetColor() != Color.None)
                {
                    foreach (Node square in squares[i, j].adjacentSquares)
                    {
                        if (square.color == Color.None && !square.visited)
                        {
                            possibleMoves.Add(square);
                            square.visited = true;
                        }
                    }
                }
            }
        }
    }

    public void FindAvailableStates() //Create a new child graph for each possible move for the parent graph
    {
        foreach (Node move in possibleMoves)
        {
            Graph newBoardState = CopyParentToChild(this);
            CopyParentToChild(newBoardState);
            newBoardState.SetParent(this);
            newBoardState.SetMove(move);

            if (currentTurnColor == Color.White)//Curent turn of the parent 
            {
                newBoardState.squares[move.X(), move.Y()].SetColor(Color.Black);//Color of the next move to make
                newBoardState.ProbeGraph(move.X(), move.Y(), Color.Black);
                newBoardState.SetTurnColor(Color.Black);
            }
            else
            {
                newBoardState.squares[move.X(), move.Y()].SetColor(Color.White);
                newBoardState.ProbeGraph(move.X(), move.Y(), Color.White);
                newBoardState.SetTurnColor(Color.White);
            }

            children.Add(newBoardState);
        }
    }

    public int emptyNodes()
    {
        int count = 0;

        for (int i = 0; i < graphWidth; i++)
        {
            for (int j = 0; j < graphHeight; j++)
            {
                if (squares[i,j].color == Color.None) count++;
            }
        }

        return count;
    }

    private Graph CopyParentToChild(Graph parent)
    {
        Graph child = new Graph(parent.GetWidth(), parent.GetHeight(), Depth + 1);

        for (int i = 0; i < child.GetWidth(); i++)
        {
            for (int j = 0; j < child.GetHeight(); j++)
            {
                child.squares[i, j].color = parent.squares[i, j].color;
                child.squares[i, j].SetWorldPos(parent.squares[i, j].worldPosition.x, parent.squares[i, j].worldPosition.y);
            }
        }

        return child;
    }

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

    public Node GetMove()
    {
        return nextMove;
    }
}

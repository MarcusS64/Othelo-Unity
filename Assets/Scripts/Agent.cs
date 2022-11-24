using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    double timer;
    [SerializeField] double maxTimeOfSearch;
    [SerializeField] bool alphaBetaPruning;
    public bool active;

    Node node;
    Node bestPlacement;
    Graph graph;
    Color agentColor;
    bool myMove;
    List<Graph> possibleStates = new List<Graph>();
    List<Node> possibleMoves = new List<Node>();
    float alpha, beta;

    [SerializeField] Transform tokenObj_w;
    [SerializeField] Transform tokenObj_b;
    [SerializeField] Transform probeObj;

    [Header("Search Statistics")]
    [SerializeField] int depthOfSearch;
    [SerializeField] int amountOfNodesExamined;

    void Start()
    {
        //graph = FindObjectOfType<GameFlow>();
        timer = 0;
        depthOfSearch = 0;
        amountOfNodesExamined = 0;
        agentColor = Color.Black;
    }

    void Update()
    {
        if (active)
        {
            timer += Time.deltaTime;

            if (timer <= maxTimeOfSearch)
            {
                SearchBestMove();
            }
            else
            {
                PlaceToken();
                ResetAgent();
                active = false;
            }
        }
    }

    private void FindAvailableMoves(Graph board) //Should be done once per board state
    {
        graph = board;
        possibleMoves.Clear();
        for (int i = 0; i < board.GetWidth(); i++)
        {
            for (int j = 0; j < board.GetHeight(); j++)
            {
                if(graph.squares[i, j].GetColor() != Color.None)
                {
                    foreach (Node square in graph.squares[i, j].adjacentSquares)
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

    private void FindAvailableStates(Graph graph)
    {
        foreach (Node move in possibleMoves)
        {
            Graph newBoardState = graph;
            newBoardState.SetParent(graph);
            newBoardState.SetMove(move);
            if (myMove)
            {
                newBoardState.squares[move.X(), move.Y()].SetColor(agentColor);                
            }
            else
            {
                newBoardState.squares[move.X(), move.Y()].SetColor(Color.White);
            }
            
            possibleStates.Add(newBoardState);
        }

        if (myMove)
        {
            myMove = false;
        }
        else
        {
            myMove = true;
        }
        
    }

    private void SearchBestMove()
    {
        bestPlacement = null;
        alpha = -Mathf.Infinity;
        beta = Mathf.Infinity;


    }

    private void PlaceToken()
    {
        if (GameFlow.currenTurn == "White")
        {
            Instantiate(tokenObj_w, transform.position, tokenObj_w.rotation);
            GameFlow.currenTurn = "Black";
            GetComponent<BoxCollider2D>().enabled = false;
            Instantiate(probeObj, transform.position, probeObj.rotation);
        }
        else
        {
            Instantiate(tokenObj_b, transform.position, tokenObj_b.rotation);
            GameFlow.currenTurn = "White";
            GetComponent<BoxCollider2D>().enabled = false;
            Instantiate(probeObj, transform.position, probeObj.rotation);
        }
    }

    private void ResetAgent()
    {
        timer = 0;
        alpha= 0;
        beta = 0;
        bestPlacement = null;
    }
}

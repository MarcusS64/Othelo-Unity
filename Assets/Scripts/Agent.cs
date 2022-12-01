using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Agent : MonoBehaviour
{
    double timer;
    [SerializeField] double maxTimeOfSearch;
    [SerializeField] bool alphaBetaPruning;
    public bool active;

    Node node;
    Node bestPlacement;
    Graph bestBoardAlternative;
    Graph graph;
    Color agentColor;
    bool isMyTurn;
    Node myMove;
    List<List<Graph>> ListOfDepth = new List<List<Graph>>();
    List<Node> possibleMoves = new List<Node>();
    List<GameObject> probes = new List<GameObject>();
    //Stack<Graph> s = new Stack<Graph>();
    float alpha, beta;

    [SerializeField] Transform tokenObj_w;
    [SerializeField] Transform tokenObj_b;
    [SerializeField] GameObject probeObj;

    [Header("Search Statistics")]
    [SerializeField] int depthOfSearch;
    [SerializeField] int maxDepthOfSearch;
    [SerializeField] int amountOfNodesExamined;

    bool foundMove;

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

            if (timer <= maxTimeOfSearch && !foundMove)
            {
                //SearchBestMove();
                FindRandomMove();
            }
            else
            {
                PlaceToken();
                ResetAgent();
                active = false;
                foundMove = false;
            }
        }
    }

    private void FindRandomMove()
    {
        
        Graph currentBoard = new Graph(GameFlow.board.GetWidth(), GameFlow.board.GetHeight(), depthOfSearch);
        CopyParentToChild(currentBoard, GameFlow.board);
        currentBoard.SetTurnColor(agentColor);

        currentBoard.FindAvailableMoves();

        myMove = currentBoard.possibleMoves[Random.Range(0, possibleMoves.Count - 1)];

        foundMove = true;
    }

    private void FindAvailableMoves(Graph board) //Should be done once per board state
    {
        graph = board;
        possibleMoves.Clear();
        for (int i = 0; i < board.GetWidth(); i++)
        {
            for (int j = 0; j < board.GetHeight(); j++)
            {
                if (graph.squares[i, j].GetColor() != Color.None)
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

    private void FindAvailableStates(Graph parentGraph) //Create a new child graph for each possible move for the parent graph
    {
        List<Graph> possibleStates = new List<Graph>();
        foreach (Node move in possibleMoves)
        {
            Graph newBoardState = new Graph(parentGraph.GetWidth(), parentGraph.GetHeight(), depthOfSearch);
            CopyParentToChild(newBoardState, parentGraph);
            newBoardState.SetParent(parentGraph);
            newBoardState.SetMove(move);
            if (isMyTurn)
            {
                newBoardState.squares[move.X(), move.Y()].SetColor(agentColor);
                newBoardState.ProbeGraph(move.X(), move.Y(), agentColor);
            }
            else
            {
                newBoardState.squares[move.X(), move.Y()].SetColor(Color.White);
                newBoardState.ProbeGraph(move.X(), move.Y(), Color.White);
            }

            float numberOfBlackNodes = newBoardState.CountBlackNodes();

            possibleStates.Add(newBoardState);
        }

        ListOfDepth.Add(possibleStates);

        if (isMyTurn)
        {
            isMyTurn = false;
        }
        else
        {
            isMyTurn = true;
        }

    }

    private void CopyParentToChild(Graph child, Graph parent)
    {
        for (int i = 0; i < child.GetWidth(); i++)
        {
            for (int j = 0; j < child.GetHeight(); j++)
            {
                child.squares[i, j] = parent.squares[i, j];
                child.squares[i, j].visited = false;
            }
        }
    }

    private void SearchBestMove()
    {
        bestPlacement = null;
        depthOfSearch = 0;
        alpha = -Mathf.Infinity;
        beta = Mathf.Infinity;

        bestBoardAlternative = null;

        Graph currentBoard = new Graph(GameFlow.board.GetWidth(), GameFlow.board.GetHeight(), depthOfSearch);
        CopyParentToChild(currentBoard, GameFlow.board);
        currentBoard.SetTurnColor(agentColor);

        generateTree(currentBoard);

        //find bestBoardAlternative by comparing alpha and beta

        if (alphaBetaPruning) alphaBetaPruningAlgorithm(currentBoard);
        else minMaxAlgorithm(currentBoard);

        retracePath(currentBoard, bestBoardAlternative);
    }

    private void generateTree(Graph childboard)
    {
        childboard.FindAvailableMoves();
        childboard.FindAvailableStates();

        depthOfSearch++;

        foreach (Graph child in childboard.children)
        {
            if (depthOfSearch >= maxDepthOfSearch)
            {
                depthOfSearch--;
                return;
            }
            generateTree(child);
        }
    }

    private void alphaBetaPruningAlgorithm(Graph CurrentBoard)
    {

    }

    private void minMaxAlgorithm(Graph currentBoard)
    {

    }

    void retracePath(Graph startGraph, Graph goalGraph)
    {
        Graph currentGraph = goalGraph;

        while (currentGraph != startGraph)
        {
            currentGraph = currentGraph.GetParent();
        }
    }

    public void ToggleActivation()
    {
        active = !active;
    }

    private void PlaceToken()
    {
        //if (GameFlow.currentTurn == "White")
        //{
        //    Instantiate(tokenObj_w, transform.position, tokenObj_w.rotation);
        //    GameFlow.currentTurn = "Black";
        //    GetComponent<BoxCollider2D>().enabled = false;
        //    Instantiate(probeObj, transform.position, probeObj.rotation);
        //}
        //else
        //{
        //    Instantiate(tokenObj_b, transform.position, tokenObj_b.rotation);
        //    GameFlow.currentTurn = "White";
        //    GetComponent<BoxCollider2D>().enabled = false;
        //    Instantiate(probeObj, transform.position, probeObj.rotation);
        //}

        //transform.position should be the position on the board/move that it picked by myMove

        Vector2 tokenPos = myMove.worldPosition;

        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Empty");
        foreach (GameObject go in tiles)
        {
            if (Vector2.Distance(go.transform.position, myMove.worldPosition) < 1)
            {
                go.GetComponent<Collider2D>().enabled = false;
            }
        }

        probes.Clear();

        if (GameFlow.currentTurn == "White")
        {
            Instantiate(tokenObj_w, tokenPos, tokenObj_w.rotation);
            StartCoroutine(waitToChange());
            for (int i = 0; i < GameFlow.coords.Length; i++)
            {
                probes.Add(Instantiate(probeObj, tokenPos, transform.rotation));
                probes[i].GetComponent<ProbeMovement>().SetDirection(GameFlow.coords[i].x, GameFlow.coords[i].y);
            }
            GameFlow.totalWhite += 1;
            GameFlow.SetColorForSquare(tokenPos, Color.White);
        }
        else
        {
            Instantiate(tokenObj_b, tokenPos, tokenObj_b.rotation);
            StartCoroutine(waitToChange());
            for (int i = 0; i < GameFlow.coords.Length; i++)
            {
                probes.Add(Instantiate(probeObj, tokenPos, transform.rotation));
                if (probes[i].GetComponent<ProbeMovement>()) probes[i].GetComponent<ProbeMovement>().SetDirection(GameFlow.coords[i].x, GameFlow.coords[i].y);
            }
            GameFlow.totalBlack += 1;
            GameFlow.SetColorForSquare(tokenPos, Color.Black);
        }
    }

    IEnumerator waitToChange()
    {
        yield return new WaitForSeconds(4);
        if (GameFlow.currentTurn == "White")
        {
            GameFlow.currentTurn = "Black";
        }
        else
        {
            GameFlow.currentTurn = "White";
        }
        GameFlow.probeChange = Change.No;
        Debug.Log(GameFlow.currentTurn);
    }

    private void ResetAgent()
    {
        timer = 0;
        alpha = 0;
        beta = 0;
        bestPlacement = null;
    }
}

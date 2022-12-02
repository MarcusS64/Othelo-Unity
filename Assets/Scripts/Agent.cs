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
                SearchBestMove();
                //FindRandomMove();
            }
            else
            {
                PlaceToken();
                ResetAgent();
                active = false;
                foundMove = false;
                timer = 0;
            }
        }
    }

    private void FindRandomMove()
    {
        Graph currentBoard = CopyParentToChild(GameFlow.board);
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
            Graph newBoardState = CopyParentToChild(parentGraph); 
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

    private Graph CopyParentToChild(Graph parent)
    {
        Graph child = new Graph(parent.GetWidth(), parent.GetHeight(), depthOfSearch);

        for (int i = 0; i < child.GetWidth(); i++)
        {
            for (int j = 0; j < child.GetHeight(); j++)
            {
                Node n = new Node(i, j);

                n.color = parent.squares[i, j].color;
                n.adjacentSquares = parent.squares[i, j].adjacentSquares;
                n.worldPosition = parent.squares[i, j].worldPosition;
                n.x = parent.squares[i, j].x;
                n.y = parent.squares[i, j].y;

                child.squares[i, j] = n;
            }
        }

        return child;
    }

    private void SearchBestMove()
    {
        bestPlacement = null;
        depthOfSearch = 0;
        alpha = -Mathf.Infinity;
        beta = Mathf.Infinity;

        bestBoardAlternative = null;

        Graph currentBoard = CopyParentToChild(GameFlow.board);
        currentBoard.SetTurnColor(agentColor);

        generateTree(currentBoard);

        if (alphaBetaPruning) alphaBetaPruningAlgorithm(currentBoard);
        else minMaxAlgorithm(currentBoard);

        retracePath(bestBoardAlternative, currentBoard);
    }

    private void generateTree(Graph currentBoard)
    {
        currentBoard.FindAvailableMoves();
        currentBoard.FindAvailableStates();

        depthOfSearch++;

        foreach (Graph child in currentBoard.children)
        {
            if (depthOfSearch > maxDepthOfSearch)
            {
                depthOfSearch--;
                return;
            }
            generateTree(child);
        }
        depthOfSearch--;
    }

    private void alphaBetaPruningAlgorithm(Graph currentBoard)
    {
        if (currentBoard.children.Count() > 0)
        {
            foreach (Graph child in currentBoard.children)
            {
                alphaBetaPruningAlgorithm(child);
            }
        }

        if (currentBoard.Depth % 2 == 0)
        {
            if (currentBoard.CountBlackNodes() > alpha)
            {
                alpha = currentBoard.CountBlackNodes();
                bestBoardAlternative = currentBoard;
            }
            return;
        }
        else
        {
            if (currentBoard.CountBlackNodes() < beta)
            {
                beta = currentBoard.CountBlackNodes();
                bestBoardAlternative = currentBoard;
            }
            return;
        }
    }

    private void minMaxAlgorithm(Graph currentBoard)
    {

    }

    void retracePath(Graph leaf, Graph root)
    {
        Graph currentGraph = leaf;

        while (currentGraph.GetParent() != root && currentGraph.GetParent() != null)
        {
            currentGraph = currentGraph.GetParent();
        }

        myMove = currentGraph.GetMove();
        foundMove = true;
    }

    public void ToggleActivation()
    {
        active = !active;
    }

    private void PlaceToken()
    {
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
        yield return new WaitForSeconds(2);
        if (GameFlow.currentTurn == "White")
        {
            GameFlow.currentTurn = "Black";
        }
        else
        {
            GameFlow.currentTurn = "White";
            GameFlow.playerWhite = true;
        }
        GameFlow.probeChange = Change.No;
        Debug.Log(GameFlow.currentTurn);
    }

    private void ResetAgent()
    {
        timer = 0;
        alpha = -Mathf.Infinity;
        beta = Mathf.Infinity;
        bestPlacement = null;
    }
}

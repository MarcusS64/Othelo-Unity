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
    bool doOnce;

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
            if (!doOnce)
            {
                amountOfNodesExamined = 0;
                depthOfSearch = 0;
                doOnce = true;
            }

            if (timer <= maxTimeOfSearch && !foundMove)
            {
                SearchBestMove();
                //FindRandomMove();
            }
            else
            {
                PlaceToken();
                //ResetAgent();
                active = false;
                foundMove = false;
                doOnce = false;
                timer = 0;
            }
        }
    }

    private void FindRandomMove()
    {
        Graph currentBoard = CopyParentToChild(GameFlow.board);
        currentBoard.SetTurnColor(agentColor);

        currentBoard.FindAvailableMoves();

        myMove = currentBoard.possibleMoves[Random.Range(0, possibleMoves.Count)];

        foundMove = true;
    }

    private Graph CopyParentToChild(Graph parent)
    {
        Graph child = new Graph(parent.GetWidth(), parent.GetHeight(), depthOfSearch);

        for (int i = 0; i < child.GetWidth(); i++)
        {
            for (int j = 0; j < child.GetHeight(); j++)
            {
                child.squares[i, j].color = parent.squares[i, j].color;
                child.squares[i, j].SetWorldPos(parent.squares[i, j].worldPosition.x, parent.squares[i, j].worldPosition.y);
                //Node n = new Node(i, j);

                //n.color = parent.squares[i, j].color;
                //n.adjacentSquares = parent.squares[i, j].adjacentSquares;
                //n.worldPosition = parent.squares[i, j].worldPosition;
                //n.x = parent.squares[i, j].x;
                //n.y = parent.squares[i, j].y;

                //child.squares[i, j] = n;
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
        currentBoard.SetTurnColor(Color.White);

        generateTree(currentBoard);
        if (alphaBetaPruning) alphaBetaPruningAlgorithm(currentBoard);
        else minMaxAlgorithm(currentBoard);

        retracePath(bestBoardAlternative, currentBoard);
    }

    private void generateTree(Graph currentBoard)
    {
        depthOfSearch++;

        if (depthOfSearch > maxDepthOfSearch || currentBoard.emptyNodes() <= 0)
        {
            depthOfSearch--;
            return;
        }
        currentBoard.FindAvailableMoves();
        currentBoard.FindAvailableStates();

        foreach (Graph child in currentBoard.children)
        {
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
                amountOfNodesExamined++;
            }
            return;
        }
        else
        {
            if (currentBoard.CountBlackNodes() < beta)
            {
                beta = currentBoard.CountBlackNodes();
                bestBoardAlternative = currentBoard;
                amountOfNodesExamined++;
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
        //GameFlow.probeChange = Change.No;
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

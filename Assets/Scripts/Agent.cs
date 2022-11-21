using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    double timer;
    [SerializeField] double maxTime;
    public bool active;
    [SerializeField] bool alphaBetaPruning;

    Node node;
    Node bestPlacement;
    Graph graph;
    GameFlow gameFlow;

    [SerializeField] Transform tokenObj_w;
    [SerializeField] Transform tokenObj_b;
    [SerializeField] Transform probeObj;

    [Header("Search Statistics")]
    [SerializeField] int depthOfSearch;
    [SerializeField] int amountOfNodesExamined;

    void Start()
    {
        graph = GetComponent<Graph>();
        gameFlow = GetComponent<GameFlow>();
        timer = 0;
        depthOfSearch = 0;
        amountOfNodesExamined = 0;
    }

    void Update()
    {
        if (active)
        {
            timer += Time.deltaTime;

            if (timer <= maxTime)
            {
                searchBestMove();
            }
            else
            {
                placeToken();
                active = false;
                timer = 0;
            }
        }
    }

    private void searchBestMove()
    {

    }

    private void placeToken()
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
}

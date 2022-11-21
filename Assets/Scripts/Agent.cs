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
    List<Graph> possibleMoves = new List<Graph>();
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

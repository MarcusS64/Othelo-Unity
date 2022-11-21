using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickOn : MonoBehaviour
{
    public Transform tokenObj_w;
    public Transform tokenObj_b;
    public Transform probeObj;
    private int[,] directions = new int[,] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
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

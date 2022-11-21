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
    private List<Transform> probes;
    // Start is called before the first frame update
    void Start()
    {
        probes = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        ProbeMovement.destroyEvent += CheckProbes;
    }

    private void OnDisable()
    {
        ProbeMovement.destroyEvent -= CheckProbes;
    }

    private void OnMouseDown()
    {
        //if (GameFlow.nextReady)
        //{
        //    GameFlow.nextReady = false;
            if (GameFlow.currenTurn == "White")
            {
                Instantiate(tokenObj_w, transform.position, tokenObj_w.rotation);
                StartCoroutine(waitToChange());
                GetComponent<BoxCollider2D>().enabled = false;
                probes.Add(Instantiate(probeObj, transform.position, probeObj.rotation));
            GameFlow.totalWhite += 1;
            }
            else
            {
                Instantiate(tokenObj_b, transform.position, tokenObj_b.rotation);
                StartCoroutine(waitToChange());
                GetComponent<BoxCollider2D>().enabled = false;
                //for (int i = 0; i < directions.Length; i++)
                //{
                //    Instantiate(probeObj, transform.position, probeObj.rotation);
                //}
                probes.Add(Instantiate(probeObj, transform.position, probeObj.rotation));
            GameFlow.totalBlack += 1;
            }
            //gameObject.tag = "Occupied";
        //}


    }

    private void CheckProbes()
    {
        GameFlow.nextReady = true;
    }

    IEnumerator waitToChange()
    {
        yield return new WaitForSeconds(4);
        if(GameFlow.currenTurn == "White")
        {
            GameFlow.currenTurn = "Black";
        }
        else
        {
            GameFlow.currenTurn = "White";
        }
        GameFlow.probeChange = Change.No;
        Debug.Log(GameFlow.currenTurn);
    }
}

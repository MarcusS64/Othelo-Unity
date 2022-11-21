using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickOn : MonoBehaviour
{
    public Transform tokenObj_w;
    public Transform tokenObj_b;
    public GameObject probeObj;
    private int[,] directions = new int[,] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };
    (int x, int y)[] coords = new (int, int)[] { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };
    private List<GameObject> probes;

    // Start is called before the first frame update
    void Start()
    {
        probes = new List<GameObject>();
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
            var probe = Instantiate(probeObj, transform.position, transform.rotation);
                probes.Add(probe);
            probes[0].GetComponent<ProbeMovement>().SetDirection(coords[2].x, coords[2].y);
            GameFlow.totalWhite += 1;
            }
            else
            {
                Instantiate(tokenObj_b, transform.position, tokenObj_b.rotation);
                StartCoroutine(waitToChange());
                GetComponent<BoxCollider2D>().enabled = false;
            //for (int i = 0; i < directions.Length; i++)
            //{
            //    Instantiate(probeObj, transform.position, transform.rotation);
            //}
            var probe = Instantiate(probeObj, transform.position, transform.rotation);
            probes.Add(probe);
            probes[0].GetComponent<ProbeMovement>().SetDirection(coords[2].x, coords[2].y);
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

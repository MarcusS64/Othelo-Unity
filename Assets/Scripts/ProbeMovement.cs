using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProbeMovement : MonoBehaviour
{
    public static event Action destroyEvent;
    //public delegate void HandleProbeDestroyed();
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<CircleCollider2D>().enabled = false;
        //StartCoroutine(activateDelay());
        //GetComponent<Rigidbody2D>().velocity = new Vector2(-2, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDirection(int x, int y)
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(x, y) * 2;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Empty")
        {
            GameFlow.probeChange = Change.Reverse;
            Destroy(gameObject);
        }
        else if(collision.tag == GameFlow.currenTurn)
        {
            GameFlow.probeChange = Change.Yes;
            Destroy(gameObject);
        }
        else
        {
            collision.tag = gameObject.tag;
            //Destroy(gameObject);
        }

    }

    private void OnDestroy()
    {
        destroyEvent?.Invoke();
    }

    //IEnumerator activateDelay()
    //{
    //    yield return new WaitForSeconds(1);
    //    GetComponent<CircleCollider2D>().enabled = true;
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if(collision.tag == "Boundary")
    //    {
    //        Destroy(gameObject);
    //    }
    //}
}

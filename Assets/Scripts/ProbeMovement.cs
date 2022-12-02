using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProbeMovement : MonoBehaviour
{
    public static event Action destroyEvent;
    public List<GameObject> tokens;
    [SerializeField] int speed;
    //public delegate void HandleProbeDestroyed();
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<CircleCollider2D>().enabled = false;
        //StartCoroutine(activateDelay());
        //GetComponent<Rigidbody2D>().velocity = new Vector2(-2, 2);
        tokens = new List<GameObject>();
        Destroy(gameObject, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDirection(int x, int y)
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(x, y) * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "probeUL")
        {
            if (collision.tag == "Empty")
            {
                //GameFlow.probeChange = Change.Reverse;
                foreach (GameObject token in tokens)
                {
                    token.GetComponentInChildren<TokenController>().Reverse();
                }
                Destroy(gameObject);
            }
            else if (collision.tag == GameFlow.currentTurn)
            {
                //GameFlow.probeChange = Change.Yes;
                foreach (GameObject token in tokens)
                {
                    token.GetComponentInChildren<TokenController>().Yes();
                }
                Destroy(gameObject);
            }
            else
            {
                //collision.tag = gameObject.tag;
                tokens.Add(collision.gameObject);
                //Destroy(gameObject);
            }
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

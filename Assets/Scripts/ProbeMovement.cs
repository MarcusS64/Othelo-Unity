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

    // Start is called before the first frame update
    void Start()
    {
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
                foreach (GameObject token in tokens)
                {
                    token.GetComponentInChildren<TokenController>().Reverse();
                }
                Destroy(gameObject);
            }
            else if (collision.tag == GameFlow.currentTurn)
            {
                foreach (GameObject token in tokens)
                {
                    token.GetComponentInChildren<TokenController>().Yes();
                }
                Destroy(gameObject);
            }
            else
            {
                tokens.Add(collision.gameObject);
            }
        }
        

    }

}

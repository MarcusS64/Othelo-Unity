using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbeMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(-2, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

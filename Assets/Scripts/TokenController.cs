using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenController : MonoBehaviour
{
    public string currentColor;
    // Start is called before the first frame update
    void Start()
    {
        currentColor = gameObject.tag;
        GetComponent<CircleCollider2D>().enabled = false;
        StartCoroutine(activateDelay());
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.tag == "probeUL")
        {
            if((GameFlow.probeChange == Change.Yes) && (currentColor == "Black"))
            {
                GetComponent<SpriteRenderer>().color = new UnityEngine.Color(1, 1, 1);
                gameObject.tag = "White";
                GameFlow.totalBlack -= 1;
                GameFlow.totalWhite += 1;
                //currentColor = gameObject.tag;
            }
            else if ((GameFlow.probeChange == Change.Yes) && (currentColor == "White"))
            {
                GetComponent<SpriteRenderer>().color = new UnityEngine.Color(0, 0, 0);
                gameObject.tag = "Black";
                GameFlow.totalWhite -= 1;
                GameFlow.totalBlack += 1;
                //currentColor = gameObject.tag;
            }

            if ((GameFlow.probeChange == Change.Reverse) && (currentColor == "Black"))
            {
                gameObject.tag = "Black";
            }
            else if ((GameFlow.probeChange == Change.Reverse) && (currentColor == "White"))
            {
                gameObject.tag = "White";
            }
        }
    }

    IEnumerator activateDelay()
    {
        yield return new WaitForSeconds(2);
        GetComponent<CircleCollider2D>().enabled = true;
    }
}
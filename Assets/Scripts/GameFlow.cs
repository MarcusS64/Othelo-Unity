using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public Transform squareObj;
    private float width;
    private float height;
    [SerializeField] public int nrOfTiles = 4;
    public static string currenTurn;
    public static Change probeChange = Change.No;
    public Graph board;
    public static bool nextReady;
    [SerializeField] public static int totalWhite = 0;
    [SerializeField] public static int totalBlack = 0;
    // Start is called before the first frame update
    void Start()
    {
        
        width = squareObj.GetComponent<SpriteRenderer>().bounds.size.x;
        height = squareObj.GetComponent<SpriteRenderer>().bounds.size.y;
        float posXstart = -width * (nrOfTiles / 2);
        float posXend = width * (nrOfTiles / 2);
        float posYstart = -height * (nrOfTiles / 2);
        float posYend = height * (nrOfTiles / 2);
        Graph board = new Graph(nrOfTiles, nrOfTiles, 0);
        int xCoord = 0;
        int yCoord = 0;
        for (float x = posXstart; x < posXend; x += width) //float x = 0; x < width * nrOfTiles; x += width
        {
            //Instantiate(squareObj, new Vector2(x, 4), squareObj.rotation);
            //MAke another pair of ints to keep track of the index of the squares!!!
            for (float y = posYstart; y < posYend; y += height)
            {
                Instantiate(squareObj, new Vector2(x, y), squareObj.rotation);
                board.squares[xCoord, yCoord].SetWorldPos(x, y);
                yCoord++;
            }
            yCoord = 0;
            xCoord++;
        }
        currenTurn = "White";
        nextReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}

public enum Change
{
    Yes,
    No,
    Reverse
}

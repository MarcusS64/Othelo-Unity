using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public Transform squareObj;
    private static float width;
    private static float height;
    [SerializeField] public int nrOfTiles = 4;
    public static string currenTurn;
    public static (int x, int y)[] coords = new (int, int)[] { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };
    public static Change probeChange = Change.No; //Might need one for each direction
    public static Graph board;
    public static bool nextReady;
    public static float posXstart;
    public static float posXend;
    public static float posYstart;
    public static float posYend;
    [SerializeField] public static int totalWhite = 0;
    [SerializeField] public static int totalBlack = 0;
    [SerializeField] public static Agent agent;
    // Start is called before the first frame update
    void Start()
    {
        
        width = squareObj.GetComponent<SpriteRenderer>().bounds.size.x;
        height = squareObj.GetComponent<SpriteRenderer>().bounds.size.y;
        posXstart = -width * (nrOfTiles / 2);
        posXend = width * (nrOfTiles / 2);
        posYstart = -height * (nrOfTiles / 2);
        posYend = height * (nrOfTiles / 2);
        board = new Graph(nrOfTiles, nrOfTiles, 0);
        int xCoord = 0;
        int yCoord = 0;
        Debug.Log("Width: " + width);
        for (float x = posXstart; x < posXend; x += width) //float x = 0; x < width * nrOfTiles; x += width
        {
            //Instantiate(squareObj, new Vector2(x, 4), squareObj.rotation);
            for (float y = posYstart; y < posYend; y += height)
            {
                Instantiate(squareObj, new Vector2(x, y), squareObj.rotation);
                board.squares[xCoord, yCoord].SetWorldPos(x, y);
                //(x - posXstart) / width) + " " + (y - posYstart) / height);
                yCoord++;
            }
            yCoord = 0;
            
            //Debug.Log("x value: " + x + ". X coord value is: " + (x - posXstart) / width);
            xCoord++;
        }
        for (int i = 0; i < coords.Length; i++)
        {

        }

        currenTurn = "White";
        nextReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SetColorForSquare(float x, float y, Color newColor)
    {
        board.squares[(int)((x - posXstart) / width), (int)((y - posYstart) / height)].SetColor(newColor);
    }

    public static Vector2 GetSquareToWorldPos(int squareX, int squareY)
    {
        return new Vector2(squareX * width + posXstart, squareY * height + posYstart);
    }

    public static void ActivateAgent()
    {
        agent.active = true;
    }
}

public enum Change
{
    Yes,
    No,
    Reverse
}

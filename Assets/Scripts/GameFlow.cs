using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public Transform squareObj;
    private static float width;
    private static float height;
    [SerializeField] public int nrOfTiles = 4;
    public static string currentTurn;
    public static (int x, int y)[] coords = new (int, int)[] { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };
    //public static Change probeChange = Change.No; 
    public static Graph board;
    public static bool nextReady;
    public static float posXstart;
    public static float posXend;
    public static float posYstart;
    public static float posYend;
    [SerializeField] public static int totalWhite = 0;
    [SerializeField] public static int totalBlack = 0;
    [SerializeField] public static Agent agent;

    public static bool playerWhite;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<Agent>();

        width = squareObj.GetComponent<SpriteRenderer>().bounds.size.x;
        height = squareObj.GetComponent<SpriteRenderer>().bounds.size.y;
        posXstart = -width * (nrOfTiles / 2);
        posXend = width * (nrOfTiles / 2);
        posYstart = -height * (nrOfTiles / 2);
        posYend = height * (nrOfTiles / 2);
        board = new Graph(nrOfTiles, nrOfTiles, 0);
        int xCoord = 0;
        int yCoord = nrOfTiles - 1;

        for (float x = posXstart; x < posXend; x += width) 
        {
            for (float y = posYstart; y < posYend; y += height)
            {
                Instantiate(squareObj, new Vector2(x + width / 2, y + height / 2), squareObj.rotation);
                board.squares[xCoord, yCoord].SetWorldPos(x + width / 2, y + height / 2);
                //(x - posXstart) / width) + " " + (y - posYstart) / height);
                yCoord--;
            }

            yCoord = nrOfTiles - 1;          
            xCoord++;
        }

        currentTurn = "White";
        playerWhite = true;
        nextReady = true;
    }

    public static void SetColorForSquare(Vector2 worldPos, Color newColor)
    {
        for (int i = 0; i < board.GetWidth(); i++)
        {
            for (int j = 0; j < board.GetHeight(); j++)
            {
                if (Vector2.Distance(board.squares[i, j].worldPosition, worldPos) <= 0.3f) 
                {
                    board.squares[i, j].SetColor(newColor);
                    //Debug.Log("Square to set color: " + i + ", " + j);
                }

            }
        }
        //board.squares[(int)((worldPos.x - posXstart) / width), (int)((worldPos.y - posYstart) / height)].SetColor(newColor);
    }

    public static Vector2 GetSquareToWorldPos(int squareX, int squareY)
    {
        return new Vector2(squareX * width + posXstart, squareY * height + posYstart);
    }

}

public enum Change
{
    Yes,
    No,
    Reverse
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    public int depth = 20;
    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    [SerializeField] float coastLevel;

    float[,] noiseMap;
    Node[,] squares;
    [SerializeField] int startTokens;
    [SerializeField] int startX;
    [SerializeField] int startY;
    [SerializeField] int limit;
    [SerializeField] int numberOfChildren;
    [SerializeField] int smoothTokens;
    [SerializeField] int mountainTokens;
    [SerializeField] int mountainTurnLimit;
    //[SerializeField] CoastalAgent CoastalAgent;
    private void Start()
    {
        squares = new Node[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
               squares[i, j] = new Node(i, j);
            }
        }
        ConnectSquares(width, height, true);
        ConnectSquares(width, height, false);
        var nodes = GetRandomNode(squares[startX, startY]);
        squares[startX, startY].visited = true;

        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    private void Update() //Update for testing purposes, should be in Start
    {
        

        //offsetX -= Time.deltaTime * 5f;
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        //Debug.Log("Terrain generate called");
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights()); //Takes the height map array values to set the heights of the terrain
        return terrainData;
    }

    float[,] GenerateHeights() //Height values for the noise map
    {
        float[,] heights = new float[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //heights[i, j] = squares[i, j].GetHeight();
            }
        }
        return heights;
    }

    private void ConnectSquares(int width, int height, bool horizontal) //Was static
    {
        if (horizontal) { width--; }
        else { height--; }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (horizontal)
                {
                    squares[i, j].SetAdjacentSquare(squares[i + 1, j ]);
                    squares[i + 1, j].SetAdjacentSquare(squares[i, j]);

                }
                else //ie vertical
                {
                    squares[i, j].SetAdjacentSquare(squares[i, j + 1]);
                    squares[i, j + 1].SetAdjacentSquare(squares[i, j]);
                }
            }
        }
    }


    private (Node, Node) GetRandomNode(Node start) //Might have to check that it's not the one we're standing on already as a start
    {
        Node repulsor = squares[Random.Range(0, width), Random.Range(0, height)];
        Node attractor = squares[Random.Range(0, width), Random.Range(0, height)];
        return (repulsor, attractor);
    }

}

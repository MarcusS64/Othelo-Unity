using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{

    public class Graph
    {
        #region Properties
        private Node[,] squares; //Was static
        private int graphWidth, graphHeight;
        public List<Node> Open { get; private set; }
        public List<Node> Closed { get; private set; }

        public Node CurrentNode { get; private set; }
        public int Size { get; private set; }
        public int OffSet { get; private set; }
        public int Generation { get; private set; }
        Random rnd;
        int numberOfWallSquares;
        int numberOfFreeSquares;
        int numberOfDeadEnds;
        int numberOfLongHorizontalWall;
        int numberOfLongVeticalWall;
        float score;

        //public Node[] PlayerGoalSquares { get; private set; }
        //public Node[] OpponentGoalSquares { get; private set; }
        #endregion

        private void SetProperties(int N, int M, int size, int offset, int generation)
        {
            Open = new List<Node>();
            Closed = new List<Node>();
            squares = new Node[M, N];
            graphWidth = N;
            graphHeight = M;
            Size = size;
            OffSet = offset;
            Generation = generation;
            rnd = new Random();
        }

        public Graph(Graph parentA, Graph parentB, int generation)
        {
            SetProperties(parentA.GetWidth(), parentA.GetHeight(), parentA.Size, parentA.OffSet, generation);
            int randomSplit = rnd.Next(4, graphWidth - 4);
            for (int i = 0; i < graphWidth; i++)
            {
                for (int j = 0; j < graphHeight; j++)
                {
                    if(i < randomSplit)
                    {
                        squares[j, i] = parentA.GetSquare(j, i);
                    }
                    else
                    {
                        squares[j, i] = parentB.GetSquare(j, i);
                    }                    
                }
            }
        }

        public Graph(int N, int M, int size, int offset, int generation)
        {
            SetProperties(N, M, size, offset, generation);
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    if((i + 1 == N) || (j + 1 == M) || (j == 0) || (i == 0))
                    {
                        squares[j, i] = new Node(j, i, size, offset, true);
                    }
                    else
                    {
                        squares[j, i] = new Node(j, i, size, offset, false);
                    }
                    
                }
            }

            ConnectSquares(N, M, true);
            ConnectSquares(N, M, false);
            PlaceBlock(N, M);
            MirrorMap(N, M);
        }

        public void Evolve()
        {
            ClearOuterRim();
            for (int i = 0; i < 3; i++)
            {
                FillVoid(graphWidth, graphHeight);
                OpenDivide(graphWidth, graphHeight);
                
            }
            MirrorMap(graphWidth, graphHeight);
        }

        private void ConnectSquares(int N, int M, bool horizontal) //Was static
        {
            if (horizontal) { N = N - 1; }
            else { M = M - 1; }
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    if (horizontal)
                    {
                        squares[j, i].SetAdjacentSquare(squares[j, i + 1]);
                        squares[j, i + 1].SetAdjacentSquare(squares[j, i]);

                    }
                    else //ie vertical
                    {
                        squares[j, i].SetAdjacentSquare(squares[j + 1, i]);
                        squares[j + 1, i].SetAdjacentSquare(squares[j, i]);
                    }
                }
            }
        }

        private void FillVoid(int N, int M)//Sets square as blocked if all around it are blocked
        {
            for (int i = 1; i < N - 1; i++)
            {
                for (int j = 1; j < M / 2; j++)
                {
                    if(squares[j, i].AllBlocked()) //|| squares[j, i].DeadEnd()
                    {
                        squares[j, i].blocked = true;
                    }
                }
            }
        }

        private void OpenDivide(int N, int M)//Removes horizontal or vertical block
        {
            Random rnd = new Random();
            for (int i = 2; i < N - 2; i++)
            {
                for (int j = 2; j < M / 2; j++)
                {
                    if (squares[j, i].IsDividing() && !squares[j, i].swapped && rnd.Next(0, 100) > 70)
                    {
                        squares[j, i].SetUnlocked();
                        squares[j, i].swapped = true;
                    }
                }
            }
            //MirrorMap(N, M);
        }

        private void FillDeadEnd(int N, int M)
        {
            for (int i = 2; i < N - 2; i++)
            {
                for (int j = 2; j < M / 2; j++)
                {
                    if (squares[j, i].DeadEnd())
                    {
                        squares[j, i].SetBlocked();                        
                    }
                }
            }
        }

        public void Mutate()
        {
            FillVoid(graphWidth, graphHeight);
            FillDeadEnd(graphWidth, graphHeight);
            for (int i = 0; i < 4; i++)
            {
                OpenDivide(graphWidth, graphHeight);
                OpenPath(graphWidth, graphHeight);                
            }
            MirrorMap(graphWidth, graphHeight);
        }

        public void ClearOuterRim()
        {
            for (int i = 1; i < graphWidth - 1; i++)
            {
                for (int j = 1; j < graphHeight - 1; j++)
                {
                    if ((i + 2 == graphWidth) || (j + 2 == graphHeight) || (j == 1) || (i == 1))
                    {
                        squares[j, i].SetUnlocked();
                    }
                }
            }
            //MirrorMap(N, M);
        }

        private void PlaceBlock(int N, int M)
        {
            for (int i = 2; i < N - 2; i++)
            {
                for (int j = 2; j < M / 2; j++)
                {
                    squares[j, i].BlockPath();
                }
            }
        }

        private void OpenPath(int N, int M)
        {
            Random rnd = new Random();
            for (int i = 2; i < N - 2; i++)
            {
                for (int j = 2; j < M / 2; j++)
                {
                    if (squares[j, i].AllBlocked() && squares[j, i].blocked && rnd.Next(0, 100) > 70)
                    {
                        squares[j, i].OpenPath();
                    }
                }
            }
            //MirrorMap(N, M);
        }

        private void MirrorMap(int N, int M)
        {
            int k = 0;
            for (int i = 1; i < N - 1; i++)
            {
                k = 0;
                for (int j = M / 2; j < M - 1; j++)
                {
                    if(squares[j - (k + 1), i].blocked)
                    {
                        squares[j, i].SetBlocked();
                    }
                    else
                    {
                        squares[j, i].SetUnlocked();
                    }
                    k += 2;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < graphWidth; i++)
            {
                for (int j = 0; j < graphHeight; j++)
                {
                    squares[j, i].Update(gameTime);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < graphWidth; i++)
            {
                for (int j = 0; j < graphHeight; j++)
                {
                    squares[j, i].Draw(spriteBatch);
                }
            }
        }
        #region Pathfinding
        public Node FindSquare(int x, int y)
        {
            foreach (Node square in squares)
            {
                if (x == square.X() && y == square.Y())
                {
                    return square;
                }
            }
            return null;
        }

        public void SetCurrentNode(Node square)
        {
            CurrentNode = square;
        }

        public void RepairLinks(Node first, Node removedFromFirst, Node second, Node removedFromSecond)
        {
            first.SetAdjacentSquare(removedFromFirst);
            removedFromFirst.SetAdjacentSquare(first);

            second.SetAdjacentSquare(removedFromSecond);
            removedFromSecond.SetAdjacentSquare(second);
        }
        #endregion

        public int GetWidth()
        {
            return graphWidth;
        }

        public int GetHeight()
        {
            return graphHeight;
        }

        public Node GetSquare(int j , int i)
        {
            return squares[j, i];
        }

        public float GetScore()
        {
            return score;
        }

        public void Evaluate()
        {
            int wallSequence = 0;
            for (int i = 1; i < graphWidth - 1; i++)
            {
                for (int j = 1; j < graphHeight - 1; j++)
                {
                    if (squares[j, i].blocked)
                    {
                        numberOfWallSquares++;
                        wallSequence++;
                        if(wallSequence > 6)
                        {
                            numberOfLongHorizontalWall++;
                        }
                    }
                    else
                    {
                        numberOfFreeSquares++;
                        wallSequence = 0;
                    }

                    if(squares[j, i].DeadEnd())
                    {
                        numberOfDeadEnds++;
                    }
                }
            }
            wallSequence = 0;
            for (int i = 1; i < graphHeight - 1; i++)
            {
                for (int j = 1; j < graphWidth - 1; j++)
                {
                    if (squares[i, j].blocked)
                    {
                        wallSequence++;
                        if (wallSequence > 6)
                        {
                            numberOfLongVeticalWall++;
                        }
                    }
                    else
                    {
                        wallSequence = 0;
                    }
                }
            }
            score = (float)(numberOfFreeSquares / numberOfWallSquares) + (float)(numberOfWallSquares / numberOfFreeSquares) - 2 + numberOfDeadEnds * 0.05f
                + (numberOfLongHorizontalWall + numberOfLongVeticalWall) * 0.5f;
        }
    }
}

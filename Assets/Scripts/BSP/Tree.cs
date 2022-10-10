using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using SCC;
using Random = System.Random;
using Terrain = SCC.Terrain;
namespace SCC
{
    public class Tree
    {
        public TreeNode root { get; set; }
    }

    public class TreeNode
    {
        public static Random rand = new Random();
        public int width, height, x, y;
        public int newW, newH, newX, newY;

        public int[,] room;
        public bool vertical;
        public bool isLeaf = false;
        public bool connected = false;

        public TreeNode leftChild;
        public TreeNode rightChild;
        public TreeNode sibling;
        public TreeNode parent;
        public Dictionary<TreeNode, bool> neighbours = new Dictionary<TreeNode, bool>();


        public TreeNode(int x, int y, int width, int height)
        {
            this.width = width;
            this.height = height;
            this.x = x;
            this.y = y;
            this.room = new int[width, height];
        }
        public TreeNode() : this(0, 0, 0, 0)
        {

        }
        public void Randomize()
        {
            newW = rand.Next(this.width / 2, this.width);
            newH = rand.Next(this.height / 2, this.height);
            if(newW < 4)
                newW = 4;
            if(newH < 4)
                newH = 4;
            newX = XMax() - newW;
            newY = YMax() - newH;


            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    room[i, j] = Terrain.VOID;//temp wall
                }
            }
            
            for (int i = 0; i < newW; i++)
            {
                for (int j = 0; j < newH; j++)
                {
                    
                    if (i == 0 || j == 0 || i == (newW - 1) || j == (newH - 1))
                        room[i + newX - x, j + newY - y] = Terrain.WALL;
                    else
                        room[i + newX - x, j + newY - y] = Terrain.GROUND;
                    
                    //Debug.Log(newW + ", " + newH + ", " + width + ", " + height);
                }
            }
        }
        public void SetRoom(int[,] map)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (map[i + x, j + y] != Terrain.GROUND)
                        map[i + x, j + y] = room[i, j];
                }
            }
        }

        public void SetRoom2(int[,] map)
        {
            for (int i = 0; i < width - 1; i++)
            {
                for (int j = 0; j < height - 1; j++)
                {
                    if (map[i, j] != Terrain.GROUND)
                        map[i, j] = room[i, j];
                }
            }
        }

        public void SaveRoom()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i == 0 || j == 0 || i == (width - 1) || j == (height - 1))
                        room[i, j] = Terrain.WALL;
                    else
                        room[i, j] = Terrain.GROUND;
                }
            }
        }

        public void LoadRoom()
        {
            if (this.leftChild == null || this.rightChild == null)
                return;
            this.room = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                    this.room[i, j] = Terrain.VOID;//temp wall
            }

            for (int i = 0; i < leftChild.width; i++)
            {
                for (int j = 0; j < leftChild.height; j++)
                {
                    this.room[i + leftChild.x - this.x, j + leftChild.y - this.y] = leftChild.room[i, j];
                }
            }
            for (int i = 0; i < rightChild.width; i++)
            {
                for (int j = rightChild.y - this.y; j < rightChild.height; j++)
                {
                    this.room[i + rightChild.x - this.x, j + rightChild.y - this.y] = rightChild.room[i, j];
                }
            }
        }

        public void PrintRoom()
        {
            //for (int i = 0; i < height; i++)
            //{
              //  for (int j = 0; j < width; j++)
                //Console.Write(room[j, i]);
                //Console.WriteLine();
        //}

        }
        public void PrintValue()
        {
            //Console.Write("Position : (" + x + ", " + y + "), ");
            //Console.WriteLine("Size : " + width + "x" + height);
        }
        public int XMax()
        {
            return x + width - 1;
        }
        public int YMax()
        {
            return y + height - 1;
        }
        public Vector2 Point()
        {
            return new Vector2(rand.Next(x + 1, XMax()), rand.Next(y + 1, YMax()));
        }
        public Vector2 Center()
        {
            return new Vector2((int)((x + XMax()) / 2), (int)((y + YMax()) / 2));
        }
        public Vector2 newCenter()
        {
            return new Vector2((int)((newX * 2 - 1) / 2), (int)((newY * 2 - 1) / 2));
        }
        public double Radius()
        {
            return Math.Sqrt(Math.Pow(width / 2, 2) + Math.Pow(height / 2, 2));
        }
        public double newRadius()
        {
            return Math.Sqrt(Math.Pow(newW / 2, 2) + Math.Pow(newH / 2, 2));
        }

        public bool Connect(int[,] map)
        {
            if(this.sibling == null || this.connected)
                return false;
            Vector2 Point1, Point2;
            int count = 1;
            while (true)
            {
                Point1 = new Vector2(rand.Next(x + 1, XMax()), rand.Next(y + 1, YMax()));
                Point2 = new Vector2(rand.Next(sibling.x + 1, sibling.XMax()), rand.Next(sibling.y + 1, sibling.YMax()));
                if (map[(int)Point1.x, (int)Point1.y] == Terrain.GROUND && map[(int)Point2.x, (int)Point2.y] == Terrain.GROUND)
                    break;
                count++;
                if(count > 9999)
                {
                    Debug.Log("Cannot Find Point.");
                    return false;
                }
            }
            int x1, x2, y1, y2;

            if (Point1.y < Point2.y)
            {
                y1 = (int)Point1.y;
                y2 = (int)Point2.y;
            }
            else if (Point1.y > Point2.y)
            {
                y1 = (int)Point2.y;
                y2 = (int)Point1.y;
            }
            else
            {
                y1 = (int)Point1.y;
                y2 = y1;
            }

            if (Point1.x < Point2.x)
            {
                x1 = (int)Point1.x;
                x2 = (int)Point2.x;

                for (int i = x1; i <= x2; i++)
                {
                    for (int j = y1; j <= y2; j++)
                    {
                        if (i == x1 || j == (int)Point2.y)
                        {
                            map[i, j] = Terrain.GROUND;
                        }
                    }
                }
            }
            else if (Point1.x > Point2.x)
            {
                x1 = (int)Point2.x;
                x2 = (int)Point1.x;

                for (int i = x1; i <= x2; i++)
                {
                    for (int j = y1; j <= y2; j++)
                    {
                        if (i == x1 || j == (int)Point1.y)
                        {
                            map[i, j] = Terrain.GROUND;
                        }
                    }
                }
            }
            else
            {
                if (y1 == y2)
                {
                    Debug.Log("Same Point Error.");
                    return false;
                }
                x1 = (int)Point1.x;
                x2 = x1;

                for (int i = x1; i <= x2; i++)
                {
                    for (int j = y1; j <= y2; j++)
                    {
                        if (i == x1)
                        {
                            map[i, j] = Terrain.GROUND;
                        }
                    }
                }
            }

            this.connected = true;
            this.sibling.connected = true;
            return true;
        }
        public bool SCCConnect(int[,] map, TreeNode node)
        {
            if (!neighbours.ContainsKey(node) || !node.neighbours.ContainsKey(this) || neighbours[node] == true)//skip connected rooms.
            {
                //Debug.Log("skip.");
                return false;
            }
            Vector2 Point1, Point2;
            int count = 1;
            while (true)
            {
                Point1 = new Vector2(rand.Next(newX + 1, newX + newW - 1), rand.Next(newY + 1, newY + newH - 1));
                Point2 = new Vector2(rand.Next(node.newX + 1, node.newX + node.newW - 1), rand.Next(node.newY + 1, node.newY + node.newH - 1));
                if (map[(int)Point1.x, (int)Point1.y] == Terrain.GROUND && map[(int)Point2.x, (int)Point2.y] == Terrain.GROUND)
                    break;
                count++;
                if (count > 9999)
                {
                    Debug.Log("Cannot Find Point.");
                    return false;
                }
            }
            int x1, x2, y1, y2;

            if (Point1.y < Point2.y)
            {
                y1 = (int)Point1.y;
                y2 = (int)Point2.y;
            }
            else if (Point1.y > Point2.y)
            {
                y1 = (int)Point2.y;
                y2 = (int)Point1.y;
            }
            else
            {
                y1 = (int)Point1.y;
                y2 = y1;
            }

            if (Point1.x < Point2.x)
            {
                x1 = (int)Point1.x;
                x2 = (int)Point2.x;

                for (int i = x1; i <= x2; i++)
                {
                    for (int j = y1; j <= y2; j++)
                    {
                        if (i == x1 || j == (int)Point2.y)
                        {
                            map[i, j] = Terrain.GROUND;
                        }
                    }
                }
            }
            else if (Point1.x > Point2.x)
            {
                x1 = (int)Point2.x;
                x2 = (int)Point1.x;

                for (int i = x1; i <= x2; i++)
                {
                    for (int j = y1; j <= y2; j++)
                    {
                        if (i == x1 || j == (int)Point1.y)
                        {
                            map[i, j] = Terrain.GROUND;
                        }
                    }
                }
            }
            else
            {
                if (y1 == y2)
                {
                    Debug.Log("Same Point Error.");
                    return false;
                }
                x1 = (int)Point1.x;
                x2 = x1;

                for (int i = x1; i <= x2; i++)
                {
                    for (int j = y1; j <= y2; j++)
                    {
                        if (i == x1)
                        {
                            map[i, j] = Terrain.GROUND;
                        }
                    }
                }
            }

            neighbours[node] = true;
            node.neighbours[this] = true;
            return true;
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCC;
using UnityEngine;
using Random = System.Random;
using Debug = UnityEngine.Debug;

namespace SCC
{
    public class BSP
    {
        public int[,] map;
        public Tree tree;
        public List<TreeNode> rooms;
        public int width;
        public int height;

        public float minSlice;
        public float maxSlice;
        public int minSize;
        public int maxSize;


        public static Random rand = new Random();

        public BSP(int width, int height, float minSl, float maxSl, int minSi, int maxSi)
        {
            this.width = width;
            this.height = height;
            map = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                    map[i, j] = Terrain.VOID;//temp wall
            }
            tree = new Tree();
            tree.root = new TreeNode(0, 0, width, height);
            rooms = new List<TreeNode>();

            minSlice = minSl;
            maxSlice = maxSl;
            minSize = minSi;
            maxSize = maxSi;
        }

        public bool Divide(TreeNode node)
        {
            if ((node.width <= maxSize && node.height <= maxSize) || node == null || (node.width < minSize * 2 && node.height < minSize * 2))
            {
                if (node != null)
                {
                    rooms.Add(node);
                    node.isLeaf = true;
                }
                return false;
            }


            bool vertical = false;
            if (node.width > node.height)
                vertical = true;

            int minValue;
            int maxValue;

            if (vertical)
            {
                minValue = (int)(node.width * minSlice);
                if (minValue < minSize - 1)
                    minValue = minSize - 1;
                maxValue = (int)(node.width * maxSlice);
                if (maxValue > maxSize)
                    maxValue = maxSize;
                if (minValue > maxValue)
                    minValue = maxValue;
                int split = node.x + rand.Next(minValue, maxValue);
                node.leftChild = new TreeNode(node.x, node.y, split - node.x + 1, node.height);
                node.rightChild = new TreeNode(split + 1, node.y, node.XMax() - split, node.height);
                node.leftChild.sibling = node.rightChild;
                node.rightChild.sibling = node.leftChild;
                node.leftChild.parent = node;
                node.rightChild.parent = node;
                node.leftChild.vertical = true;
                node.rightChild.vertical = true;
            }
            else
            {
                minValue = (int)(node.height * minSlice);
                if (minValue < minSize - 1)
                    minValue = minSize - 1;
                maxValue = (int)(node.height * maxSlice);
                if (maxValue > maxSize)
                    maxValue = maxSize;
                if (minValue > maxValue)
                    minValue = maxValue;
                int split = node.y + rand.Next(minValue, maxValue);
                node.leftChild = new TreeNode(node.x, node.y, node.width, split - node.y + 1);
                node.rightChild = new TreeNode(node.x, split + 1, node.width, node.YMax() - split);
                node.leftChild.sibling = node.rightChild;
                node.rightChild.sibling = node.leftChild;
                node.leftChild.parent = node;
                node.rightChild.parent = node;
                node.leftChild.vertical = false;
                node.rightChild.vertical = false;
            }

            return true;
        }

        public void DivideLevel(TreeNode node)
        {
            if (Divide(node))
            {
                DivideLevel(node.leftChild);
                DivideLevel(node.rightChild);
                if (node.leftChild != null && node.rightChild != null)
                {
                    node.leftChild.Connect(map);
                    //node.rightChild.Connect(map);
                }
            }
            node.LoadRoom();
            if (node.isLeaf)
            {
                node.Randomize();
                node.SetRoom(map);
            }
        }

        public void DivideLevel2(TreeNode node)
        {
            if (Divide(node))
            {
                DivideLevel2(node.leftChild);
                DivideLevel2(node.rightChild);  
            }
            if (node.isLeaf)
            {
                node.Randomize();
                node.SetRoom(map);
            }
        }

        public void ResetLevel()
        {
            map = new int[width, height];
            tree = new Tree();
            tree.root = new TreeNode(50, 50, 0, 0);
            rooms = new List<TreeNode>();
        }

        public void MakeLevel()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                    map[i, j] = Terrain.VOID;//temp wall
            }
            SelectRooms((int)(width / 4), (int)(width / 2));
            MakeNeighbours();
            //Debug.Log(rooms.Count);
            foreach (TreeNode node in rooms)
                node.SetRoom(map);
            foreach (TreeNode node in rooms)
            {
                //node.SetRoom(map);
                foreach(TreeNode node2 in rooms)
                    node.SCCConnect(map, node2);
            }
            
                
        }

        //Functions for BSP-based SCC.
        public void SelectRooms(int minR, int maxR)//Select Rooms in range of min~max radius of the circle.
        {
            Vector2 center = new Vector2((int)(width / 2), (int)(height / 2));
            for (int i = 0; i < rooms.Count; i++)
            {
                Vector2 point = new Vector2(rooms[i].x, rooms[i].y);
                if (Vector2.Distance(center, point) < minR || Vector2.Distance(center, point) > maxR)
                {
                    rooms.RemoveAt(i);
                    i--;
                }
            }
        }
        public void MakeNeighbours()
        {
            for(int i = 0; i < rooms.Count; i++)
            {
                for(int j = 0; j < rooms.Count; j++)
                {
                    if (i == j || rooms[i].neighbours.ContainsKey(rooms[j]))
                        continue;
                    double dist = Vector2.Distance(rooms[i].newCenter(), rooms[j].newCenter());
                    double zdist = rooms[i].newRadius() + rooms[j].newRadius();
                    if(dist - zdist > 0 && dist - zdist <= dist * 0.4f)
                    {
                        //Debug.Log("Connection Success.");
                        rooms[i].neighbours.Add(rooms[j], false);
                        rooms[j].neighbours.Add(rooms[i], false);
                    }
                }
            }
        }


        public void PrintLevel()
        {
            for (int i = 0; i < width; i++)
            {
                String line = "";
                for (int j = 0; j < height; j++)
                {
                    line += map[i, j];
                    line += " ";
                }
                Debug.Log(line);
            }
        }
    }


}

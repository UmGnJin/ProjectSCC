using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = System.Random;


namespace SCC
{
    public abstract class Level
    {
        public int radius = 0;
        public int width, height, length;
        public int[,] map;

        public bool locked = false;

        public List<Room> rooms;

        public Rect levelr;
        public static Random rand = new Random();


        public void Create()
        {
            Random random = new Random();
            InitRooms();
        }
        public abstract void InitRooms();
        public abstract void PlaceRooms();
        public bool MoveRoom(Room r, int xDir, int yDir, out int index)//방을 옮기면서 다른 방과 겹치는지를 확인한다. 겹치면 false가 나오고, index를 통해 겹친 방의 번호가 나온다.
        {
            r.MovePosition(xDir, yDir);
            int i = -1;
            if (CheckOverlap(r, out i))
            {
                r.MovePosition(-xDir, -yDir);
                index = i;
                //Debug.Log(i);
                return false;
            }
            index = -1;
            return true;
        }
        public bool CheckOverlap(Room r1, Room r2) // 매개변수인 두 방의 겹침 여부 확인.
        {
            Rect rect = r1.Intersect(r2);
            if (rect.Width() > 0 && rect.Height() > 0)
                return true;
            return false;
        }
        public bool CheckOverlap(Room r) // 이 방과 겹치는 방이 있는지 모든 방을 검사함.
        {
            foreach (Room r1 in rooms)
            {
                //if ((r.GetHashCode() == r1.GetHashCode() || !r1.placed) && r1.GetType() != typeof(DownStairsRoom))
                if (r.GetHashCode() == r1.GetHashCode() || !r1.placed)
                    continue;
                Rect rect = r.Intersect(r1);
                if (rect.Width() > 0 && rect.Height() > 0)
                {
                    //Debug.Log("room " + rooms.IndexOf(r) + " Intersects with Room number " + rooms.IndexOf(r1) + ", Intersect Range : " + rect.Width() + ", " + rect.Height());
                    return true;
                }
            }
            return false;
        }
        public bool CheckOverlap(Room r, out int index) // 겹치는 방 발견 시, 해당 방 번호를 같이 제공(최초 발견한 방만)
        {
            foreach (Room r1 in rooms)
            {
                if (r.GetHashCode() == r1.GetHashCode() || !r1.placed)
                    continue;
                Rect rect = r.Intersect(r1);
                if (rect.Width() > 0 && rect.Height() > 0)
                {
                    //Debug.Log("Intersects with Room number " + rooms.IndexOf(r1) + ", Intersect Range : " + rect.Width() + ", " + rect.Height());
                    index = rooms.IndexOf(r1);
                    return true;
                }
            }
            index = -1;
            return false;
        }
        public void ConnectRooms()
        {
            foreach(Room r1 in rooms)
            {
                foreach(Room r2 in rooms)
                {
                    if (r2.GetHashCode() == r1.GetHashCode() || r1 == null || r2 == null || r1.neighbours.ContainsKey(r2) || r2.neighbours.ContainsKey(r1))
                        continue;
                    if(r1.IsNeighbour(r2))
                    {
                        r1.neighbours.Add(r2, false);
                        r2.neighbours.Add(r1, false);
                    }
                }
            }
            foreach(Room r in rooms)
            {
                foreach (KeyValuePair<Room, bool> p in r.neighbours.ToList())
                    r.Connect(this, p.Key);
            }
        }

        
        public Rect LevelRect()
        {
            Rect rect = new Rect();
            foreach (Room r in rooms)
            {
                if (r.x < rect.x)
                    rect.x = r.x;
                if (r.y < rect.y)
                    rect.y = r.y;
                if (r.xMax > rect.xMax)
                    rect.xMax = r.xMax;
                if (r.yMax > rect.yMax)
                    rect.yMax = r.yMax;
            }
            rect.x -= 1;
            rect.y -= 1;
            width = rect.Width();
            height = rect.Height();
            /*width = radius;
            height = radius;
            rect.xMax = rect.x + width - 1;
            rect.yMax = rect.y + height - 1;*/
            length = width * height;
            return rect;
        }
        public void MoveRooms()
        {
            int xDir = -levelr.x;
            int yDir = -levelr.y;
            //Debug.Log("Moving : " + xDir + ", " + yDir);
            //Debug.Log(levelr.Width() + ", " + levelr.Height());
            levelr.SetPosition(0 ,0);
            foreach (Room r in rooms)
            {
                r.MovePosition(xDir, yDir);
            }
        }
    }
}
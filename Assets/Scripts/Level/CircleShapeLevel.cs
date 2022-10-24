using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SCC
{
    public class CircleShapeLevel : Level
    {
        public int angle;
        public bool clockwise = true;

        public CircleShapeLevel(int r)
        {
            this.radius = r;
        }

        public override void InitRooms()
        {
            rooms = new List<Room>();
            //rooms.Add(new UpStairsRoom());
            rooms.Add(new DownStairsRoom(this));

            PlaceRooms();
            AddBranches();
            levelr = LevelRect2();
            MoveRooms();
            map = new int[width, height];
            //Debug.Log(width + ", " + height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    map[i, j] = Terrain.VOID;
                }
            }

            foreach (Room r in rooms)
                r.Paint(this, r);

        }
        public override void PlaceRooms()
        {
            if (radius == 0)
                return;
            int startangle = 0;

            rooms[0].SetPosition((int)(radius * Mathf.Cos(Mathf.PI * startangle / 180)), (int)(radius * Mathf.Sin(Mathf.PI * startangle / 180)));
            rooms[0].placed = true;
            rooms[0].angle = startangle;
            //Debug.Log(rooms[0].Info());
            
            //Debug.Log(Mathf.Cos(Mathf.PI * startangle / 180));
            angle = startangle;       

            Room ent = rooms[0];
            int num = 0;
            Room room = new EmptyRoom(this);
            rooms.Add(room);
            int count = 0;
            while (num < rooms.Count)
            {
                Room r = rooms[num];

                if (ent.GetHashCode() == r.GetHashCode() || r.placed == true)
                {
                    num++;
                    continue;
                }//���� ��� ���� ���̰ų�, �̹� ��ġ�� ���̸� �ѱ��.

                r.SetPosition(ent.x, ent.y);
                //int count = 1;

                while (CheckOverlap(r))
                {
                    int angle = startangle + count;
                    r.SetPosition((int)(radius * Mathf.Cos(Mathf.PI * angle / 180)), (int)(radius * Mathf.Sin(Mathf.PI * angle / 180)));
                    
                    count++;
                    if (count > 360)//���࿡ �� ��ħ�̳� ���ѷ��� �̽� �߻���, count ���� �÷��� ��.
                    {
                        //Debug.Log("ERR0");
                        rooms.Remove(r);
                        r = null;
                        break;
                    }
                }
                //Debug.Log("count : " + count);
                if (r != null)
                {
                    while (!r.IsNeighbour(ent))
                    {
                        int angle = startangle + count;
                        r.SetPosition((int)(radius * Mathf.Cos(Mathf.PI * angle / 180)), (int)(radius * Mathf.Sin(Mathf.PI * angle / 180)));
                        r.angle = angle;
                        count++;
                        if (count > 360 || CheckOverlap(r)) 
                        {
                            //Debug.Log("ERR1");
                            rooms.Remove(r);
                            r = null;
                            break;
                        }
                    }
                }
                if (r != null)
                {


                    //��ġ �� ���� �� �־�� �ϴ��� �˻�
                    r.placed = true;
                    //Debug.Log("Room Added. Info : " + r.Info() + ", Number : " + rooms.Count);
                    if (/*!r.IsNeighbour(d) &&*/ r.GetType() != typeof(DownStairsRoom))
                    {
                        Room rm = new EmptyRoom(this);
                        //Debug.Log("room added.");
                        rooms.Add(rm);
                        num++;
                    }
                    ent = r;
                }
                //else
                    //Debug.Log("FInished.");
            }
        }

        public void AddBranches()
        {
            int n = 0;
            int bradius = radius;
            while (n < rooms.Count)
            {
                Room r = rooms[n];
                if (r.GetType() != typeof(EmptyRoom) || r.branch || r.hasbranch)
                {
                    n++;
                    continue;
                }
                bradius = radius;

                Room room = new EmptyRoom(this);
                rooms.Add(room);
                room.SetPosition(r.x, r.y);

                while (CheckOverlap(room))
                {
                    room.SetPosition((int)(bradius * Mathf.Cos(Mathf.PI * r.angle / 180)), (int)(bradius * Mathf.Sin(Mathf.PI * r.angle / 180)));

                    bradius++;
                    if (bradius > radius * 2)//���࿡ �� ��ħ�̳� ���ѷ��� �̽� �߻���, count ���� �÷��� ��.
                    {
                        Debug.Log("ERR2");
                        rooms.Remove(room);
                        break;
                    }
                }
                while (!room.IsNeighbour(r))
                {
                    room.SetPosition((int)(bradius * Mathf.Cos(Mathf.PI * r.angle / 180)), (int)(bradius * Mathf.Sin(Mathf.PI * r.angle / 180)));
                    
                    bradius++;
                    if (bradius > radius * 2)//���࿡ �� ��ħ�̳� ���ѷ��� �̽� �߻���, count ���� �÷��� ��.
                    {
                        //Debug.Log("ERR3");
                        rooms.Remove(room);
                        break;
                    }
                }

                room.placed = true;
                room.branch = true;
                r.hasbranch = true;
                n++;
            }
        }

        public Rect LevelRect2()
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
            //width = rect.Width();
            //height = rect.Height();
            width = (int)(radius * 2.5);
            height = (int)(radius * 2.5);
            rect.xMax = rect.x + width - 1;
            rect.yMax = rect.y + height - 1;
            length = width * height;
            return rect;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SCC
{
    public class LineShapeLevel : Level
    {
        
        public int angle;

        public LineShapeLevel(int r)
        {
            this.radius = r;
        }

        public override void InitRooms()
        {
            rooms = new List<Room>();
            rooms.Add(new UpStairsRoom(this));
            rooms.Add(new DownStairsRoom(this));

            PlaceRooms();
            AddBranches();
            levelr = LevelRect2();
            //Debug.Log(width + ", " + height);
            MoveRooms();
            map = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    map[i, j] = Terrain.VOID;
                }
            }

            foreach (Room r in rooms)
                r.Paint(this, r);
            ConnectRooms();
        }
        public override void PlaceRooms()
        {
            if (radius == 0)
                return;
            rooms[0].SetPosition(0, 0);
            rooms[0].placed = true;
            //Debug.Log(rooms[0].Info());
            //int startangle = rand.Next(0, 360);
            int startangle = 90;
            //Debug.Log(startangle);
            angle = startangle; 
            int i = 0;

            Room d = rooms[1];
            foreach (Room r in rooms)
            {
                if (r.GetType() != typeof(DownStairsRoom))
                    continue;

                int x = (int)(radius * Math.Sin(Math.PI * startangle / 180));
                int y = (int)(radius * Math.Cos(Math.PI * startangle / 180));
                //Debug.Log(x + ", " + y);
                r.SetPosition(x, y);
                r.placed = true;
                i++;
            }// 출구방을 정해진 각도와 방향으로 배치한다.

            Room ent = rooms[0];
            int num = 0;
            Room room = new EmptyRoom(this);
            rooms.Add(room);

            while (num < rooms.Count)
            {
                Room r = rooms[num];

                if (ent.GetHashCode() == r.GetHashCode() || r.placed == true)
                {
                    num++;
                    continue;
                }//이전 방과 같은 방이거나, 이미 배치된 방이면 넘긴다.

                r.SetPosition(ent.x, ent.y);
                int xOrigin = r.x;
                int yOrigin = r.y;
                int count = 1;

                while (CheckOverlap(r))
                {
                    r.SetPosition(xOrigin + (int)(count * Math.Sin(Math.PI * startangle / 180)), yOrigin + (int)(count * Math.Cos(Math.PI * startangle / 180)));
                    if (CheckOverlap(r, d))
                    {
                        rooms.Remove(r);
                        r = null;
                        int count2 = 0;
                        d.SetPosition(xOrigin , yOrigin);
                        while (CheckOverlap(d, ent))
                        { 
                            d.SetPosition(xOrigin + (int)(count2 * Math.Sin(Math.PI * startangle / 180)), yOrigin + (int)(count2 * Math.Cos(Math.PI * startangle / 180)));
                            count2++;
                            if (count2 > 200)//만약에 방 겹침이나 무한루프 이슈 발생시, count 값을 늘려볼 것.
                            {
                                //Debug.Log("ERR");
                                break;
                            }
                        }
                        count2--;
                        d.SetPosition(xOrigin + (int)(count2 * Math.Sin(Math.PI * startangle / 180)), yOrigin + (int)(count2 * Math.Cos(Math.PI * startangle / 180)));
                        //Debug.Log("Attatched. Info : " + d.Info());
                        d.placed = true;
                        //return;
                        break;
                    }
                    //Debug.Log(r.Info());
                    count++;
                    if (count > 200)//만약에 방 겹침이나 무한루프 이슈 발생시, count 값을 늘려볼 것.
                    {
                        //Debug.Log("ERR");
                        break;
                    }
                }

                if(r != null)
                {
                    while (!r.IsNeighbour(ent))
                    {
                        r.SetPosition(xOrigin + (int)(count * Math.Sin(Math.PI * startangle / 180)), yOrigin + (int)(count * Math.Cos(Math.PI * startangle / 180)));
                        count++;
                        if (count > 200)//만약에 방 겹침이나 무한루프 이슈 발생시, count 값을 늘려볼 것.
                        {
                            //Debug.Log("ERR");
                            break;
                        }
                    }
                }
                else
                {
                    while (!d.IsNeighbour(ent))
                    {
                        d.SetPosition(xOrigin + (int)(count * Math.Sin(Math.PI * startangle / 180)), yOrigin + (int)(count * Math.Cos(Math.PI * startangle / 180)));
                        count++;
                        if (count > 200)//만약에 방 겹침이나 무한루프 이슈 발생시, count 값을 늘려볼 것.
                        {
                            //Debug.Log("ERR");
                            break;
                        }
                    }
                }
   
                

                if (r != null)
                {


                    //배치 후 방을 더 넣어야 하는지 검사
                    r.placed = true;
                    //Debug.Log("Room Added. Info : " + r.Info() + ", Number : " + rooms.Count);
                    if (/*!r.IsNeighbour(d) &&*/ r.GetType() != typeof(DownStairsRoom))
                    {
                        Room rm = new EmptyRoom(this);
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
            int inverse = 1;
            double rangle = Math.PI * angle / 180;
            while (n < rooms.Count)
            {
                Room r = rooms[n];
                if (r.GetType() != typeof(EmptyRoom) || r.branch)
                {
                    n++;
                    continue;
                }

                double bangle = rangle + inverse * (Math.PI / 2);
                inverse *= -1;
                Room room = new EmptyRoom(this);
                if (rangle == Math.PI / 2 || rangle == Math.PI * 3 / 2)
                {
                    if (room.width > room.height)
                    {
                        room.Set(room.y, room.x, room.yMax, room.xMax);
                    }
                }
                else if (rangle == Math.PI || rangle == 0)
                {
                    if (room.width < room.height)
                    {
                        room.Set(room.y, room.x, room.yMax, room.xMax);
                    }
                }
                rooms.Add(room);
                room.SetPosition(r.x, r.y);
                int xOrigin = room.x;
                int yOrigin = room.y;
                int count = 1;

                while (CheckOverlap(room))
                {
                    /*f (rangle == Math.PI / 2 || rangle == Math.PI * 3 / 2)
                        room.SetPosition(xOrigin, yOrigin + (int)(count * Math.Cos(bangle)));
                    else if (rangle == Math.PI || rangle == 0)
                        room.SetPosition(xOrigin + (int)(count * Math.Sin(bangle)), yOrigin);*/
                    room.SetPosition(xOrigin + (int)(count * Math.Sin(bangle)), yOrigin + (int)(count * Math.Cos(bangle)));
                    //Debug.Log(room.Info());
                    count++;
                    if (count > 200)//만약에 방 겹침이나 무한루프 이슈 발생시, count 값을 늘려볼 것.
                    {
                        //Debug.Log("ERR");
                        rooms.Remove(room);
                        break;
                    }
                }

                while (!room.IsNeighbour(r))
                {
                    room.SetPosition(xOrigin + (int)(count * Math.Sin(bangle)), yOrigin + (int)(count * Math.Cos(bangle)));
                    count++;
                    if (count > 200)//만약에 방 겹침이나 무한루프 이슈 발생시, count 값을 늘려볼 것.
                    {
                        //Debug.Log("ERR");
                        break;
                    }
                }

                room.placed = true;
                room.branch = true;
                count = 0;
                int max = 0;
                
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
            width = (int)(radius * 1.25);
            height = (int)(radius * 1.25);
            rect.xMax = rect.x + width - 1;
            rect.yMax = rect.y + height - 1;
            length = width * height;
            return rect;
        }
    }
}
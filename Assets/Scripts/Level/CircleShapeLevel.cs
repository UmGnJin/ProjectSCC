using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace SCC
{
    public class CircleShapeLevel : Level
    {
        public int angle;
        public bool clockwise = true;
        public int mradius;

        public CircleShapeLevel(int r)
        {
            this.radius = r;
            this.mradius = (int)(r * 0.3);
        }

        public override void InitRooms()
        {
            rooms = new List<Room>();
            //rooms.Add(new UpStairsRoom());
            rooms.Add(new DownStairsRoom(this));

            PlaceRooms();
            PlaceRooms2();
            //AddBranches();
            //AddAnotherBranches();
            AddReverseBranches();
            AddAnotherReverseBranches();
            levelr = LevelRect2();
            MoveRooms();
            map = new int[width, height];
            Debug.Log(width + ", " + height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    map[i, j] = Terrain.VOID;
                }
            }

            foreach (Room r in rooms)
            {
                if (r.x < 0 || r.y < 0 || r.x > width || r.y > height)
                {
                    rooms.Remove(r);
                    continue;
                }
                r.Paint(this, r);
                
            }
            foreach (Room r in rooms)
                foreach (KeyValuePair<Room, bool> p in r.neighbours.ToList())
                    r.Connect(this, p.Key);
            //Debug.Log(rooms.Count);
            //ConnectRooms();
        }
        public override void PlaceRooms()
        {
            if (radius == 0)
                return;
            int startangle = rand.Next(0, 360);

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


                if (ent.GetHashCode() == r.GetHashCode() || r.placed == true || r == null)
                {
                    num++;
                    continue;
                }//이전 방과 같은 방이거나, 이미 배치된 방이면 넘긴다.

                r.SetPosition(ent.x, ent.y);
                //int count = 1;

                while (CheckOverlap(r))
                {
                    int angle = startangle + count;
                    r.SetPosition((int)(radius * Mathf.Cos(Mathf.PI * angle / 180)), (int)(radius * Mathf.Sin(Mathf.PI * angle / 180)));
                    r.angle = angle;
                    count++;
                    if (count > 360)
                    {
                        Debug.Log("ERR0 : Overlap test failed.");
                        rooms.Remove(r);
                        r = null;
                        break;
                    }
                }
                //Debug.Log("count : " + count);
                if (r != null)
                {
                    int angle = startangle;
                    while (!r.IsNeighbour(ent))
                    {
                        angle = startangle + count;
                        r.SetPosition((int)(radius * Mathf.Cos(Mathf.PI * angle / 180)), (int)(radius * Mathf.Sin(Mathf.PI * angle / 180)));
                        r.angle = angle;
                        count++;
                        if (count > 3600 || CheckOverlap(r)) 
                        {
                            Debug.Log("ERR1 : Neighbour test failed." + count);
                            rooms.Remove(r);
                            r = null;
                            break;
                        }
                    }
                }
                if (r != null)
                {

                    
                    //배치 후 방을 더 넣어야 하는지 검사
                    r.placed = true;
                    r.neighbours.Add(ent, false);
                    ent.neighbours.Add(r, false);
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
                /*else if (r == null)
                {
                    ent.neighbours.Add(rooms[0], false);
                    rooms[0].neighbours.Add(ent, false);
                }*/
                //else
                    //Debug.Log("FInished.");
            }
        }

        public void PlaceRooms2()
        {
            if (radius == 0)
                return;
            int startangle = rand.Next(0, 360);

            rooms[0].SetPosition((int)(mradius * Mathf.Cos(Mathf.PI * startangle / 180)), (int)(mradius * Mathf.Sin(Mathf.PI * startangle / 180)));
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


                if (ent.GetHashCode() == r.GetHashCode() || r.placed == true || r == null)
                {
                    num++;
                    continue;
                }//이전 방과 같은 방이거나, 이미 배치된 방이면 넘긴다.

                r.SetPosition(ent.x, ent.y);
                //int count = 1;

                while (CheckOverlap(r))
                {
                    int angle = startangle + count;
                    r.SetPosition((int)(mradius * Mathf.Cos(Mathf.PI * angle / 180)), (int)(mradius * Mathf.Sin(Mathf.PI * angle / 180)));
                    r.angle = angle;
                    count++;
                    if (count > 360)
                    {
                        Debug.Log("ERR0 : Overlap test failed.");
                        rooms.Remove(r);
                        r = null;
                        break;
                    }
                }
                //Debug.Log("count : " + count);
                if (r != null)
                {
                    int angle = startangle;
                    while (!r.IsNeighbour(ent))
                    {
                        angle = startangle + count;
                        r.SetPosition((int)(mradius * Mathf.Cos(Mathf.PI * angle / 180)), (int)(mradius * Mathf.Sin(Mathf.PI * angle / 180)));
                        r.angle = angle;
                        count++;
                        if (count > 3600 || CheckOverlap(r))
                        {
                            Debug.Log("ERR1 : Neighbour test failed." + count);
                            rooms.Remove(r);
                            r = null;
                            break;
                        }
                    }
                }
                if (r != null)
                {


                    //배치 후 방을 더 넣어야 하는지 검사
                    r.placed = true;
                    r.hasbranch = true;
                    r.neighbours.Add(ent, false);
                    ent.neighbours.Add(r, false);
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
                /*
                else if (r == null)
                {
                    ent.neighbours.Add(rooms[0], false);
                    rooms[0].neighbours.Add(ent, false);
                }
                */
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
                if (r.GetType() != typeof(EmptyRoom) || r.branch || r.hasbranch || !r.placed || r == null)
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
                    room.newRadius = bradius;
                    room.angle = r.angle;
                    if (bradius > radius * 2)
                    {
                        Debug.Log("ERR2");
                        rooms.Remove(room);
                        room = null;
                        break;
                    }
                }
                if (room != null)
                {
                    while (!room.IsNeighbour(r) || CheckOverlap(room))
                    {
                        room.SetPosition((int)(bradius * Mathf.Cos(Mathf.PI * r.angle / 180)), (int)(bradius * Mathf.Sin(Mathf.PI * r.angle / 180)));

                        bradius++;
                        room.newRadius = bradius;
                        room.angle = r.angle;
                        if (bradius > radius * 2)
                        {
                            //Debug.Log("ERR3");
                            rooms.Remove(room);
                            room = null;
                            break;
                        }
                    }
                }

                if (room != null)
                {
                    room.placed = true;
                    room.branch = true;
                    r.hasbranch = true;

                    room.neighbours.Add(r, false);
                    r.neighbours.Add(room, false);
                }
                n++;
            }
        }

        public void AddAnotherBranches()
        {
            int n = 0;
            int bradius = radius;
            while (n < rooms.Count)
            {
                Room r = rooms[n];
                if (r.GetType() != typeof(EmptyRoom) || !r.branch || r.hasbranch || r.newRadius == 0 || r == null)
                {
                    n++;
                    continue;
                }
                bradius = r.newRadius;

                Room room = new EmptyRoom(this);
                rooms.Add(room);
                room.SetPosition(r.x, r.y);

                while (CheckOverlap(room))
                {
                    room.SetPosition((int)(bradius * Mathf.Cos(Mathf.PI * r.angle / 180)), (int)(bradius * Mathf.Sin(Mathf.PI * r.angle / 180)));

                    bradius++;
                    room.newRadius = bradius;
                    if (bradius > radius * 2)//만약에 방 겹침이나 무한루프 이슈 발생시, count 값을 늘려볼 것.
                    {
                        Debug.Log("ERR2");
                        rooms.Remove(room);
                        room = null;
                        break;
                    }
                }
                if (room != null)
                {
                    while (!room.IsNeighbour(r) || CheckOverlap(room))
                    {
                        room.SetPosition((int)(bradius * Mathf.Cos(Mathf.PI * r.angle / 180)), (int)(bradius * Mathf.Sin(Mathf.PI * r.angle / 180)));

                        bradius++;
                        room.newRadius = bradius;
                        if (bradius > radius * 2)//만약에 방 겹침이나 무한루프 이슈 발생시, count 값을 늘려볼 것.
                        {
                            Debug.Log("ERR3");
                            rooms.Remove(room);
                            room = null;
                            break;
                        }
                    }
                }

                if (room != null)
                {
                    room.placed = true;
                    room.branch = true;
                    r.hasbranch = true;
                    room.hasbranch = true;

                    room.neighbours.Add(r, false);
                    r.neighbours.Add(room, false);
                }
                
                n++;
            }
        }

        public void AddReverseBranches()
        {
            int n = 0;
            int bradius = radius;
            while (n < rooms.Count)
            {
                Room r = rooms[n];
                if (r.GetType() != typeof(EmptyRoom) || r.branch || r.rbranch || r.hasrbranch || r == null)
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

                    bradius--;
                    room.newRadius = bradius;
                    room.angle = r.angle;
                    if (bradius < radius / 2)//만약에 방 겹침이나 무한루프 이슈 발생시, count 값을 늘려볼 것.
                    {
                        //Debug.Log("ERR2");
                        rooms.Remove(room);
                        room = null;
                        break;
                    }
                }
                if (room != null)
                {
                    while (!room.IsNeighbour(r) || CheckOverlap(room))
                    {
                        room.SetPosition((int)(bradius * Mathf.Cos(Mathf.PI * r.angle / 180)), (int)(bradius * Mathf.Sin(Mathf.PI * r.angle / 180)));

                        bradius--;
                        room.newRadius = bradius;
                        room.angle = r.angle;
                        if (bradius < radius / 2)//만약에 방 겹침이나 무한루프 이슈 발생시, count 값을 늘려볼 것.
                        {
                            //Debug.Log("ERR3");
                            rooms.Remove(room);
                            room = null;
                            break;
                        }
                    }
                }

                if (room != null)
                {
                    room.placed = true;
                    room.rbranch = true;
                    r.hasrbranch = true;
                    room.hasbranch = true;

                    room.neighbours.Add(r, false);
                    r.neighbours.Add(room, false);
                }

                n++;
            }
        }

        public void AddAnotherReverseBranches()
        {
            int n = 0;
            int bradius = radius;
            while (n < rooms.Count)
            {
                Room r = rooms[n];
                if (r.GetType() != typeof(EmptyRoom) || !r.rbranch || r.hasrbranch || r == null)
                {
                    n++;
                    continue;
                }
                bradius = r.newRadius;

                Room room = new EmptyRoom(this);
                rooms.Add(room);
                room.SetPosition(r.x, r.y);

                while (CheckOverlap(room))
                {
                    room.SetPosition((int)(bradius * Mathf.Cos(Mathf.PI * r.angle / 180)), (int)(bradius * Mathf.Sin(Mathf.PI * r.angle / 180)));

                    bradius--;
                    room.newRadius = bradius;
                    if (bradius < radius / 8)//만약에 방 겹침이나 무한루프 이슈 발생시, count 값을 늘려볼 것.
                    {
                        //Debug.Log("ERR2");
                        rooms.Remove(room);
                        room = null;
                        break;
                    }
                }
                if (room != null)
                {
                    while (!room.IsNeighbour(r) || CheckOverlap(room))
                    {
                        room.SetPosition((int)(bradius * Mathf.Cos(Mathf.PI * r.angle / 180)), (int)(bradius * Mathf.Sin(Mathf.PI * r.angle / 180)));

                        bradius--;
                        room.newRadius = bradius;
                        if (bradius < radius / 8)//만약에 방 겹침이나 무한루프 이슈 발생시, count 값을 늘려볼 것.
                        {
                            //Debug.Log("ERR3");
                            rooms.Remove(room);
                            room = null;
                            break;
                        }
                    }
                }

                if (room != null)
                {
                    room.placed = true;
                    room.rbranch = true;
                    r.hasrbranch = true;
                    room.hasbranch = true;

                    room.neighbours.Add(r, false);
                    r.neighbours.Add(room, false);
                }

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
            width = (int)(radius * 2.2);
            height = (int)(radius * 2.2);
            rect.xMax = rect.x + width - 1;
            rect.yMax = rect.y + height - 1;
            length = width * height;
            return rect;
        }
    }
}
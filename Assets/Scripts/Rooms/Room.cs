using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using System; 

namespace SCC
{
    public abstract class Room : Rect
    {
        protected static Random rand = new Random();//하위 클래스들이 방 크기 설정에 사용할 랜덤 클래스.
        public const int MINROOMSIZE = 4;
        public const int MAXROOMSIZE = 9;
        public int width, height;

        public int newRadius = 0;

        public bool placed = false;
        public bool branch = false;
        public bool rbranch = false;
        public bool hasbranch = false;
        public bool hasrbranch = false;
    


        public int angle;

        public Dictionary<Room, bool> neighbours =new Dictionary<Room, bool>();

        public bool IsNeighbour(Room r)//이 방이 이웃이 될 수 있는지를 검사. 
        {
            if (this.branch && r.branch)
                return false;
            double dist = Vector2.Distance(this.Center(), r.Center());
            double zdist = this.Radius() + r.Radius();
            //Debug.Log(dist - zdist);
            if (dist - zdist > 3 && dist - zdist <= dist * 0.5f)
            {
                //Debug.Log(dist - zdist);
                return true;
            }
            //Debug.Log(dist - zdist);
            return false;
        }
        public void DefaultSet()
        {
            x = 0;
            y = 0;
            xMax = x + width;
            yMax = y + height;
        }
        public double Radius()
        {
            return Math.Sqrt(Math.Pow(width / 2, 2) + Math.Pow(height / 2, 2));
        }
        public String Info()
        {
            return "Position : (" + x + ", " + y + "), " + "width : " + Width() + ", height : " + Height();
        }
        public bool Connect(Level l, Room r)
        {
            if (!neighbours.ContainsKey(r) || !r.neighbours.ContainsKey(this) || r == null || !r.placed || !this.placed)
            {
                //Debug.Log("Wrong Connection Error.");
                return false;
            }
            else if(neighbours[r] == true || r.neighbours[this] == true)
            {
                //Debug.Log("Already Connected.");
                return false;
            }

            Vector2 Point1 = new Vector2(rand.Next(x + 1, xMax), rand.Next(y + 1, yMax));
            Vector2 Point2 = new Vector2(rand.Next(r.x + 1, r.xMax), rand.Next(r.y + 1, r.yMax));
            if (Point1.x < 0 || Point1.y < 0 || Point2.x < 0 || Point2.y < 0)
                return false;
            //Debug.Log(Point1);
            //Debug.Log(Point2);
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

                for(int i = x1; i <= x2; i++)
                {
                    for(int j = y1; j <= y2; j++)
                    {
                        if ((i == x1 || j == (int)Point2.y) && i > 0 && j > 0 && i < l.width && j < l.height)
                        {
                            l.map[i, j] = Terrain.GROUND;
                            /*
                            try
                            {
                                l.map[i, j] = Terrain.GROUND;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message + ", Current Position : " + i + ", " + j);
                            }
                            */
                        }
                    }
                }
            }
            else if(Point1.x > Point2.x)
            {
                x1 = (int)Point2.x;
                x2 = (int)Point1.x;

                for (int i = x1; i <= x2; i++)
                {
                    for (int j = y1; j <= y2; j++)
                    {
                        if ((i == x1 || j == (int)Point1.y) && i > 0 && j > 0 && i < l.width && j < l.height)
                        {
                            l.map[i, j] = Terrain.GROUND;
                            /*
                            try
                            {
                                l.map[i, j] = Terrain.GROUND;
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.Message + ", Current Position : " + i + ", " + j);
                            }
                            */
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
                        if (i == x1 && i > 0 && j > 0 && i < l.width && j < l.height)
                        {
                            l.map[i, j] = Terrain.GROUND;
                            /*
                            try
                            {
                                l.map[i, j] = Terrain.GROUND;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message + ", Current Position : " + i + ", " + j);
                            }
                            */
                        }
                    }
                }
            }

            neighbours[r] = true;
            r.neighbours[this] = true;
            return true;
        }


        public abstract void Paint(Level l, Room r);// 오버라이딩용
    }
}
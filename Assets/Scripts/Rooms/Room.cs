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

        public bool placed = false;
        public bool branch = false;
        public bool hasbranch = false;


        public int angle;

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
        public abstract void Paint(Level l, Room r);// 오버라이딩용
    }
}
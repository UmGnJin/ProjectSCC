using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace SCC
{
    public class EmptyRoom : Room
    {
        public EmptyRoom(Level l)
        {
            width = rand.Next((l.radius / 10), (l.radius / 5));
            height = rand.Next((l.radius / 10), (l.radius / 5));
            width = rand.Next((width / 2), width);
            height = rand.Next((height / 2), height);
            


            if (width < 4)
                width = 4;
            if (height < 4)
                height = 4;
            //setting room size.
            DefaultSet();
        }
        public EmptyRoom(int w, int h)
        {
            width = w;
            height = h;
            DefaultSet();
        }
        
        public override void Paint(Level l, Room r)
        {
            for (int i = r.x; i < r.x + r.width; i++)
            {
                for (int j = r.y; j < r.y + r.height; j++)
                {
                    int tile;
                    tile = Terrain.GROUND;
                    if (i == r.x + r.width - 1 || j == r.y + r.height - 1 || i == r.x || j == r.y)
                        tile = Terrain.WALL;
                    l.map[i, j] = tile;
                }
            }
            //Debug.Log("done.");
        }
    }
}
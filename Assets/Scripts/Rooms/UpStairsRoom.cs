using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCC
{
    public class UpStairsRoom : Room
    {
        public UpStairsRoom(Level l)
        {
            width = rand.Next((l.radius / 20), (l.radius / 10));
            height = rand.Next((l.radius / 20), (l.radius / 10));
            if (width < 4)
                width = 4;
            if (height < 4)
                height = 4;
            DefaultSet();
        }
        public UpStairsRoom(int w, int h)
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
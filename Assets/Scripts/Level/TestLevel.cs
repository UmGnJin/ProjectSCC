using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SCC
{
    public class TestLevel : Level
    {
        public override void InitRooms()
        {
            rooms = new List<Room>();
            rooms.Add(new EmptyRoom(this));
            rooms.Add(new EmptyRoom(this));
            
            PlaceRooms();
            levelr = LevelRect();
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
            
        }

        public override void PlaceRooms()
        {
            rooms[0].SetPosition(0, 0);
            rooms[0].placed = true;
            int radius = 5;

            rooms[1].SetPosition(0, 10 * radius);
            rooms[1].placed = true;
            
        }
    }
}
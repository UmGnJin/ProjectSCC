using UnityEngine;
using System.Threading;

namespace SCC
{
    public class PerlinNoise
    {
        public int width = 0;
        public int height = 0;

        public float scale = 1f;

        public float offsetX;
        public float offsetY;

        public int[,] map;
        public int[,] area;
        public int areaNum = 1;

        public static int count = 0;

        public PerlinNoise(int size)
        {
            this.width = size;
            this.height = size;
            offsetX = Random.Range(0f, 99999f);
            offsetY = Random.Range(0f, 99999f);
            map = new int[width, height];
            area = new int[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map[x, y] = CalculateTile(x, y);
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (map[i, j] != Terrain.GROUND)
                        area[i, j] = -1;
                    else
                        area[i, j] = 0;
                }
            }
        }
        public void ThreadFindRooms(int x, int y)
        {
            int num = 2147483647;
            Thread thread = new Thread(() =>
            {
                FindRooms4(x, y);
            }, num);
            thread.Start();
        }
        public void FindRooms4(int x, int y)//Define connected area. Use it to find isolated mini-room.
        {

            switch (map[x, y])
            {
                case Terrain.WALL:
                    if (NeighborValue(x, y) == 8)
                        area[x, y] = -1;
                    else
                        area[x, y] = -2;
                    break;

                case Terrain.GROUND:
                    if (area[x, y] <= 0)
                    {
                        area[x, y] = areaNum;

                        if (x > 0)
                            FindRooms4(x - 1, y);
                        if (y > 0)
                            FindRooms4(x, y - 1);
                        if (x < width - 1)
                            FindRooms4(x + 1, y);
                        if (y < height - 1)
                            FindRooms4(x, y + 1);
                    }
                    break;

                default:
                    break;
            }

        }

        public int NeighborValue(int x, int y)//Check 8Dir neighbors' tile. return the number of walls.
        {
            int value = 0;
            int neighbor = 0;
            if (x > 0)
            {
                value += map[x - 1, y];//west
                neighbor++;
                if (y > 0)
                {
                    value += map[x - 1, y - 1];//northwest
                    neighbor++;
                }
            }
            if (y > 0)
            {
                value += map[x, y - 1];//north
                neighbor++;
                if (x < width - 1)
                {
                    value += map[x + 1, y - 1];//northeast
                    neighbor++;
                }
            }
            if (x < width - 1)
            {
                value += map[x + 1, y];//east
                neighbor++;
                if (y < height - 1)
                {
                    value += map[x + 1, y + 1];//southeast
                    neighbor++;
                }
            }
            if (y < height - 1)
            {
                value += map[x, y + 1];//south
                neighbor++;
                if (x > 0)
                {
                    value += map[x - 1, y + 1];//southwest
                    neighbor++;
                }
            }
            if (neighbor < 8)
                value += Terrain.WALL * (8 - neighbor);
            return value;
        }














        Texture2D GenerateTexture()
        {
            Texture2D texture = new Texture2D(width, height);



            texture.Apply();

            return texture;
        }

        Color CalculateColor(int x, int y)
        {
            float xCoord = (float)x / width * scale + offsetX;
            float yCoord = (float)y / height * scale + offsetY;

            float sample = Mathf.PerlinNoise(xCoord, yCoord);

            Color c = sample >= 0.5f ? Color.white : Color.black;
            //return new Color(sample, sample, sample);
            return c;
        }

        int CalculateTile(int x, int y)
        {
            float xCoord = (float)x / width * scale + offsetX;
            float yCoord = (float)y / height * scale + offsetY;

            float sample = Mathf.PerlinNoise(xCoord, yCoord);

            int c = sample >= 0.5f ? 0 : 1;
            //return new Color(sample, sample, sample);
            return c;
        }
    }
}
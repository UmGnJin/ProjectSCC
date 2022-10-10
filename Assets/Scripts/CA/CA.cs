using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCC;
using Random = System.Random;
using Terrain = SCC.Terrain;

public class CA
{
    public int[,] map;
    public int[,] area;
    public int areaNum = 1;

    public int w, h;
    public int n, t;// n is iteration number, t is tile replacing number.
    public double f;
    public static Random rand = new Random();

    public bool smooth = false;

    public CA(int width, int height, int n, int t, double f)//f is initial floor ratio.
    {
        map = new int[width, height];
        area = new int[width, height];
        w = width;
        h = height;
        this.f = f;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                map[i, j] = rand.NextDouble() <= f ? Terrain.GROUND : Terrain.WALL;
                if (map[i, j] != Terrain.GROUND)
                    area[i, j] = -1;
                else
                    area[i, j] = 0;
            }
        }
        this.n = n;
        this.t = t;
    }

    

    public void ShuffleInitialMap()
    {
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                map[i, j] = rand.NextDouble() <= f ? 0 : 1;
                //if (map[i, j] != Terrain.FLOOR)
                //  area[i, j] = -1;
                //else
                //  area[i, j] = 0;
            }
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
            if (x < w - 1)
            {
                value += map[x + 1, y - 1];//northeast
                neighbor++;
            }
        }
        if (x < w - 1)
        {
            value += map[x + 1, y];//east
            neighbor++;
            if (y < h - 1)
            {
                value += map[x + 1, y + 1];//southeast
                neighbor++;
            }
        }
        if (y < h - 1)
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
                    if (x < w - 1)
                        FindRooms4(x + 1, y);
                    if (y < h - 1)
                        FindRooms4(x, y + 1);
                }
                break;

            default:
                break;
        }

    }

    public void FindRooms8(int x, int y)//Define connected area. Use it to find isolated mini-room.
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
                    {
                        FindRooms8(x - 1, y);
                        if (y > 0)
                            FindRooms8(x - 1, y - 1);
                    }
                    if (y > 0)
                    {
                        FindRooms8(x, y - 1);
                        if (x < w - 1)
                            FindRooms8(x + 1, y - 1);
                    }
                    if (x < w - 1)
                    {
                        FindRooms8(x + 1, y);
                        if (y < h - 1)
                            FindRooms8(x + 1, y + 1);
                    }
                    if (y < h - 1)
                    {
                        FindRooms8(x, y + 1);
                        if (x > 0)
                            FindRooms8(x - 1, y + 1);
                    }
                }
                break;

            default:
                break;
        }

    }

    public void ClearAreaInfo()
    {
        areaNum = 1;
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                if (map[j, i] == Terrain.GROUND)
                    area[j, i] = 0;
                else
                    area[j, i] = -1;
            }
        }
    }
    public void Smoothing()// Smoothing map with CA. can be iterated n times.
    {
        int[,] tempmap = new int[w, h];
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                //Console.WriteLine(NeighborValue(j, i));
                if (NeighborValue(j, i) >= t)
                    tempmap[j, i] = Terrain.WALL;
                else
                    tempmap[j, i] = Terrain.GROUND;
            }
        }
        
        map = tempmap;
    }

    public void Smoothing2()// Smoothing map with CA. can be iterated n times.
    {
        for (int num = 0; num < n; num++)
        {
            int[,] tempmap = new int[w, h];
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (NeighborValue(j, i) >= t)
                        tempmap[j, i] = Terrain.WALL;
                    else
                        tempmap[j, i] = Terrain.GROUND;
                }
            }
            map = tempmap;
        }
    }
}

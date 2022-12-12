using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using Random = System.Random;
using Debug = UnityEngine.Debug;
using SCC;
using Terrain = SCC.Terrain;

public class BSPManager : MonoBehaviour
{
    public static Random random = new Random();

    public GameObject[] Tiles;

    public BSP bsp;

    public static BSPManager lm;

    public void RunTimeCheck()
    {
        long[] runtimes = new long[100];
        long avg;
        double[] deviation = new double[100];
        double sd;

        int[] rooms = new int[100];
        double[] deviation2 = new double[100];
        double sd2;
        double avg2;

        for (int i = 0; i < 100; i++)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            BSP bsp = new BSP(1000, 1000, 0.4f, 0.6f, 99, 200);
            bsp.DivideLevel(bsp.tree.root);

            watch.Stop();
            Debug.Log("Runtime : " + watch.ElapsedMilliseconds + "ms.");
            runtimes[i] = watch.ElapsedMilliseconds;
            rooms[i] = bsp.rooms.Count;
        }

        long sum = 0;
        long asum = 0;
        foreach (long l in runtimes)
            sum += l;
        avg = sum / runtimes.Length;
        foreach (int i in rooms)
            asum += i;
        avg2 = asum / rooms.Length;

        double sum2 = 0;
        for (int i = 0; i < runtimes.Length; i++)
        {
            deviation[i] = Math.Pow((runtimes[i] - avg), 2);
            sum2 += deviation[i];
        }
        sd = Math.Sqrt(sum2 / deviation.Length);
        Debug.Log("Average Runtime : " + avg + "ms.");
        Debug.Log("Standard Deviation : " + sd);

        double asum2 = 0;
        for(int i = 0; i < rooms.Length;i++)
        {
            deviation2[i] = Math.Pow((rooms[i] - avg2), 2);
            asum2 += deviation2[i];
        }
        sd2 = Math.Sqrt(asum2 / deviation2.Length);
        Debug.Log("Average Rooms : " + avg2);
        Debug.Log("Standard Deviation : " + sd2);
    }

    public void RandomCheck()
    {
        BSP bsp = new BSP(1000, 1000, 0.4f, 0.6f, 99, 200);
        bsp.DivideLevel(bsp.tree.root);

        int[] rnd = new int[100];
        int[,] map = bsp.map;

        for(int n = 0; n < 100; n++)
        {
            BSP bsp2 = new BSP(1000, 1000, 0.4f, 0.6f, 99, 200);
            bsp2.DivideLevel(bsp2.tree.root);
            int num = 0;

            for (int x = 0; x < bsp.width; x++)
            {
                for (int y = 0; y < bsp.height; y++)
                {
                    if (map[x, y] == bsp2.map[x, y])
                        num++;
                }
            }
            double val = (double)num / (double)bsp2.map.Length;
            Debug.Log("Randomness : " + val);
            rnd[n] = (int)(val * 100);
        }
        double sum = 0;
        foreach (int d in rnd)
            sum += d;
        Debug.Log("Average Randomness : " + (sum / rnd.Length));
    }

    private void Awake()
    {
        if (lm == null)
        {
            lm = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (lm != this)
                Destroy(this.gameObject);
        }// 본 관리자 오브젝트의 싱글턴화.

        //RunTimeCheck();
        RandomCheck();

        //Tiles = Resources.LoadAll<GameObject>("prefabs/Tiles");
        //BSP bsp = new BSP(100, 100, 0.3f, 0.7f, 10, 20);
        //bsp.DivideLevel(bsp.tree.root);

        //PrintLevel();
        //bsp.PrintLevel();
    }

    public void PrintLevel()
    {
        for (int i = 0; i < bsp.width; i++)
        {
            for (int j = 0; j < bsp.height; j++)
            {
                GameObject tileObject;
                int tile = bsp.map[i, j];
                switch (tile)
                {
                    case Terrain.VOID:
                        tileObject = Tiles[Array.FindIndex(Tiles, t => t.name == "void")];
                        break;
                    case Terrain.GROUND:
                        tileObject = Tiles[Array.FindIndex(Tiles, t => t.name == "FloorTile2")];
                        break;
                    case Terrain.WALL:
                        tileObject = Tiles[Array.FindIndex(Tiles, t => t.name == "WallTile2")];
                        break;
                    default:
                        tileObject = Tiles[Array.FindIndex(Tiles, t => t.name == "void")];
                        break;
                }
                GameObject newTile = Instantiate(tileObject, new Vector2(i, -j), Quaternion.identity) as GameObject;
                newTile.transform.SetParent(this.transform, false);
            }
        }//현재 레벨의 맵 화면상에 출력
    }
}

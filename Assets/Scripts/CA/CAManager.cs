using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using Random = System.Random;
using SCC;
using Terrain = SCC.Terrain;
using Debug = UnityEngine.Debug;

public class CAManager : MonoBehaviour
{
    public static Random random = new Random();

    public GameObject[] Tiles;

    public CA ca;

    public static CAManager lm;

    public bool start = false;

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

        RunTimeCheck();

        //Tiles = Resources.LoadAll<GameObject>("prefabs/Tiles");
        //ca = new CA(100, 100, 4, 5, 0.4);
        //ca.Smoothing();
        //ca.Smoothing2();

        //PrintLevel2(ca);
    }

    private void Update()
    {
        if (!start)
        {
            //RandomCheck();
            //RunTimeCheck();
            //start = true;
        }
    }

    public void PrintLevel()
    {
        for (int i = 0; i < ca.w; i++)
        {
            for (int j = 0; j < ca.h; j++)
            {
                GameObject tileObject;
                int tile = ca.map[i, j];
                switch (tile)
                {
                    case Terrain.VOID:
                        continue;
                    case Terrain.GROUND:
                        tileObject = Tiles[Array.FindIndex(Tiles, t => t.name == "FloorTile2")];
                        break;
                    case Terrain.WALL:
                        tileObject = Tiles[Array.FindIndex(Tiles, t => t.name == "WallTile2")];
                        break;
                    default:
                        continue;
                }
                GameObject newTile = Instantiate(tileObject, new Vector2(i - 1, -j + 1), Quaternion.identity) as GameObject;
                newTile.transform.SetParent(this.transform, false);
            }
        }//현재 레벨의 맵 화면상에 출력
    }

    public void PrintLevel2(CA ca)
    {
        Renderer renderer = GetComponent<Renderer>();
        Texture2D texture = new Texture2D(ca.w, ca.h);

        for (int x = 0; x < ca.w; x++)
        {
            for (int y = 0; y < ca.h; y++)
            {
                Color color = ca.map[x, y] <= 0.5f ? Color.white : Color.black;

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        renderer.material.mainTexture = texture;
    }

    public void RunTimeCheck()
    {
        long[] runtimes = new long[100];
        long avg;
        double[] deviation = new double[100];
        double sd;

        for (int n = 0; n < 100; n++)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            for (int i = 0; i < 1; i++)
            {
                CA ca = new CA(4000, 4000, 4, 5, 0.5);
                ca.Smoothing2();
            }

            watch.Stop();
            Debug.Log("Runtime : " + watch.ElapsedMilliseconds + "ms.");
            runtimes[n] = watch.ElapsedMilliseconds;
        }
        long sum = 0;
        foreach (long l in runtimes)
        {
            sum += l;
        }
        avg = sum / runtimes.Length;

        double sum2 = 0;
        for (int i = 0; i < runtimes.Length; i++)
        {
            deviation[i] = Math.Pow((runtimes[i] - avg), 2);
            sum2 += deviation[i];
        }
        sd = Math.Sqrt(sum2 / deviation.Length);

        Debug.Log("Average Runtime : " + avg + "ms.");
        Debug.Log("Standard Deviation : " + sd + ".");
    }

    public void RandomCheck()
    {
        CA ca = new CA(1000, 1000, 4, 5, 0.5);
        ca.Smoothing2();

        int[] rnd = new int[100];
        int[,] map = ca.map;

        for (int n = 0; n < 100; n++)
        {
            CA ca2 = new CA(1000, 1000, 4, 5, 0.5);
            ca2.Smoothing2();
            int num = 0;

            for (int y = 0; y < ca.h; y++)
            {
                for (int x = 0; x < ca.w; x++)
                {
                    if (map[x, y] == ca2.map[x, y])
                        num++;
                }
            }
            double val = (double)num / (double)ca2.map.Length;
            Debug.Log("Randomness : " + val);
            rnd[n] = (int)(val * 100);
        }
        double sum = 0;
        foreach (int d in rnd)
        {
            sum += d;
        }
        Debug.Log("Average Randomness : " + (sum / rnd.Length));
    }
}

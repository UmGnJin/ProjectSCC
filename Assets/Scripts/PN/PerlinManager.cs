using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Threading;
using Random = System.Random;
using SCC;
using Terrain = SCC.Terrain;
using Debug = UnityEngine.Debug;

namespace SCC
{
    public class PerlinManager : MonoBehaviour
    {
        public static Random random = new Random();

        public GameObject[] Tiles;

        public CA pn;

        public static PerlinManager pm;

        Thread thread;

        private void Awake()
        {
            if (pm == null)
            {
                pm = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                if (pm != this)
                    Destroy(this.gameObject);
            }// 본 관리자 오브젝트의 싱글턴화.

            //RunTimeCheck();
            //RoomCheck();
            RandomCheck();
        }

        public static void RoomCheck()
        {
            int[] rooms = new int[100];

            for (int n = 0; n < 100; n++)
            {
                PerlinNoise pn = new PerlinNoise(1000);

                for (int i = 0; i < pn.height; i++)
                {
                    for (int j = 0; j < pn.width; j++)
                    {
                        if (pn.map[j, i] == Terrain.GROUND && pn.area[j, i] <= 0)
                        {
                            pn.ThreadFindRooms(j, i);
                            pn.areaNum++;
                        }
                    }
                }

                rooms[n] = pn.areaNum;
                Debug.Log("Independent Rooms : " + pn.areaNum);
            }

            int sum = 0;
            foreach (int i in rooms)
                sum += i;
            float avg = sum / 100;
            Debug.Log("Average Independent Rooms : " + avg);

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

                PerlinNoise pn = new PerlinNoise(4000);

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
            PerlinNoise pn = new PerlinNoise(4000);

            int[] rnd = new int[100];
            int[,] map = pn.map;

            for (int n = 0; n < 100; n++)
            {
                PerlinNoise pn2 = new PerlinNoise(4000);
                int num = 0;

                for (int y = 0; y < pn.height; y++)
                {
                    for (int x = 0; x < pn.width; x++)
                    {
                        if (map[x, y] == pn2.map[x, y])
                            num++;
                    }
                }
                double val = (double)num / (double)pn2.map.Length;
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
}
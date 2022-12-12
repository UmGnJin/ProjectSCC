using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Random = System.Random;
using SCC;

namespace SCC
{
    public class LevelManager : MonoBehaviour
    {
        public static Random random = new Random();

        public GameObject[] Tiles;

        public Level level;

        public static LevelManager lm;

        private void Awake()// radius for levels are not exact level size.
                            // LineShapeLevel -> 1.25 * radius = level size
                            // CircleShapeLevel -> 2.5 * radius = level size
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

            //RoomCheck();
            //RunTimeCheck();
            //RandomCheck();

            
            Tiles = Resources.LoadAll<GameObject>("prefabs/Tiles");
            level = new CircleShapeLevel(100);

            level.Create();
            PrintLevel();
            
        }

        public void RunTimeCheck()
        {
            long[] runtimes = new long[100];
            long avg;
            double[] deviation = new double[100];
            double sd;

            for (int i = 0; i < 100; i++)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                
                for (int j = 0; j < 1; j++)
                {
                    CircleShapeLevel level = new CircleShapeLevel(1820);
                    level.Create();                                        
                }

                watch.Stop();
                Debug.Log("Runtime : " + watch.ElapsedMilliseconds + "ms.");
                runtimes[i] = watch.ElapsedMilliseconds;

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
            Debug.Log("Standard Deviation : " + sd);
        }

        public void RoomCheck()
        {
            double[] rooms = new double[100];
            double[] deviation = new double[100];
            double sd;

            for (int n = 0; n < 100; n++)
            {
                CircleShapeLevel level = new CircleShapeLevel(1820);
                level.Create();
                rooms[n] = level.rooms.Count;
                Debug.Log("Independent Rooms : " + rooms[n]);
            }

            double sum = 0;
            foreach (double i in rooms)
                sum += i;
            double avg = sum / (double)100;

            double sum2 = 0;
            for (int i = 0; i < rooms.Length; i++)
            {
                deviation[i] = Math.Pow((rooms[i] - avg), 2);
                sum2 += deviation[i];
            }
            sd = Math.Sqrt(sum2 / deviation.Length);

            Debug.Log("Average Independent Rooms : " + avg);
            Debug.Log("Standard Deviation : " + sd);
        }

        public void RandomCheck()
        {
            CircleShapeLevel level = new CircleShapeLevel(1820);
            level.Create();

            int[] rnd = new int[100];
            int[,] map = level.map;

            for(int n = 0; n < 100; n++)
            {
                CircleShapeLevel level2 = new CircleShapeLevel(1820);
                level2.Create();
                int num = 0;
                int num2 = 0;

                for(int x = 0; x < level.width; x++)
                {
                    for(int y = 0; y < level.height; y++)
                    {
                        if (map[x, y] == level2.map[x, y])
                        {
                            if (map[x, y] == Terrain.VOID)
                                num2++;
                            num++;
                        }
                    }
                }
                double val = (double)num / (double)level2.map.Length;
                double val2 = (double)num2 / (double)level2.map.Length;
                Debug.Log("Randomness : " + val + "void : " + val2);
                rnd[n] = (int)(val * 100);
            }
            double sum = 0;
            foreach (int d in rnd)
                sum += d;
            Debug.Log("Average Randomness : " + (sum / rnd.Length));
        }

        public void PrintLevel()
        {
            for (int i = 0; i < level.width; i++)
            {
                for (int j = 0; j < level.height; j++)
                {
                    GameObject tileObject;
                    int tile = level.map[i, j];
                    switch (tile)
                    {
                        case Terrain.VOID:
                            //tileObject = Tiles[Array.FindIndex(Tiles, t => t.name == "void")];
                            //break;
                            continue;
                        case Terrain.GROUND:
                            tileObject = Tiles[Array.FindIndex(Tiles, t => t.name == "FloorTile")];
                            break;
                        case Terrain.WALL:
                            tileObject = Tiles[Array.FindIndex(Tiles, t => t.name == "WallTile")];
                            break;
                        default:
                            //tileObject = Tiles[Array.FindIndex(Tiles, t => t.name == "void")];
                            //break;
                            continue;
                    }
                    GameObject newTile = Instantiate(tileObject, new Vector2(i, -j), Quaternion.identity) as GameObject;
                    newTile.transform.SetParent(this.transform, false);
                }
            }//현재 레벨의 맵 화면상에 출력
        }
    }
}
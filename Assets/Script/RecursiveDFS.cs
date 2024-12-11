using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

public class RecursiveDFS : MazeLogic
{
    public List<MapLocation> directions = new List<MapLocation>()
    {
        // list ini yang akan di-shuffle
        new MapLocation(1, 0),  // move right by x + 1, z + 0 from current coordinate
        new MapLocation(0, 1),  // move up by x + 0, z + 1 from current coordinate
        new MapLocation(-1, 0), // move left by x - 1, z + 0 from current coordinate
        new MapLocation(0, -1)  // move down by x + 0, z - 1 from current coordinate
    };
    public override void GenerateMaps()
    {
        Generate(5, 5);
    }


    void Generate(int x, int z)
    {
        if (CountSquareNeighbours(x, z) >= 2) return;
        map[x, z] = 0;

        directions.Shuffle();

        Generate(x + directions[0].x, z + directions[0].z);
        Generate(x + directions[1].x, z + directions[1].z);
        Generate(x + directions[2].x, z + directions[2].z);
        Generate(x + directions[3].x, z + directions[3].z);
    }

}


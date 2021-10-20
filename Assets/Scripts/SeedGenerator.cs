using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedGenerator : MonoBehaviour
{
    int[,] grid = new int[100, 100];
    int seed;
    int seedOffset;

    Vector2Int position;

    void Start()
    {
        position = new Vector2Int(50, 50);

        seedOffset = Random.Range(0, 10000);
        seed = seedOffset;
        
        grid[position.x, position.y] = seed;
    }

    void Update()
    {
        /*if (playerWentEast)
        {
            position.x++;
        }
        else if (playerWentWest)
        {
            position.x--;
        }
        else if (playerWentNorth)
        {
            position.y++;
        }
        else if (playerWentSouth)
        {
            position.y--;
        }

        if (grid[position.x, position.y] == 0)
        {
            grid[position.x, position.y] = NewSeed();

            SpawnNewForest(grid[position.x, position.y]);
        }
        else if (grid[position.x, position.y] != 0)
        {
            SpawnNewForest(grid[position.x, position.y]);
        }*/
    }

    public int NewSeed()
    {
        seed++;
        return seed;
    }
}

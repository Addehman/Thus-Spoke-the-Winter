using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedGenerator : MonoBehaviour
{
    public event Action<int> SendSeed;
    ScreenWrap screenWrap;

    [SerializeField]Vector2Int position;
    [SerializeField]int seed;
    int seedOffset;

    int[,] grid = new int[100, 100];

    private void Awake()
    {
        screenWrap = FindObjectOfType<ScreenWrap>();
        screenWrap.PlayerTraveling += GenerateSeed;

        
    }

    private void Start()
    {
        Invoke(nameof(Init), 0.01f);
        /*Init();*/

    }

    void Init()
    {
        position = new Vector2Int(50, 50);

        seedOffset = UnityEngine.Random.Range(0, 10000);
        seed = seedOffset;
        
        grid[position.x, position.y] = seed;

/*        print($"Seed: {seed}");
        print($"Seed in grid: {grid[position.x, position.y]}. This is the seed we send in Start()");*/

        SendSeed?.Invoke(grid[position.x, position.y]);
    }

    void GenerateSeed(string latitude)
    {
        if (latitude == "east")
        {
            position.x++;
        }
        else if (latitude == "west")
        {
            position.x--;
        }
        else if (latitude == "north")
        {
            position.y++;
        }
        else if (latitude == "south")
        {
            position.y--;
        }

        if (grid[position.x, position.y] == 0)
        {
            grid[position.x, position.y] = NewSeed();

            SendSeed?.Invoke(grid[position.x, position.y]);
        }
        else if (grid[position.x, position.y] != 0)
        {
            SendSeed?.Invoke(grid[position.x, position.y]);
        }
    }

    public int NewSeed()
    {
        seed++;
        return seed;
    }
}

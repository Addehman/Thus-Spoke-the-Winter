using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedGenerator : MonoBehaviour
{
	public event Action<int> SendSeed;
	public event Action<EnergyCost> DrainEnergy;

	[SerializeField] private ScreenWrap screenWrap;
	[SerializeField] int seed = 0, worldGridSize = 100;
	public Vector2Int position;
	public int[,] worldGrid;

	int seedOffset;


	private void Awake()
	{
		screenWrap = FindObjectOfType<ScreenWrap>();
		worldGrid = new int[worldGridSize, worldGridSize];
		screenWrap.PlayerTraveling += UpdatePosition;
	}

	private void Start()
	{
		//Temporary fix because Forest Controller doesn't spawn a forest in the beginning.
		//99% chance this is because of execution order, might not have to fix this because we should spawn with the cabin in the beginning.
		// Invoke(nameof(Init), 0.01f);
		Init();
	}

	void Init()
	{
		position = new Vector2Int(worldGridSize / 2, worldGridSize / 2);
		worldGrid[position.x, position.y] = -1; // Setting the Home position to a value definitively different to the one of the Seed.

		seedOffset = UnityEngine.Random.Range(1, 10000);
		seed = seedOffset;

		/*        print($"Seed: {seed}");
				print($"Seed in grid: {grid[position.x, position.y]}. This is the seed we send in Start()");*/

		//SendSeed?.Invoke(worldGrid[position.x, position.y]); // We don't want forestController to execute this at start since we're most likely always spawning at the cabin at start. However, this might change later on!
	}

	void UpdatePosition(string latitude)
	{
		if (latitude == "east") {
			position.x++;
		}
		else if (latitude == "west") {
			position.x--;
		}
		else if (latitude == "north") {
			position.y++;
		}
		else if (latitude == "south") {
			position.y--;
		}

		GenerateSeed(position.x, position.y);
	}

	private void GenerateSeed(int x, int y)
	{ 
		if (worldGrid[x, y] == 0) {
			worldGrid[x, y] = NewSeed();
			DrainEnergy?.Invoke(EnergyCost.Small);
			SendSeed?.Invoke(worldGrid[x, y]);
		}
		else if (worldGrid[x, y] != 0) {
			SendSeed?.Invoke(worldGrid[x, y]);
		}
	}

	public int NewSeed()
	{
		seed++;
		return seed;
	}

	private void OnDestroy()
	{
		screenWrap.PlayerTraveling -= UpdatePosition;
	}
}

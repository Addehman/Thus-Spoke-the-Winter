using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedGenerator : MonoBehaviour
{
	private static SeedGenerator _instance;
	public static SeedGenerator Instance { get { return _instance; } }

	public event Action<int> SendSeed;
	public event Action<EnergyCost> DrainEnergy;
	public event Action<bool, bool, bool, bool> UpdateExploration;

	[SerializeField] private ScreenWrap _screenWrap;
	[SerializeField] private int _seed = 0, _worldGridSize = 100;

	public Vector2Int position;
	public int[,] worldGrid;
	public int currentSeed;
	public Vector2Int distanceFromHome;

	private int _seedOffset, _startPos;
	private bool _north, _east, _south, _west;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;

		worldGrid = new int[_worldGridSize, _worldGridSize];
		_screenWrap.PlayerTraveling += UpdatePosition;
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
		_startPos = _worldGridSize / 2;
		position = new Vector2Int(_startPos, _startPos);
		worldGrid[position.x, position.y] = -1; // Setting the Home position to a value definitively different to the one of the Seed.

		_seedOffset = UnityEngine.Random.Range(1, 10000);
		_seed = _seedOffset;

		/*        print($"Seed: {seed}");
				print($"Seed in grid: {grid[position.x, position.y]}. This is the seed we send in Start()");*/

		//SendSeed?.Invoke(worldGrid[position.x, position.y]); // We don't want forestController to execute this at start since we're most likely always spawning at the cabin at start. However, this might change later on!
	}

	void UpdatePosition(Latitude latitude)
	{
		if (latitude == Latitude.East)
		{
			position.x++;
		}
		else if (latitude == Latitude.West)
		{
			position.x--;
		}
		else if (latitude == Latitude.North)
		{
			position.y++;
		}
		else if (latitude == Latitude.South)
		{
			position.y--;
		}

		WhatHasBeenExplored();
		UpdateDistanceFromHome();
		GenerateSeed(position.x, position.y);
		currentSeed = worldGrid[position.x, position.y];
	}

	private void GenerateSeed(int x, int y)
	{
		if (worldGrid[x, y] == 0)
		{
			worldGrid[x, y] = NewSeed();
			DrainEnergy?.Invoke(EnergyCost.Small);
			SendSeed?.Invoke(worldGrid[x, y]);
		}
		else if (worldGrid[x, y] != 0)
		{
			SendSeed?.Invoke(worldGrid[x, y]);
		}
	}

	void WhatHasBeenExplored()
	{
		//We check if positions around us != 0 (if they have been explored that is).
		//North check
		if (worldGrid[position.x, position.y + 1] != 0)
		{
			_north = false;
		}
		else
		{
			_north = true;
		}
		//East check
		if (worldGrid[position.x + 1, position.y] != 0)
		{
			_east = false;
		}
		else
		{
			_east = true;
		}
		//South check
		if (worldGrid[position.x, position.y - 1] != 0)
		{
			_south = false;
		}
		else
		{
			_south = true;
		}
		//West check
		if (worldGrid[position.x - 1, position.y] != 0)
		{
			_west = false;
		}
		else
		{
			_west = true;
		}
		//We then let listeners know about it by sending bools for all latitudes.
		UpdateExploration?.Invoke(_north, _east, _south, _west);
	}


	public int NewSeed()
	{
		_seed++;
		return _seed;
	}

	private void UpdateDistanceFromHome()
	{
		distanceFromHome = new Vector2Int(Mathf.Abs(position.x - _startPos), Mathf.Abs(position.y - _startPos));
	}

	private void OnDestroy()
	{
		_screenWrap.PlayerTraveling -= UpdatePosition;
	}
}

public enum Latitude
{
	North, East, South, West,
}

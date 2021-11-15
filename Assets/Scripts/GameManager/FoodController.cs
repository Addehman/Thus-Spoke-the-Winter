using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FoodObjectPoolQuantitySetup
{
	[HideInInspector] public int[] quantities;
	public int blueberryAmount, lingonberryAmount, mushroomAmount,
		fruitTree_1Amount, fruitTree_2Amount, fruitTree_3Amount;
}

[Serializable]
public class FoodObjectPoolPrefabLibrary
{
	public GameObject[] prefabs = new GameObject[6];
}

public class FoodController : MonoBehaviour
{
	private static FoodController _instance;
	public static FoodController Instance { get { return _instance; } }

	public event Action OnClearFoods;

	[SerializeField] private Transform _foodParent;
	[SerializeField] private LayerMask _ground, _forestObjects;
	[SerializeField] private PlayerController _player;
	[SerializeField] private SeedGenerator _seedGenerator;
	[SerializeField] private int _currentSeed, foodRarityWeight;
	[SerializeField] private FoodObjectPoolQuantitySetup _objectQuantitySetup;
	[SerializeField] private FoodObjectPoolPrefabLibrary _objectPrefabLibrary;
	[Space(10)]
	[SerializeField] private int minSpawnAmount = 2;
	[SerializeField] private int maxSpawnAmount = 15;


	private Camera _camera;
	private Dictionary<int, List<string>> _blacklistDictionary = new Dictionary<int, List<string>>();
	private List<string> _tempBlacklist;
	private List<Transform> initialSpawns;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;

		_camera = Camera.main;
		_seedGenerator.SendSeed += SpawnFoods;
		_player.ResourceGathered += SaveIDToBlacklist;
	}

	private void Start()
	{
		InitializeObjectPool();
	}

	public void SpawnFoods(int seed)
	{
		_currentSeed = seed;

		/*Debug.LogWarning($"Seed is {_currentSeed}");*/

		if (_blacklistDictionary.TryGetValue(_currentSeed, out List<string> result))
		{
			_tempBlacklist = result;
		}
		else
		{
			_tempBlacklist = new List<string>();
		}

		ClearFoods();

		List<int> usedRandomNumbers = new List<int>();

		UnityEngine.Random.InitState(_currentSeed);

		//We run CheckRarityTier before setting the spawnCount, so the "-foodRarityWeight"
		//value is updated before selecting how much food that can spawn.
		CheckRarityTier();
		if (foodRarityWeight == FoodObjectPool.Instance.foodObjectPool.Length)
			return;

		int spawnCount = UnityEngine.Random.Range(minSpawnAmount, maxSpawnAmount - foodRarityWeight);
		/*print($"Amount of new Foods: {spawnCount}");*/


		for (int i = 0; i < spawnCount; i++)
		{
			int availableObjectsLength = FoodObjectPool.Instance.foodObjectPool.Length - foodRarityWeight;

			int randomObject = UnityEngine.Random.Range(0, availableObjectsLength);

			while (usedRandomNumbers.Contains(randomObject))
			{
				randomObject = (randomObject + 1) % availableObjectsLength;
			}
			usedRandomNumbers.Add(randomObject);
			/*print($"randomObject nr: {randomObject}");*/
			Transform newObject = FoodObjectPool.Instance.foodObjectPool[randomObject];

			newObject.position = GenerateRandomPosition();

			int randomID_1 = UnityEngine.Random.Range(0, 1000000);
			int randomID_2 = UnityEngine.Random.Range(0, 1000000);

			newObject.gameObject.name = $"{randomID_1}{randomID_2}";

			if (IsObjectBlacklisted(newObject)) // THE DAMN FRUITS AREN'T TURNED OFF!!! The tree is set to depleted if blacklisted, but the fruits aren't considered at all!! :DDD
			{
				continue;
			}

			//newObject.position = PositionCorrection(newObject.position);

			//if (!CanObjectSpawnThisSeason()) return;

			SpawnFruitsIfFruitTree(newObject);

			newObject.gameObject.SetActive(true);
		}
	}

	private void ClearFoods()
	{
		foreach (Transform food in _foodParent)
		{
			OnClearFoods?.Invoke();
			food.gameObject.SetActive(false);
		}
	}

	private Vector3 GenerateRandomPosition()
	{
		Vector3 randomPosition = Vector3.zero;

		System.Random randomPos = new System.Random(UnityEngine.Random.Range(0, 10000));

		float randomViewPortPosX = UnityEngine.Random.Range(0.1f, 0.9f);
		float randomViewPortPosY = UnityEngine.Random.Range(0.1f, 0.9f);

		Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width * randomViewPortPosX, Screen.height * randomViewPortPosY));
		RaycastHit hit;

		int tries = 0;
		while (Physics.Raycast(ray, out hit, float.MaxValue, _forestObjects))
		{
			randomViewPortPosX = randomPos.Next(1000, 9000) / 10000f;
			randomViewPortPosY = randomPos.Next(1000, 9000) / 10000f;
			ray = _camera.ScreenPointToRay(new Vector3(Screen.width * randomViewPortPosX, Screen.height * randomViewPortPosY));

			/*print($"Food Ray hit: {hit.transform.name}. Trying again.");*/

			tries++;
			if (tries > 9) break;
		}

		if (Physics.Raycast(ray, out hit, float.MaxValue, _ground))
		{
			randomPosition = hit.point;
		}

		if (hit.collider is null)
		{
			throw new System.Exception($"{ray} did not hit");
		}
		return randomPosition;
	}

	private bool IsObjectBlacklisted(Transform obj)
	{
		if (obj.TryGetComponent(out FoodBehaviour food))
		{
			if (_tempBlacklist.Count > 0 && _tempBlacklist.Contains(obj.gameObject.name))
			{
				print($"{obj} is blacklisted!");

				if ((food.type == ResourceType.apple || food.type == ResourceType.mushroom)) // it's redundant to check for apples as they're not spawned the same way as the other foods, and are checked before they're spawned.
				{
					return true;
				}
				else
				{
					obj.gameObject.SetActive(true);
					//obj.position = PositionCorrection(obj.position);
					food.IsDepleted(true);
					return true;
				}
			}
			else
			{
				food.IsDepleted(false);
				return false;
			}
		}
		else
		{
			obj.TryGetComponent(out TreeBehaviour tree);

			if (_tempBlacklist.Count > 0 && _tempBlacklist.Contains(obj.gameObject.name))
			{
				print($"{obj} is blacklisted!");
				obj.gameObject.SetActive(true);
				tree.SetTreeToDead();
				return true;
			}
			else
			{
				tree.UpdateState(SeasonController.Instance.currentSeason);
				return false;
			}
		}
	}

	//private Vector3 PositionCorrection(Vector3 correction)
	//{
	//	correction.y = 0f;
	//	return correction;
	//}

	private void InitializeObjectPool()
	{
		_objectQuantitySetup.quantities = new int[6] { _objectQuantitySetup.blueberryAmount, _objectQuantitySetup.lingonberryAmount, _objectQuantitySetup.mushroomAmount,
		_objectQuantitySetup.fruitTree_1Amount, _objectQuantitySetup.fruitTree_2Amount, _objectQuantitySetup.fruitTree_3Amount };

		foodRarityWeight = 0;
		for (int i = 0; i < _objectQuantitySetup.quantities.Length; i++)
		{
			foodRarityWeight += _objectQuantitySetup.quantities[i];
		}

		initialSpawns = new List<Transform>();

		for (int i = 0; i < _objectQuantitySetup.quantities.Length; i++)
		{
			if (_objectQuantitySetup.quantities[i] <= 0) continue;

			SpawnThisTypeThisMany(_objectPrefabLibrary.prefabs[i], _objectQuantitySetup.quantities[i]);
		}

		FoodObjectPool.Instance.AddFoodObjectsToArray(initialSpawns);
	}

	private void SpawnThisTypeThisMany(GameObject type, int typeAmount)
	{
		GameObject spawn = null;
		for (int i = 0; i < typeAmount; i++)
		{
			spawn = Instantiate(type, _foodParent);
			spawn.SetActive(false);
			initialSpawns.Add(spawn.transform);
		}
	}

	private void SaveIDToBlacklist(GameObject obj)
	{
		if (!obj.TryGetComponent(out FoodBehaviour food) && obj.TryGetComponent(out TreeBehaviour tree) && tree.type != ResourceType.fruitTree)
		{
			return;
		}

		print($"{obj.name} is now blacklisted!");

		_tempBlacklist.Add(obj.name);

		if (_tempBlacklist.Count != 1)
		{
			_blacklistDictionary[_currentSeed] = _tempBlacklist;
		}
		else
		{
			_blacklistDictionary.Add(_currentSeed, _tempBlacklist);
		}
	}

	private void CheckRarityTier() // this needs to more Modular!
	{
		int dominant = _seedGenerator.distanceFromHome.x >= _seedGenerator.distanceFromHome.y ? _seedGenerator.distanceFromHome.x : _seedGenerator.distanceFromHome.y;
		if (dominant > 1 && dominant <= 5)
		{
			foodRarityWeight = 30;
		}
		else if (dominant > 5 && dominant <= 10)
		{
			foodRarityWeight = 25;
		}
		else if (dominant > 10 && dominant <= 20)
		{
			foodRarityWeight = 15;
		}
		else if (dominant > 20)
		{
			foodRarityWeight = 0;
		}
		else
		{
			foodRarityWeight = 0;
			for (int i = 0; i < _objectQuantitySetup.quantities.Length; i++)
			{
				foodRarityWeight += _objectQuantitySetup.quantities[i];
			}
		}
	}

	private void SpawnFruitsIfFruitTree(Transform obj)
	{
		int counter = 0;
		string appleParent = obj.name;

		if (obj.TryGetComponent(out TreeBehaviour tree) && tree.type == ResourceType.fruitTree)
		{
			if (tree.status == Status.Dead) return;

			print($"The Status of: {obj} is now: {tree.status}");

			foreach (Transform item in obj)
			{
				item.gameObject.name = $"{appleParent}: {counter++}";

				if (_tempBlacklist.Count > 0 && _tempBlacklist.Contains(item.gameObject.name))
				{
					print($"{item.gameObject.name} is blacklisted!");
					item.gameObject.SetActive(false);
					continue;
				}
				else
				{
					/*Debug.LogWarning($"MY APPLES WASN'T BLACKLISTED! {item.gameObject.name}");*/
					item.TryGetComponent(out FoodBehaviour food);
					food.health = food.data.health;
					food.status = Status.Alive;
					item.gameObject.SetActive(true);
				}
			}
			// Update/Fill list on this newObject with event
			tree.AddFruitsToList();
		}
	}

	private void OnDestroy()
	{
		_seedGenerator.SendSeed -= SpawnFoods;
		_player.ResourceGathered -= SaveIDToBlacklist;
	}
}

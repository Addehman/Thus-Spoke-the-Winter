using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ObjectPoolQuantitySetup
{
	[HideInInspector] public int[] quantities;
	public int leafTree_1Amount, leafTree_2Amount, leafTree_3Amount, leafTree_4Amount,
		leafTree_5Amount, pineTreeAmount, tallPineTreeAmount, blueberryAmount,
		lingonberryAmount, mushroomAmount, fruitTree_1Amount, fruitTree_2Amount,
		fruitTree_3Amount;
}

[Serializable]
public class ObjectPoolPrefabLibrary
{
	public GameObject[] prefabs = new GameObject[13];
}

public class ForestController : MonoBehaviour
{
	private static ForestController _instance;
	public static ForestController Instance { get { return _instance; } }

	public event Action OnClearForest;

	[SerializeField] private Transform _forestParent;
	[SerializeField] private LayerMask _ground;
	[SerializeField] private LayerMask _default;
	[SerializeField] private PlayerController _player;
	[SerializeField] private SeedGenerator _seedGenerator;
	[SerializeField] private int _currentSeed;
	[SerializeField] private ObjectPoolQuantitySetup _objectPoolQuantitySetup;
	[SerializeField] private ObjectPoolPrefabLibrary _objectPoolPrefabLibrary;
	[SerializeField] private int foodRarityWeight;
	[SerializeField] private int uniqueFoodObjects = 6;
	[Space(10)]
	[SerializeField] private int minSpawnAmount = 5;
	[SerializeField] private int maxSpawnAmount = 50;

	private GameObject _cabinParent;

	private Camera _camera;
	private Dictionary<int, List<string>> _blackListDictionary = new Dictionary<int, List<string>>();
	private List<string> _tempBlacklist;
	private List<Transform> tempSpawns;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;

		_camera = Camera.main;
		_seedGenerator.SendSeed += SpawnForest;
		_player.ResourceGathered += SaveIDToBlacklist;
	}

	private void Start()
	{
		InitialSpawn();
	}

	public void SetCabinParent(GameObject obj)
	{
		_cabinParent = obj;
	}

	public void SpawnForest(int seed)
	{
		_currentSeed = seed;

		if (_blackListDictionary.TryGetValue(_currentSeed, out List<string> result))
		{
			_tempBlacklist = result;
		}
		else
		{
			_tempBlacklist = new List<string>();
		}

		print($"Seed: {seed}");
		ClearForest();
		// Check if the incoming seed number here is "-1", this means it's the Home block and no forest should spawn.
		if (seed == -1)
		{
			print("Spawning Cabin");
			SceneController.Instance.LoadScene("CabinScene");
			return;
		}
		else if (SceneController.Instance.IsCurrentSceneName("CabinScene"))
		{
			_cabinParent.SetActive(false);
			SceneController.Instance.LoadScene("ForestScene");
		}

		List<int> usedRandomNumbers = new List<int>();

		UnityEngine.Random.InitState(seed); // To be used to control the seed of the random forest, whether it should be random or not.

		int spawnCount = UnityEngine.Random.Range(minSpawnAmount, maxSpawnAmount);
		print("Amount of new Objects: " + spawnCount);

		CheckRarityTier();

		for (int i = 0; i < spawnCount; i++)
		{
			//This needs to check so that we don't random the same number twice in a row or something like that.
			int randomObject = UnityEngine.Random.Range(0, ForestObjectPool.Instance.forestObjectPool.Length - foodRarityWeight); // This number should be a variable

			while (usedRandomNumbers.Contains(randomObject))
			{
				randomObject = (randomObject + 1) % ForestObjectPool.Instance.forestObjectPool.Length;
			}
			usedRandomNumbers.Add(randomObject);

			Transform newObject = ForestObjectPool.Instance.forestObjectPool[randomObject];

			//A way to make the seed control what the random name is going to be.
			int randomID1 = UnityEngine.Random.Range(0, 1000000);
			int randomID2 = UnityEngine.Random.Range(0, 1000000);

			// Here we use a Raycast to find a random position for the newObject to be positioned to.
			Vector3 randomWorldPos = GenerateRandomPosition();

			//float randomOffset = UnityEngine.Random.Range(-1f, 1f);
			//randomWorldPos.z += randomOffset;

			newObject.position = randomWorldPos;

			//Om vi får seed = 1. Blir namnet sedan 2000.
			//Vi plockar upp item med ID 2000.
			//Vår lista lägger till ett item med ID 2000 på blacklisten.
			//Vi går vidare. I nästa ruta får vi seed = 2. Random lyckas välja samma objekt i hierarkin och ger den den här gången ID 5000.
			//Vi plockar upp objektet med ID 5000. Vi lägger till ID 5000 på blacklisten.
			//När vi tillslut går tillbaka till platsen med seed = 1 randomas ID 2000 fram igen.
			//Vi checkar blacklisten. Blacklisten innehåller 2000. Sätt inte objektet aktivt.

			//Vi kan eventuellt strunta i att ge alla objekt ett random namn varje gång utan bara ge alla ett random namn första gången.
			//För att spara prestanda. I och med att vi ändå alltid har koll på vilken seed vi är på med Dictionariet.
			newObject.gameObject.name = $"{randomID1}{randomID2}";

			if (_tempBlacklist.Count > 0 && _tempBlacklist.Contains(newObject.gameObject.name))
			{
				print($"{newObject.name} is blacklisted!");
				continue;
			}

			newObject.gameObject.SetActive(true);

			if (newObject.TryGetComponent(out TreeBehaviour tree) && tree.type == ResourceType.fruitTree)
			{
				foreach (Transform item in newObject)
				{
					item.gameObject.name = item.gameObject.GetInstanceID().ToString();
					if (_tempBlacklist.Count > 0 && _tempBlacklist.Contains(item.gameObject.name))
					{
						print($"{item.gameObject.name} is blacklisted!");
						item.gameObject.SetActive(false);
						continue;
					}
					else
					{
						item.gameObject.SetActive(true);
					}
				}
				// Update/Fill list on this newObject with event
				tree.AddFruitsToList();
			}

			//Another way to give the spawned object a unique ID as name. Does not utilize seed though, so not for us right now.
			//newObject.name = Guid.NewGuid().ToString();

			// Here we make sure that the spawned object is not in the air.
			Vector3 positionCorrection = newObject.position;
			positionCorrection.y = 0f;
			newObject.position = positionCorrection;
		}
	}

	private void SaveIDToBlacklist(GameObject obj)
	{
		print($"{obj.name} is now blacklisted!");

		_tempBlacklist.Add(obj.name);

		if (_tempBlacklist.Count != 1)
		{
			_blackListDictionary[_currentSeed] = _tempBlacklist;
		}
		else
		{
			_blackListDictionary.Add(_currentSeed, _tempBlacklist);
		}
	}

	private void ClearForest()
	{
		foreach (Transform forestObject in _forestParent)
		{
			OnClearForest?.Invoke();
			forestObject.gameObject.SetActive(false);
		}
	}
	/// <summary>
	/// Here we fill the ObjectPool up with managable accuracy concerning the amounts for each type - Spawns all the objects that will be possible to spawn on each block of forest.
	/// </summary>
	private void InitialSpawn()
	{
		_objectPoolQuantitySetup.quantities = new int[13] { _objectPoolQuantitySetup.leafTree_1Amount, _objectPoolQuantitySetup.leafTree_2Amount, _objectPoolQuantitySetup.leafTree_3Amount,
			_objectPoolQuantitySetup.leafTree_4Amount, _objectPoolQuantitySetup.leafTree_5Amount, _objectPoolQuantitySetup.pineTreeAmount, _objectPoolQuantitySetup.tallPineTreeAmount,
			_objectPoolQuantitySetup.blueberryAmount, _objectPoolQuantitySetup.lingonberryAmount, _objectPoolQuantitySetup.mushroomAmount,
			_objectPoolQuantitySetup.fruitTree_1Amount, _objectPoolQuantitySetup.fruitTree_2Amount, _objectPoolQuantitySetup.fruitTree_3Amount};

		foodRarityWeight = 0;
		for (int i = _objectPoolQuantitySetup.quantities.Length - uniqueFoodObjects; i < _objectPoolQuantitySetup.quantities.Length; i++)
		{
			foodRarityWeight += _objectPoolQuantitySetup.quantities[i];
		}

		tempSpawns = new List<Transform>();

		for (int i = 0; i < _objectPoolQuantitySetup.quantities.Length; i++)
		{
			if (_objectPoolQuantitySetup.quantities[i] <= 0) continue;

			SpawnThisTypeThisMany(_objectPoolPrefabLibrary.prefabs[i], _objectPoolQuantitySetup.quantities[i]);
		}

		ForestObjectPool.Instance.AddForestObjectsToList(tempSpawns);
	}

	private void SpawnThisTypeThisMany(GameObject type, int typeAmount)
	{
		GameObject spawn = null;
		for (int i = 0; i < typeAmount; i++)
		{
			spawn = Instantiate(type, _forestParent);
			spawn.SetActive(false);
			tempSpawns.Add(spawn.transform);
		}
	}

	private void CheckRarityTier()
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
			for (int i = _objectPoolQuantitySetup.quantities.Length - uniqueFoodObjects; i < _objectPoolQuantitySetup.quantities.Length; i++)
			{
				foodRarityWeight += _objectPoolQuantitySetup.quantities[i];
			}
		}
	}

	/// <summary>
	/// Use this to Generate a new Random Position on the Screen using the Camera.
	/// </summary>
	/// <returns>A random Vector3-position within the screen.</returns>
	private Vector3 GenerateRandomPosition()
	{
		Vector3 randomPosition = Vector3.zero;

		// The X and Y positions ranging from min to max of what the camera displays,
		// with a padding to make sure that an object doesn't block the player entering a block of forest.
		float randomViewPortPosX = UnityEngine.Random.Range(0.1f, 0.9f);
		float randomViewPortPosY = UnityEngine.Random.Range(0.1f, 0.9f);

		// Change the randomWorldPos if it's placed too close to another object.
		Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width * randomViewPortPosX, Screen.height * randomViewPortPosY));
		RaycastHit hit;

		int tries = 0;
		while (Physics.Raycast(ray, out hit, float.MaxValue, _default))
		{
			randomViewPortPosX = UnityEngine.Random.Range(0.1f, 0.9f);
			randomViewPortPosY = UnityEngine.Random.Range(0.1f, 0.9f);
			ray = _camera.ScreenPointToRay(new Vector3(Screen.width * randomViewPortPosX, Screen.height * randomViewPortPosY));

			//print($"Ray hit: {hit.transform.name}. Trying again.");

			tries++;
			if (tries > 9) break;
		}

		if (Physics.Raycast(ray, out hit, float.MaxValue, _ground))
		{
			randomPosition = hit.point;
			//print($"Ray hit: {hit.transform.name}. Sticking with this.");
		}

		if (hit.collider is null)
		{
			throw new System.Exception($"{ray} did not hit");
		}
		return randomPosition;
	}

	private void OnDestroy()
	{
		_seedGenerator.SendSeed -= SpawnForest;
		_player.ResourceGathered -= SaveIDToBlacklist;
	}
}
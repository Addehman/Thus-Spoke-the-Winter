using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class TreeObjectPoolQuantitySetup
{
	[HideInInspector] public int[] quantities;
	public int leafTree_1Amount, leafTree_2Amount, leafTree_3Amount, leafTree_4Amount,
		leafTree_5Amount, pineTreeAmount, tallPineTreeAmount;
}

[Serializable]
public class ObjectPoolPrefabLibrary
{
	public GameObject[] prefabs = new GameObject[7];
}

public class TreeController : MonoBehaviour
{
	private static TreeController _instance;
	public static TreeController Instance { get { return _instance; } }

	public event Action OnClearTrees;

	[SerializeField] private Transform _treeParent;
	[SerializeField] private LayerMask _ground;
	[SerializeField] private LayerMask _forestObjects;
	[SerializeField] private PlayerController _player;
	[SerializeField] private SeedGenerator _seedGenerator;
	[SerializeField] private int _currentSeed;
	[SerializeField] private TreeObjectPoolQuantitySetup _objectQuantitySetup;
	[SerializeField] private ObjectPoolPrefabLibrary _objectPrefabLibrary;
	[Space(10)]
	[SerializeField] private int minSpawnAmount = 5;
	[SerializeField] private int maxSpawnAmount = 50;

	private Camera _camera;
	private Dictionary<int, List<string>> _blackListDictionary = new Dictionary<int, List<string>>();
	private List<string> _tempBlacklist;
	private List<Transform> initialSpawns;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;

		_camera = Camera.main;
		_seedGenerator.SendSeed += SpawnTrees;
		_player.ResourceGathered += SaveIDToBlacklist;
	}

	private void Start()
	{
		InitializeObjectPool();
	}

	public void SpawnTrees(int seed)
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
			return;

		List<int> usedRandomNumbers = new List<int>();

		UnityEngine.Random.InitState(seed); // To be used to control the seed of the random forest, whether it should be random or not.

		int spawnCount = UnityEngine.Random.Range(minSpawnAmount, maxSpawnAmount);
		print("Amount of new Trees: " + spawnCount);

		for (int i = 0; i < spawnCount; i++)
		{
			//This needs to check so that we don't random the same number twice in a row or something like that.
			int randomObject = UnityEngine.Random.Range(0, TreeObjectPool.Instance.treeObjectPool.Length); // This number should be a variable

			while (usedRandomNumbers.Contains(randomObject))
			{
				randomObject = (randomObject + 1) % TreeObjectPool.Instance.treeObjectPool.Length;
			}
			usedRandomNumbers.Add(randomObject);

			Transform newObject = TreeObjectPool.Instance.treeObjectPool[randomObject];

			//A way to make the seed control what the random name is going to be.
			int randomID_1 = UnityEngine.Random.Range(0, 1000000);
			int randomID_2 = UnityEngine.Random.Range(0, 1000000);

			//float randomOffset = UnityEngine.Random.Range(-1f, 1f);
			//randomWorldPos.z += randomOffset;

			newObject.position = GenerateRandomPosition();

			//Om vi får seed = 1. Blir namnet sedan 2000.
			//Vi plockar upp item med ID 2000.
			//Vår lista lägger till ett item med ID 2000 på blacklisten.
			//Vi går vidare. I nästa ruta får vi seed = 2. Random lyckas välja samma objekt i hierarkin och ger den den här gången ID 5000.
			//Vi plockar upp objektet med ID 5000. Vi lägger till ID 5000 på blacklisten.
			//När vi tillslut går tillbaka till platsen med seed = 1 randomas ID 2000 fram igen.
			//Vi checkar blacklisten. Blacklisten innehåller 2000. Sätt inte objektet aktivt.

			//Vi kan eventuellt strunta i att ge alla objekt ett random namn varje gång utan bara ge alla ett random namn första gången.
			//För att spara prestanda. I och med att vi ändå alltid har koll på vilken seed vi är på med Dictionariet.
			newObject.gameObject.name = $"{randomID_1}{randomID_2}";

			if (IsObjectBlacklisted(newObject))
				continue;

			// Here we make sure that the spawned object is not in the air.
			/*Vector3 positionCorrection = newObject.position;
			positionCorrection.y = 0f;
			newObject.position = positionCorrection;*/
			newObject.position = PositionCorrection(newObject.position);

			newObject.gameObject.SetActive(true);

			//Another way to give the spawned object a unique ID as name. Does not utilize seed though, so not for us right now.
			//newObject.name = Guid.NewGuid().ToString();
		}
	}

	private bool IsObjectBlacklisted(Transform obj)
	{
		obj.TryGetComponent(out TreeBehaviour tree);

		if (_tempBlacklist.Count > 0 && _tempBlacklist.Contains(obj.gameObject.name))
		{
			print($"{obj.name} is blacklisted!");
			// here we should check if it's mushroom or apple, then do like we have done
			
			obj.gameObject.SetActive(true);
			obj.position = PositionCorrection(obj.position);
			tree.SetSpriteToDepleted();
			return true;
		}
		else // Not on Blacklist - simply update the sprites to not look depleted/cut down
		{
			tree.UpdateState(SeasonController.Instance.currentSeason);
			return false;
		}
	}

	private void SpawnFruitsIfFruitTree(Transform obj)
	{
		if (obj.TryGetComponent(out TreeBehaviour fruitTree) && fruitTree.type == ResourceType.fruitTree)
		{
			foreach (Transform item in obj)
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
			fruitTree.AddFruitsToList();
		}
	}

	private Vector3 PositionCorrection(Vector3 correction)
	{
		correction.y = 0f;
		return correction;
	}

	private void SaveIDToBlacklist(GameObject obj)
	{
		if (!obj.TryGetComponent(out TreeBehaviour tree)) return;

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
		foreach (Transform forestObject in _treeParent)
		{
			OnClearTrees?.Invoke();
			forestObject.gameObject.SetActive(false);
		}
	}
	/// <summary>
	/// Here we fill the ObjectPool up with managable accuracy concerning the amounts for each type - Spawns all the objects that will be possible to spawn on each block of forest.
	/// </summary>
	private void InitializeObjectPool()
	{
		_objectQuantitySetup.quantities = new int[7] { _objectQuantitySetup.leafTree_1Amount, _objectQuantitySetup.leafTree_2Amount, _objectQuantitySetup.leafTree_3Amount,
			_objectQuantitySetup.leafTree_4Amount, _objectQuantitySetup.leafTree_5Amount, _objectQuantitySetup.pineTreeAmount, _objectQuantitySetup.tallPineTreeAmount};

		initialSpawns = new List<Transform>();

		for (int i = 0; i < _objectQuantitySetup.quantities.Length; i++)
		{
			// if the amount is set to 0, then we don't want this type -> continue to next iteration in loop.
			if (_objectQuantitySetup.quantities[i] <= 0) continue;

			SpawnThisTypeThisMany(_objectPrefabLibrary.prefabs[i], _objectQuantitySetup.quantities[i]);
		}

		TreeObjectPool.Instance.AddTreeObjectsToArray(initialSpawns);
	}

	private void SpawnThisTypeThisMany(GameObject type, int typeAmount)
	{
		GameObject spawn = null;
		for (int i = 0; i < typeAmount; i++)
		{
			spawn = Instantiate(type, _treeParent);
			spawn.SetActive(false);
			initialSpawns.Add(spawn.transform);
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
		while (Physics.Raycast(ray, out hit, float.MaxValue, _forestObjects))
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
		_seedGenerator.SendSeed -= SpawnTrees;
		_player.ResourceGathered -= SaveIDToBlacklist;
	}
}
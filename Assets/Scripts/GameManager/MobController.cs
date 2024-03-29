using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class MobObjectPoolQuantitySetup
{
	[HideInInspector] public int[] quantities;
	public int whiteBunny_Amount, brownBunny_Amount;
}

[Serializable]
public class MobObjectPoolPrefabLibrary
{
	public GameObject[] prefabs = new GameObject[2];
}

public class MobController : MonoBehaviour
{
	private static MobController _instance;
	public static MobController Instance { get { return _instance; } }

	public event Action OnClearMobs;

	[SerializeField] private Transform _mobParent;
	[SerializeField] private LayerMask _ground;
	[SerializeField] private LayerMask _forestObjects;
	[SerializeField] private PlayerController _player;
	[SerializeField] private Vector2 _playerViewPortPos;
	[SerializeField] private Vector2 _mobViewPortPos;
	[SerializeField] private Vector3 _mobWorldPos;
	[SerializeField] private SeedGenerator _seedGenerator;
	[SerializeField] private int _currentSeed;
	[SerializeField] private MobObjectPoolQuantitySetup _objectQuantitySetup;
	[SerializeField] private MobObjectPoolPrefabLibrary _objectPrefabLibrary;
	[SerializeField] private int mobRarityWeight;
	[SerializeField] private int uniqueMobObjects = 1;
	[Space(10)]
	[SerializeField] private int minSpawnAmount = 1;
	[SerializeField] private int maxSpawnAmount = 1;
	[SerializeField] private int _lengthOfSmellTrail = 4;
	[SerializeField] private int _spawnRandomMax = 1000;
	[SerializeField] private float _spawnChancePercentage;

	public List<int> seedsWithSmell = new List<int>();
	public Dictionary<string, Vector3> tempSavedDeadMobDictionary;

	private GameObject _cabinParent;
	private Camera _camera;
	private Dictionary<int, Dictionary<string, Vector3>> _savedDeadMobDictionary = new Dictionary<int, Dictionary<string, Vector3>>();
	private List<Transform> tempSpawns;



	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;

		_camera = Camera.main;
		_seedGenerator.SendSeed += SpawnLottery;
		//_player.ResourceGathered += RemoveButcheredFromDeadMobDictionary;
	}

	private void Start()
	{
		InitializeObjectPool();
	}

	private void Update()
	{
		/*_playerViewPortPos = _camera.WorldToViewportPoint(_player.transform.position);
		_VECTOR2_playerViewPos = _camera.WorldToViewportPoint(_player.transform.position);

		Vector3 tempMaxVector = new Vector3(1, 1, 1);

		_mobViewPortPos = tempMaxVector - _playerViewPortPos;

		_mobWorldPos = _camera.ViewportToWorldPoint(_mobViewPortPos);*/
	}

	public void SetCabinParent(GameObject obj)
	{
		_cabinParent = obj;
	}

	private void SpawnLottery(int seed)
	{
		_currentSeed = seed;

		bool isSmellySeed = false;

		if (_savedDeadMobDictionary.TryGetValue(_currentSeed, out Dictionary<string, Vector3> result))
		{
			tempSavedDeadMobDictionary = result;
		}
		else
		{
			tempSavedDeadMobDictionary = new Dictionary<string, Vector3>();
		}

		ClearMobs();

		int spawnLotteryOutcome = UnityEngine.Random.Range(0, _spawnRandomMax);

		// If we revisit a square that we've been to recently, and it's within the smell trail,
		// then remove it from it's older position in the list and add it again, to set it to the most recent visited square - last in list.
		if (seedsWithSmell.Contains(seed))
		{
			seedsWithSmell.Remove(seed);
			seedsWithSmell.Add(seed);
			isSmellySeed = true;
			print("It's an smelly seed, no bunnies should spawn");
		}
		// If the list is full, remove the oldest entry and then add new to last spot(automatic), pushing older ones back.
		else if (seedsWithSmell.Count == _lengthOfSmellTrail)
		{
			seedsWithSmell.RemoveAt(0);
			seedsWithSmell.Add(seed);
		}
		else // The list is probably empty so we fill the list with this seed to make sure that the list is full - trapController depend on this.
		{
			for (int i = 0; i < _lengthOfSmellTrail; i++)
			{
				seedsWithSmell.Add(seed);
			}
		}

		// If there is any Saved Dead Mobs in this seed:
		if (tempSavedDeadMobDictionary.Count > 0)
		{
			SpawnMob(seed);
			return;
		}
		// If it's a seed that still smells like player, Or if there is a trapped animal there, then don't go further.
		else if (isSmellySeed || TrapController.Instance.CheckIfThereAreAnyTrappedMobs(seed))
		{
			return;
		}

		// If it's Spring, early or late, then it's high season for animals(mating season = high activity / bold behaviour).
		if (SeasonController.Instance.currentSeason <= Seasons.lateSpring)
			_spawnChancePercentage = 0.5f;
		else if (SeasonController.Instance.currentSeason == Seasons.earlySummer)
			_spawnChancePercentage = 0.3f;
		else
			_spawnChancePercentage = 0.1f; // Maybe even less? 5%?

		print($"Bunny Lottery outcome: {spawnLotteryOutcome}");
		int spawnOdds = Mathf.RoundToInt(_spawnRandomMax * _spawnChancePercentage);
		print($"The MobSpawnOdds are: {spawnOdds}");
		if (spawnLotteryOutcome <= spawnOdds)
		{
			print($"Spawning a bunny on seed: {seed}");
			SpawnMob(seed);
		}
	}

	public void SpawnMob(int seed)
	{
		//print($"Seed: {seed}");

		// Check if the incoming seed number here is "-1", this means it's the Home block and no forest should spawn.
		if (seed == -1)
		{
			return;
		}

		List<int> usedRandomNumbers = new List<int>();

		UnityEngine.Random.InitState(seed); //To be used to control the seed of the random mob spawns, whether it should be random or not.

		int spawnCount = UnityEngine.Random.Range(minSpawnAmount, maxSpawnAmount);
		print("Amount of new Objects: " + spawnCount);

		/*CheckRarityTier();*/ //Remove comment when testing is done and we want to implement the rarity of mobs aswell.

		int counter = 0;

		for (int i = 0; i < spawnCount; i++)
		{
			//This needs to check so that we don't random the same number twice in a row or something like that.
			int randomObject = UnityEngine.Random.Range(0, MobObjectPool.Instance.mobObjectPool.Length);

			while (usedRandomNumbers.Contains(randomObject))
			{
				randomObject = (randomObject + 1) % MobObjectPool.Instance.mobObjectPool.Length;
			}
			usedRandomNumbers.Add(randomObject);

			Transform newObject = MobObjectPool.Instance.mobObjectPool[randomObject];

			//A way to make the seed control what the random name is going to be.
			//int randomID1 = UnityEngine.Random.Range(0, 1000000);
			//int randomID2 = UnityEngine.Random.Range(0, 1000000);

			newObject.position = GeneratePosition();

			newObject.gameObject.name = $"{_currentSeed} {counter++}";

			newObject.TryGetComponent(out MobBehaviour mob);

			if (CheckSavedDeadMobList(newObject, mob))
			{
				//SetActive dead mob
				mob.IsDepleted(true);
				mob.status = Status.Dead;
				// return;
			}
			else 
			{
				mob.IsDepleted(false);
				mob.status = Status.Alive;
			}

			newObject.gameObject.SetActive(true);

			//newObject.position = PositionCorrection(newObject.position);
		}
	}

	private Vector3 PositionCorrection(Vector3 correction)
	{
		correction.y = 0f;
		return correction;
	}

	private void ClearMobs()
	{
		foreach (Transform mobObject in _mobParent)
		{
			OnClearMobs?.Invoke();
			mobObject.gameObject.SetActive(false);
		}
	}
	/// <summary>
	/// Here we fill the ObjectPool up with managable accuracy concerning the amounts for each type - Spawns all the objects that will be possible to spawn on each block of forest.
	/// </summary>
	private void InitializeObjectPool()
	{
		_objectQuantitySetup.quantities = new int[2] { _objectQuantitySetup.brownBunny_Amount, _objectQuantitySetup.whiteBunny_Amount };

		/*mobRarityWeight = 0;
		for (int i = _mobObjectPoolQuantitySetup.quantities.Length - uniqueMobObjects; i < _mobObjectPoolQuantitySetup.quantities.Length; i++)
		{
			mobRarityWeight += _mobObjectPoolQuantitySetup.quantities[i];
		}*/

		tempSpawns = new List<Transform>();

		for (int i = 0; i < _objectQuantitySetup.quantities.Length; i++)
		{
			if (_objectQuantitySetup.quantities[i] <= 0) continue;

			SpawnThisTypeThisMany(_objectPrefabLibrary.prefabs[i], _objectQuantitySetup.quantities[i]);
		}
		MobObjectPool.Instance.AddMobObjectsToList(tempSpawns);
	}

	private void SpawnThisTypeThisMany(GameObject type, int typeAmount)
	{
		GameObject spawn = null;
		for (int i = 0; i < typeAmount; i++)
		{
			spawn = Instantiate(type, _mobParent);
			spawn.SetActive(false);
			tempSpawns.Add(spawn.transform);
		}
	}

	public void SaveIDToBlacklist(GameObject obj)
	{
		if (!obj.TryGetComponent(out MobBehaviour mob) || tempSavedDeadMobDictionary.ContainsKey(obj.name)) 
			return;

		print($"{obj.name} is now blacklisted!");

		tempSavedDeadMobDictionary.Add(obj.name, obj.transform.position);

		if (tempSavedDeadMobDictionary.Count != 1) // If the Dictionary doesn't hold 1 item, then it's holding more and is not new.
		{
			// Thus we paste the uppdated temporary list over the list on the dictionary and thus update it.
			_savedDeadMobDictionary[_currentSeed] = tempSavedDeadMobDictionary;
		}
		else // if it's a new dictionary, then we need to add the seed and dictionary to the list as a new entry.
		{
			
			if (_savedDeadMobDictionary.ContainsKey(_currentSeed))
			{
				_savedDeadMobDictionary[_currentSeed] = tempSavedDeadMobDictionary;
			}
			else
			{
				_savedDeadMobDictionary.Add(_currentSeed, tempSavedDeadMobDictionary);
			}
		}
	}

	private bool CheckSavedDeadMobList(Transform trans, MobBehaviour mob)
	{
		// trans.TryGetComponent(out MobBehaviour mob);

		if (tempSavedDeadMobDictionary.Count > 0 && tempSavedDeadMobDictionary.ContainsKey(trans.name))
		{
			print($"{trans.name} exists in SavedDeadMobList");

			trans.position = tempSavedDeadMobDictionary[trans.name];
			trans.gameObject.SetActive(true);
			//trans.position = PositionCorrection(trans.position);
			mob.status = Status.Dead;
			mob.IsDepleted(true);
			return true;
		}
		else // Not on Blacklist - simply update the sprites to not look depleted
		{
			mob.IsDepleted(false);
			return false;
		}
	}

	public void RemoveButcheredFromDeadMobDictionary(GameObject obj)
	{
		if (!tempSavedDeadMobDictionary.ContainsKey(obj.name)) return;

		print($"{obj} was Butchered, and Removed from SavedDeadMobList");
		tempSavedDeadMobDictionary.Remove(obj.name);
		_savedDeadMobDictionary[_currentSeed] = tempSavedDeadMobDictionary;
	}

	private void CheckRarityTier()
	{
		int dominant = _seedGenerator.distanceFromHome.x >= _seedGenerator.distanceFromHome.y ? _seedGenerator.distanceFromHome.x : _seedGenerator.distanceFromHome.y;
		if (dominant > 1 && dominant <= 5)
		{
			mobRarityWeight = 30;
		}
		else if (dominant > 5 && dominant <= 10)
		{
			mobRarityWeight = 25;
		}
		else if (dominant > 10 && dominant <= 20)
		{
			mobRarityWeight = 15;
		}
		else if (dominant > 20)
		{
			mobRarityWeight = 0;
		}
		else
		{
			mobRarityWeight = 0;
			for (int i = _objectQuantitySetup.quantities.Length - uniqueMobObjects; i < _objectQuantitySetup.quantities.Length; i++)
			{
				mobRarityWeight += _objectQuantitySetup.quantities[i];
			}
		}
	}

	/// <summary>
	/// Use this to Generate a new Random Position on the Screen using the Camera.
	/// </summary>
	/// <returns>A random Vector3-position within the screen.</returns>
	private Vector3 GeneratePosition()
	{
		Vector3 generatedPosition = Vector3.zero;

		//Might want to make this a separate function, from here:
		_playerViewPortPos = _camera.WorldToViewportPoint(_player.transform.position);

		Vector2 tempMaxVector = new Vector2(1, 1);

		_mobViewPortPos = tempMaxVector - _playerViewPortPos;

		Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width * _mobViewPortPos.x, Screen.height * _mobViewPortPos.y));
		RaycastHit hit;

		int tries = 0;
		while (Physics.Raycast(ray, out hit, float.MaxValue, _forestObjects))
		{
			if (_mobViewPortPos.x > 0.5f)
			{
				_mobViewPortPos.x = UnityEngine.Random.Range(0.5f, 0.9f);
			}
			else
			{
				_mobViewPortPos.x = UnityEngine.Random.Range(0.1f, 0.5f);
			}

			if (_mobViewPortPos.y > 0.5f)
			{
				_mobViewPortPos.y = UnityEngine.Random.Range(0.5f, 0.9f);
			}
			else
			{
				_mobViewPortPos.y = UnityEngine.Random.Range(0.1f, 0.5f);
			}

			ray = _camera.ScreenPointToRay(new Vector3(Screen.width * _mobViewPortPos.x, Screen.height * _mobViewPortPos.y));

			tries++;
			if (tries > 9) break;
		}

		if (Physics.Raycast(ray, out hit, float.MaxValue, _ground))
		{
			generatedPosition = Vector3.Lerp(hit.point, _player.transform.position, 0.15f);
		}

		if (hit.collider is null)
		{
			throw new System.Exception($"{ray} did not hit");
		}
		return generatedPosition;
	}

	//public void SpawnTrappedMob(Transform parent)
	//{
	//	int randomObject = UnityEngine.Random.Range(0, MobObjectPool.Instance.mobObjectPool.Length);
	//	Transform newObject = MobObjectPool.Instance.mobObjectPool[randomObject];
	//	newObject.TryGetComponent(out MobBehaviour mob);
	//	mob.IsDepleted(true);
	//	newObject.parent = parent;
	//	newObject.position = Vector3.zero;
	//	newObject.gameObject.SetActive(true);
	//}

	private void OnDestroy()
	{
		_seedGenerator.SendSeed -= SpawnMob;
		//_player.ResourceGathered -= RemoveButcheredFromDeadMobDictionary;
	}
}

// public class Mob
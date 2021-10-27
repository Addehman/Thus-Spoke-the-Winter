﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class ForestController : MonoBehaviour
{
	private static ForestController _instance;
	public static ForestController Instance { get { return _instance; } }

	public event Action OnClearForest;

	[SerializeField] private Transform _forestParent;
	[SerializeField] private LayerMask _ground;
	[SerializeField] private PlayerController _player;
	[SerializeField] private SeedGenerator _seedGenerator;
	[SerializeField] private int _currentSeed;

	private Camera _camera;
	private Dictionary<int, List<string>> _blackListDictionary = new Dictionary<int, List<string>>();
	private	List<string> _tempBlacklist;
	

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

	public void SpawnForest(int seed)
	{
		_currentSeed = seed;

		_tempBlacklist = new List<string>();

        if (_blackListDictionary.ContainsKey(_currentSeed))
        {
			_blackListDictionary.TryGetValue(_currentSeed, out List<string> result);
			_tempBlacklist = result;
        }

		print($"Seed: {seed}");
		ClearForest();
		// Check if the incoming seed number here is "-1", this means it's the Home block and no forest should spawn.
		if (seed == -1) {
			print("Spawning Cabin");
			SceneController.Instance.LoadScene("CabinScene");
			return;
		}
		else if (SceneController.Instance.IsCurrentSceneName("CabinScene")) {
			SceneController.Instance.LoadScene("ForestScene");
		}

		List<int> randomNumbers = new List<int>();


		UnityEngine.Random.InitState(seed); // To be used to control the seed of the random forest, whether it should be random or not.

		int spawnCount = (int)UnityEngine.Random.Range(5, 50);
		print("Amount of new Objects: " + spawnCount);

		for (int i = 0; i < spawnCount; i++) {
			//This needs to check so that we don't random the same number twice in a row or something like that.
			int randomObject = (int)UnityEngine.Random.Range(0, ForestObjectPool.Instance.forestObjectPool.Length);

			while (randomNumbers.Contains(randomObject)) {
				randomObject = (randomObject + 1) % ForestObjectPool.Instance.forestObjectPool.Length;
			}

			randomNumbers.Add(randomObject);

			Transform newObject = ForestObjectPool.Instance.forestObjectPool[randomObject];

			// The X and Y positions ranging from min to max of what the camera displays,
			// with a padding to make sure that an object doesn't block the player entering a block of forest.
			float randomViewPortPosX = UnityEngine.Random.Range(0.1f, 0.9f);
			float randomViewPortPosY = UnityEngine.Random.Range(0.1f, 0.9f);

			//float randomViewPortPosX = UnityEngine.Random.Range(0f, 1f);
			//float randomViewPortPosY = UnityEngine.Random.Range(0f, 1f);

			float randomOffset = UnityEngine.Random.Range(-1f, 1f);

			//A way to make the seed control what the random name is going to be.
			int randomID1 = UnityEngine.Random.Range(0, 1000000);
			int randomID2 = UnityEngine.Random.Range(0, 1000000);

			Vector3 randomWorldPos = Vector3.zero;
			Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width * randomViewPortPosX, Screen.height * randomViewPortPosY));
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, _ground)) {
				randomWorldPos = hit.point;
			}

			if (hit.collider is null) {
				throw new System.Exception($"{ray} did not hit");
			}

			randomWorldPos.z += randomOffset;


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

        if (_blackListDictionary.ContainsKey(_currentSeed))
        {
			_blackListDictionary.Remove(_currentSeed);
        }

		_blackListDictionary.Add(_currentSeed, _tempBlacklist);
	}

	private void ClearForest()
	{
		foreach (Transform forestObject in _forestParent) {
			OnClearForest?.Invoke();
			forestObject.gameObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		_seedGenerator.SendSeed -= SpawnForest;
		_player.ResourceGathered -= SaveIDToBlacklist;
	}
}
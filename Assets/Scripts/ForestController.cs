using UnityEngine;
using System;
using System.Collections.Generic;

public class ForestController : MonoBehaviour
{
	private static ForestController _instance;
	public static ForestController Instance { get { return _instance; } }


	[SerializeField] private ScreenWrap screenWrap = null;
	[SerializeField] private Transform forestParent = null;
	[SerializeField] private LayerMask ground;
	[SerializeField] private PlayerController _player;

	public GameObject[] forestObjects = new GameObject[6];
	public event Action OnClearForest;

	private Camera _camera;
	private List<string> blacklist;


	private void Awake()
	{
		_camera = Camera.main;
		_player = FindObjectOfType<PlayerController>();

		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
	}

	private void Start()
	{
		//Change so that SeedGenerator listens to screenWrap and ForestController listens to SeedGenerator.
		//We need to send a seed from SeedGenerator to ForestController and the SpawnNewForest function.
		screenWrap.SpawnNewForest += SpawnNewForest;
		/*_player.ResourceGathered += SaveIDToBlacklist;*/

		if (forestParent.gameObject.activeSelf)
			SpawnNewForest();
	}

	public void SpawnNewForest(/*int seed*/)
	{
		ClearForest();

		//UnityEngine.Random.InitState(1); // To be used to control the seed of the random forest, whether it should be random or not.

		int spawnCount = (int)UnityEngine.Random.Range(5, 50);
		print("Amount of new Trees: " + spawnCount);

		for (int i = 0; i < spawnCount; i++) {
			int randomTree = (int)UnityEngine.Random.Range(0, forestObjects.Length);

			float randomViewPortPosX = UnityEngine.Random.Range(0f, 1f);
			float randomViewPortPosY = UnityEngine.Random.Range(0f, 1f);

			Vector3 randomWorldPos = Vector3.zero;
			Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width * randomViewPortPosX, Screen.height * randomViewPortPosY));
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, ground)) {
				randomWorldPos = hit.point;
			}

			if (hit.collider is null) {
				throw new System.Exception($"{ray} did not hit");
			}

			float randomOffset = UnityEngine.Random.Range(-1f, 1f);
			randomWorldPos.z += randomOffset;

			GameObject newTree = forestObjects[randomTree];
			GameObject spawn = Instantiate(newTree, randomWorldPos, newTree.transform.rotation, forestParent);

			//A way to make the seed control what the random name is going to be.
			int randomID1 = UnityEngine.Random.Range(0, 1000000);
			int randomID2 = UnityEngine.Random.Range(0, 1000000);

			spawn.name += $" - ID: {randomID1}{randomID2}"; // This is one possible way to create an ID for the spawned objects

			//Another way to give the spawned object a unique ID as name.
			//spawn.name = Guid.NewGuid().ToString();

			// Here we make sure that the spawned object is not in the air.
			Vector3 positionCorrection = spawn.transform.position;
			positionCorrection.y = 0f;
			spawn.transform.position = positionCorrection;
		}
	}

	/*private void SaveIDToBlacklist(GameObject obj)
    {
		blacklist.Add(obj.name);
    }*/

	private void ClearForest()
	{
		foreach (Transform tree in forestParent) {
			OnClearForest?.Invoke();
			Destroy(tree.gameObject);
		}
	}
}
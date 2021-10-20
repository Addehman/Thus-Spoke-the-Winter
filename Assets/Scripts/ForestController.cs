using UnityEngine;
using System;

public class ForestController : MonoBehaviour
{
	private static ForestController _instance;
	public static ForestController Instance { get { return _instance; } }


	[SerializeField] private ScreenWrap screenWrap = null;
	[SerializeField] private Transform forestParent = null;
	[SerializeField] private LayerMask ground;

	public GameObject[] forestObjects = new GameObject[6];
	public event Action OnClearForest;

	private Camera _camera;


	private void Awake()
	{
		_camera = Camera.main;

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

		if (forestParent.gameObject.activeSelf)
			SpawnNewForest();
	}

	public void SpawnNewForest(/*int seed*/)
	{
		ClearForest();

		//UnityEngine.Random.InitState(seed); // To be used to control the seed of the random forest, whether it should be random or not.

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
			//spawn.name = UnityEngine.Random.Range(0, 100000).ToString(); // This is one possible way to create an ID for the spawned objects

			// Here we make sure that the spawned object is not in the air.
			Vector3 positionCorrection = spawn.transform.position;
			positionCorrection.y = 0f;
			spawn.transform.position = positionCorrection;
		}
	}

	private void ClearForest()
	{
		foreach (Transform tree in forestParent) {
			OnClearForest?.Invoke();
			Destroy(tree.gameObject);
		}
	}
}
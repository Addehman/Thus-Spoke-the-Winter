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
		screenWrap.SpawnNewForest += SpawnNewForest;

		if (forestParent.gameObject.activeSelf)
			SpawnNewForest();
	}

	public void SpawnNewForest()
	{
		ClearForest();

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
			Instantiate(newTree, randomWorldPos, newTree.transform.rotation, forestParent);
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
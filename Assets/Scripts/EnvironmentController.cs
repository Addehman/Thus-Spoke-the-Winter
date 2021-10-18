using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
	private static EnvironmentController _instance;
	public static EnvironmentController Instance { get { return _instance; } }


	[SerializeField] ScreenWrap screenWrap = null;
	[SerializeField] Transform treeParent = null;
	[SerializeField] private Camera _camera;

	public LayerMask ground;

	public GameObject[] trees = new GameObject[6];

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

		if (treeParent.gameObject.activeSelf)
			SpawnNewForest();
	}

    public void SpawnNewForest()
	{
		ClearForest();

		int spawnCount = (int)Random.Range(5, 50);
		print("Amount of new Trees: " + spawnCount);

		for (int i = 0; i < spawnCount; i++)
		{
			int randomTree = (int)Random.Range(0, 6);

			float randomViewPortPosX = Random.Range(0f, 1f);
			float randomViewPortPosY = Random.Range(0f, 1f);

			Vector3 randomWorldPos = Vector3.zero;

			Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width * randomViewPortPosX, Screen.height * randomViewPortPosY));


			RaycastHit hit;

            if (Physics.Raycast(ray, out hit, ground))
            {
				randomWorldPos = hit.point;
            }

            if (hit.collider is null)
            {
				throw new System.Exception($"{ray} did not hit");
			}

			//Randomize this number to get a different offset for each tree if wanted.
			randomWorldPos.z -= 1f;

            GameObject newTree = trees[randomTree];
			Instantiate(newTree, randomWorldPos, newTree.transform.rotation, treeParent);
		}
	}

	private void ClearForest()
	{
		foreach (Transform tree in treeParent)
		{
			Destroy(tree.gameObject);
		}
	}
}
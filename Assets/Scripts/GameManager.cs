using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static GameManager _instance;
	public static GameManager Instance { get { return _instance; } }


	[SerializeField] PlayerController player = null;
	[SerializeField] Transform treeParent = null;

	public GameObject[] trees = new GameObject[6];

	public static float ScreenBorder_Left = -2.4f, ScreenBorder_Top = 1.35f, ScreenBorder_Right = 2.4f, ScreenBorder_Bottom = -1.35f;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
	}

	private void Start()
	{
		player.SpawnNewForest += SpawnNewForest;

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

			float posX = Random.Range(ScreenBorder_Left, ScreenBorder_Right);
			float posZ = Random.Range(ScreenBorder_Bottom, ScreenBorder_Top);

			Vector3 randomPosition = new Vector3(posX, 0f, posZ);
			GameObject newTree = trees[randomTree];
			Instantiate(newTree, randomPosition, Quaternion.identity, treeParent);

			// //Here I want it to set the order in the Sorting layer according to its Y position,
			// //but it seems to give them a random order..
			// Now this code works, but the order is not being assigned the "order" even though it explicitly is told to, 
			// maybe the compiler isn't catching up doing so..
			// order = (int)(posY * orderIncrease) * -1;
			// print(newTree + " " + "New Order: " + order);
			// // order = order * -1;
			// newTree.GetComponentInChildren<SpriteRenderer>().sortingOrder = order;
			// print(order + " Should be similar to " + posY);
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
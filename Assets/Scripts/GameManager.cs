using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] PlayerController player = null;
	[SerializeField] Transform treeParent = null;

	public GameObject[] trees = new GameObject[6];

	float leftBorder, topBorder, rightBorder, bottomBorder;

	void Start()
	{
		leftBorder = player.playerBorderLeft;
		topBorder = player.playerBorderTop;
		rightBorder = player.playerBorderRight;
		bottomBorder = player.playerBorderBottom;

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

			float posX = Random.Range(leftBorder, rightBorder);
			float posY = Random.Range(bottomBorder, topBorder);

			Vector3 randomPosition = new Vector3(posX, posY, 0);
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

	void ClearForest()
	{
		foreach (Transform tree in treeParent)
		{
			Destroy(tree.gameObject);
		}
	}
}
using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	private static Inventory _instance;
	public static Inventory Instance { get { return _instance; } }

	public event Action UpdateUI;

	public int currentInventory, inventoryMaxCapacity = 100;
	public int wood, food, blueberry, lingonberry, apple, mushroom, venison;

	[SerializeField] private PlayerController _player;
	[SerializeField] private StorageController _storageController;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
	}

	private void OnEnable()
	{
		_player.ResourceGathered += GatherResource;
		_storageController.UpdateUI += UpdateCurrentInventory;
	}

	void Start()
	{
		currentInventory = 0;
	}

	void GatherResource(GameObject obj)
	{
		print($"Food: {food}. Wood: {wood}");

		int gatheredWood = 0;
		int gatheredBlueberry = 0;
		int gatheredLingonberry = 0;
		int gatheredApple = 0;
		int gatheredMushroom = 0;
		int gatheredVenison = 0;


		if (currentInventory >= inventoryMaxCapacity)
		{
			print("Inventory is full");
			return;
		}

		var treeBehav = obj.GetComponent<TreeBehaviour>();

		var foodBehav = obj.GetComponent<FoodBehaviour>();

		if (treeBehav != null)
		{
			//These switch cases can be removed if we always add to "gatheredWood" as it is right now.
			//Keeping it for now if we want to add specific kinds of wood later on, for example; "gatheredPineWood" or "gatheredLeafTreeWood".
			switch (treeBehav.type)
			{
				case ResourceType.fruitTree:
					{
						gatheredWood = treeBehav.resourceAmount;
						break;
					}

				case ResourceType.leafTree:
					{
						gatheredWood = treeBehav.resourceAmount;
						break;
					}

				case ResourceType.pineTree:
					{
						gatheredWood = treeBehav.resourceAmount;
						break;
					}
				/*case ResourceType.newTree:
					{
						wood += newTreeAmount;
						break;
					}*/
			}
		}
		else if (foodBehav != null)
		{
			switch (foodBehav.type)
			{
				case ResourceType.blueberry:
					{
						gatheredBlueberry += foodBehav.resourceAmount;
						break;
					}

				case ResourceType.lingonberry:
					{
						gatheredLingonberry += foodBehav.resourceAmount;
						break;
					}

				case ResourceType.apple:
					{
						gatheredApple += foodBehav.resourceAmount;
						break;
					}
				case ResourceType.mushroom:
					{
						gatheredMushroom += foodBehav.resourceAmount;
						break;
					}
				/*case ResourceType.venison:
					{
						gatheredVenison += foodBehav.resourceAmount;
						break;
					}*/
			}
		}

		wood += gatheredWood;
		blueberry += gatheredBlueberry;
		lingonberry += gatheredLingonberry;
		apple += gatheredApple;
		mushroom += gatheredMushroom;
		venison += gatheredVenison;

		food = (blueberry + lingonberry + apple + mushroom + venison);


		InventoryCapacityFailsafe(gatheredWood, gatheredBlueberry, gatheredLingonberry, 
			gatheredApple, gatheredMushroom, gatheredVenison);


		UpdateCurrentInventory();

		print($"Food: {food}. Wood: {wood}");

		UpdateUI?.Invoke();
	}

	private void UpdateCurrentInventory()
	{
		currentInventory = (wood + food);
	}

	public void ClearFood()
    {
		food = blueberry = lingonberry = apple = mushroom = venison = 0;
	}

	public void ClearWood()
    {
		wood = 0;
    }

	/*void WhatResourceToAdd(GameObject obj)
	{
		//Try and implement and run this function in "GatherResource" to tidy up in "GatherResource".
	}*/


	void InventoryCapacityFailsafe(int gatheredWood, int gatheredBlueberry, int gatheredLingonberry, 
		int gatheredApple, int gatheredMushroom, int gatheredVenison)
	{
		if (inventoryMaxCapacity - (wood + food) < 0)
		{
			int correction = inventoryMaxCapacity - (wood + food);

			if (gatheredWood > 0)
			{
				//correction is a negative value, therefore we "add" the negative value to subtract.
				wood += correction;
			}
			else if (gatheredBlueberry > 0)
			{
				blueberry += correction;
			}
			else if (gatheredLingonberry > 0)
			{
				lingonberry += correction;
			}
			else if (gatheredApple > 0)
			{
				apple += correction;
			}
			else if (gatheredMushroom > 0)
			{
				mushroom += correction;
			}
			else if (gatheredVenison > 0)
			{
				venison += correction;
			}

			print("Inventory is full");
		}
	}

	private void OnDestroy() 
	{
		_player.ResourceGathered -= GatherResource;
	}
}
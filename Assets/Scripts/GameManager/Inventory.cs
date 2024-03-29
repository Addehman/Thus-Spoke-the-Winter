using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	private static Inventory _instance;
	public static Inventory Instance { get { return _instance; } }

	public event Action UpdateUI;

	public int inventoryMaxCapacity = 100;
	public int currentTotalInventory;
	[Space(10)]
	public int wood;
	public int food;
	[Space(10)]
	public int blueberry;
	public int lingonberry;
	public int apple;
	public int mushroom;
	public int venison;
	public int bunnyMeat;

	[SerializeField] private PlayerController _player;


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
		StorageController.Instance.UpdateUI += UpdateCurrentInventory;
	}

	void Start()
	{
		currentTotalInventory = 0;
	}

	void GatherResource(GameObject obj)
	{
		//print($"Food: {food}. Wood: {wood}");

		int gatheredWood = 0;
		int gatheredBlueberry = 0;
		int gatheredLingonberry = 0;
		int gatheredApple = 0;
		int gatheredMushroom = 0;
		int gatheredBunnyMeat = 0;


		if (currentTotalInventory >= inventoryMaxCapacity)
		{
			print("Inventory is full");
			return;
		}

		if (obj.TryGetComponent(out TreeBehaviour treeBehav))
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
		else if (obj.TryGetComponent(out FoodBehaviour foodBehav))
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
		else if (obj.TryGetComponent(out MobBehaviour mobBehav))
		{
			switch (mobBehav.type)
			{
				case ResourceType.bunny:
					gatheredBunnyMeat += mobBehav.resourceAmount;
					break;
				default:
					break;
			}
		}
		else if (obj.TryGetComponent(out TrapBehaviour trapBehav))
		{
			switch (trapBehav.type)
			{
				case ResourceType.bunny:
					gatheredBunnyMeat += trapBehav.resourceAmount;
					break;
				default:
					break;
			}
		}

		wood += gatheredWood;

		blueberry += gatheredBlueberry;
		lingonberry += gatheredLingonberry;
		apple += gatheredApple;
		mushroom += gatheredMushroom;
		bunnyMeat += gatheredBunnyMeat;

		food = blueberry + lingonberry + apple + mushroom + bunnyMeat;


		InventoryCapacityFailsafe(gatheredWood, gatheredBlueberry, gatheredLingonberry,
			gatheredApple, gatheredMushroom, gatheredBunnyMeat);


		UpdateCurrentInventory();

		print($"Food: {food}. Wood: {wood}");

		UpdateUI?.Invoke();
	}

	private void UpdateCurrentInventory()
	{
		currentTotalInventory = (wood + food);
	}

	public void ClearFood()
	{
		food = blueberry = lingonberry = apple = mushroom = bunnyMeat = 0;
	}

	public void ClearWood()
	{
		wood = 0;
	}

	/*void WhatResourceToAdd(GameObject obj)
	{
		//Try and implement and run this function in "GatherResource" to tidy up in "GatherResource".
	}*/


	private void InventoryCapacityFailsafe(int gatheredWood, int gatheredBlueberry, int gatheredLingonberry,
		int gatheredApple, int gatheredMushroom, int gatheredBunnyMeat)
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
			else if (gatheredBunnyMeat > 0)
			{
				bunnyMeat += correction;
			}

			food = blueberry + lingonberry + apple + mushroom + bunnyMeat;

			print("Inventory is full");
		}
	}

	private void OnDestroy()
	{
		_player.ResourceGathered -= GatherResource;
	}
}

using System;
using UnityEngine;

public class StorageController : MonoBehaviour
{
	private static StorageController _instance;
	public static StorageController Instance { get { return _instance; } }

	public event Action UpdateUI;

	[SerializeField] private Inventory _inventory;
	public StorageHandler woodStorageHandler, foodStorageHandler;
	public int woodStorage, foodStorage;



	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
	}

	public void InitHandler(StorageHandler handler)
	{
		if (handler.type == StorageType.Wood) {
			woodStorageHandler = handler;
			woodStorageHandler.IncomingWood += StoreWoodResource;
		}
		else if (handler.type == StorageType.Food) {
			foodStorageHandler = handler;
			foodStorageHandler.IncomingFood += StoreFoodResource;
		}
	}

	public void TerminateHandler(StorageHandler handler)
	{
		if (handler.type == StorageType.Wood) {
			woodStorageHandler.IncomingWood -= StoreWoodResource;
			woodStorageHandler = null;
		}
		else if (handler.type == StorageType.Food) {
			foodStorageHandler.IncomingFood -= StoreFoodResource;
			foodStorageHandler = null;
		}
	}

	private void StoreWoodResource()
	{
		woodStorage += _inventory.wood;
		_inventory.ClearWood();
		UpdateUI?.Invoke();
	}
	
	private void StoreFoodResource()
	{
		foodStorage += _inventory.food;
		_inventory.ClearFood();
		UpdateUI?.Invoke();
	}
}

public enum StorageType
{
	Wood, Food,
}

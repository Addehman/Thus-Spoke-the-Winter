using System;
using UnityEngine;

public class StorageHandler : MonoBehaviour, IInteractable
{
	public event Action IncomingWood, IncomingFood;

	public StorageType type = StorageType.Wood;


	private void Start()
	{
		StorageController.Instance.InitHandler(this);
	}

	public void OnInteract()
	{
		if (type == StorageType.Wood)
		{
			IncomingWood?.Invoke();
			UIManager.Instance.UpdateStorageStatusUI("Wood Stack", StorageController.Instance.woodStorage, type);
		}
		else
		{
			IncomingFood?.Invoke();
			UIManager.Instance.UpdateStorageStatusUI("Food Stash", StorageController.Instance.foodStorage, type);
		}
	}

	private void OnDestroy()
	{
		StorageController.Instance.TerminateHandler(this);
	}

	public void OnDestruction() { /*This won't be destructable.*/ }
}

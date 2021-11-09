using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodObjectPool : MonoBehaviour
{
	private static FoodObjectPool _instance;
	public static FoodObjectPool Instance { get { return _instance; } }

	public Transform[] foodObjectPool;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
	}

	public void AddFoodObjectsToArray(List<Transform> spawns)
	{
		foodObjectPool = spawns.ToArray();
	}
}

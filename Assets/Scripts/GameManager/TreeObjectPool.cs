using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObjectPool : MonoBehaviour
{
	private static TreeObjectPool _instance;
	public static TreeObjectPool Instance { get { return _instance; } }

	public Transform[] treeObjectPool;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
	}

	public void AddTreeObjectsToArray(List<Transform> spawns)
	{
		treeObjectPool = spawns.ToArray();
	}
}

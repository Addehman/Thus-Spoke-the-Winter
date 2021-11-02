using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestObjectPool : MonoBehaviour
{
	private static ForestObjectPool _instance;
	public static ForestObjectPool Instance { get { return _instance; } }

	public Transform[] forestObjectPool;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
	}

	public void AddForestObjectsToList(List<Transform> spawns)
	{
		forestObjectPool = spawns.ToArray();
	}
}

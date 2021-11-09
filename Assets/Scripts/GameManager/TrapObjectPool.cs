using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapObjectPool : MonoBehaviour
{
	private static TrapObjectPool _instance;
	public static TrapObjectPool Instance { get { return _instance; } }

	public Transform[] trapObjectPool;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
	}

	public void AddTrapObjectsToList(List<Transform> spawns)
	{
		trapObjectPool = spawns.ToArray();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobObjectPool : MonoBehaviour
{
	private static MobObjectPool _instance;
	public static MobObjectPool Instance { get { return _instance; } }

	public Transform[] mobObjectPool;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
	}

	public void AddMobObjectsToList(List<Transform> spawns)
	{
		mobObjectPool = spawns.ToArray();
	}
}

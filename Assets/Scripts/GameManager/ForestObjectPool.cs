using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestObjectPool : MonoBehaviour
{
	private static ForestObjectPool _instance;
	public static ForestObjectPool Instance { get { return _instance; } }

	public Transform[] forestObjectPool;

	private Transform _myTransform;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;

		_myTransform = transform;
	}

	public void AddForestObjectsToList()
	{
		int index = 0;
		foreach (Transform forestObject in _myTransform)
		{
			forestObjectPool[index] = forestObject;
			index++;
		}
	}
}

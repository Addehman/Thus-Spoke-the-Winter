using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinController : MonoBehaviour
{
	private static CabinController _instance;
	public static CabinController Instance { get { return _instance; } }

	[SerializeField] private SeedGenerator _seedGenerator;
	[SerializeField] private GameObject _cabinSceneGroup;


	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this);
		}
		else
		{
			_instance = this;
		}
	}

	private void Start()
	{
		_seedGenerator.SendSeed += ChangeScene;
	}

	private void ChangeScene(int seed)
	{
		if (seed == -1)
		{
			_cabinSceneGroup.SetActive(true);
		}
		else if (seed != -1 && _cabinSceneGroup.activeSelf)
		{
			_cabinSceneGroup.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		_seedGenerator.SendSeed -= ChangeScene;
	}
}

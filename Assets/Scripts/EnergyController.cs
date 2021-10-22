using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyController : MonoBehaviour
{
	private static EnergyController _instance;
	public static EnergyController Instance {get{ return _instance;}}

	[SerializeField] private int startEnergy = 1000;
	public int totalEnergy = 0;


	private void Awake() 
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;

	}

	private void Start() 
	{
		// Subscribe to events here
	}

	private void OnEnable() 
	{
		totalEnergy = startEnergy;	
	}

	public void LoseEnergy(int cost)
	{
		totalEnergy -= cost;
	}

	private void OnDestroy() 
	{
		// Unsubscribe to events here
	}
}

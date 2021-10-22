using System;
using UnityEngine;

public class EnergyController : MonoBehaviour
{
	private static EnergyController _instance;
	public static EnergyController Instance {get{ return _instance;}}

	[SerializeField] private int startEnergy = 1000;
	public event Action UpdateEnergyUI;
	public int totalEnergy = 0;

	private PlayerController _player;


	private void Awake() 
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
		
		_player = FindObjectOfType<PlayerController>();
	}

	private void Start() 
	{
		// Subscribe to events here
		_player.EnergyDrain += LoseEnergy;
	}

	private void OnEnable() 
	{
		totalEnergy = startEnergy;	
	}

	public void LoseEnergy(ResourceSize size)
	{
		int cost = 0;
		switch (size) {
			case ResourceSize.Small:
				// here we should drain a certain amount of energy according to the size of the object/task
				break;
			case ResourceSize.Medium:
				break;
			case ResourceSize.Large:
				break;
		}
		totalEnergy -= cost;
		UpdateEnergyUI?.Invoke();
	}

	private void OnDestroy() 
	{
		// Unsubscribe to events here
	}
}

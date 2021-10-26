using System;
using UnityEngine;

public class EnergyController : MonoBehaviour
{
	private static EnergyController _instance;
	public static EnergyController Instance {get{ return _instance;}}

	[SerializeField] private int smallEnergyCost = 1, mediumEnergyCost = 5, largeEnergyCost = 10;
	public int currentEnergy = 0, startEnergy = 1000;

	public event Action UpdateEnergyUI;

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
		currentEnergy = startEnergy;	
	}

	public void LoseEnergy(ResourceSize size)
	{
		int cost = 0;
		switch (size) {
		// here we should drain a certain amount of energy according to the size of the object/task
			case ResourceSize.Small:
				cost = smallEnergyCost;
				break;
			case ResourceSize.Medium:
				cost = mediumEnergyCost;
				break;
			case ResourceSize.Large:
				cost = largeEnergyCost;
				break;
		}
		print($"EnergyCost: {cost}");
		currentEnergy -= cost;
		UpdateEnergyUI?.Invoke();
	}

	private void OnDestroy() 
	{
		// Unsubscribe to events here
		_player.EnergyDrain -= LoseEnergy;
	}
}

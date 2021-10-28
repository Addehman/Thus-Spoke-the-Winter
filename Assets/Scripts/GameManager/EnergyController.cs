using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnergyController : MonoBehaviour
{
	private static EnergyController _instance;
	public static EnergyController Instance {get{ return _instance;}}

	[SerializeField] private PlayerController _player;
	[SerializeField] private SeedGenerator _seedGenerator;
	[SerializeField] private int smallEnergyCost = 10, mediumEnergyCost = 50, largeEnergyCost = 100;
	public int currentEnergy = 0, startEnergy = 1000;

	public event Action UpdateEnergyUI, EnergyDepleted;


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
		_player.EnergyDrain += LoseEnergy;
		_seedGenerator.DrainEnergy += LoseEnergy;
	}

	private void OnEnable() 
	{
		Rest();
	}

	public void LoseEnergy(EnergyCost size)
	{
		int cost = 0;
		switch (size) {
		// here we should drain a certain amount of energy according to the size of the object/task
			case EnergyCost.Small:
				cost = smallEnergyCost;
				break;
			case EnergyCost.Medium:
				cost = mediumEnergyCost;
				break;
			case EnergyCost.Large:
				cost = largeEnergyCost;
				break;
		}
		print($"EnergyCost: {cost}");
		currentEnergy -= cost;
		UpdateEnergyUI?.Invoke();

		if (currentEnergy <= 0) {
			EnergyDepleted?.Invoke();
		}
	}

	/// <summary>
	/// Set currentEnergy to max again.
	/// </summary>
	public void Rest()
	{
		currentEnergy = startEnergy;
		UpdateEnergyUI?.Invoke();
	}

	private void OnDestroy() 
	{
		// Unsubscribe to events here
		_player.EnergyDrain -= LoseEnergy;
	}

#if UNITY_EDITOR
	private void Update()
	{
		Keyboard kb = InputSystem.GetDevice<Keyboard>();
		if (kb.kKey.wasPressedThisFrame) LoseEnergy(EnergyCost.Large);
	}
#endif
}

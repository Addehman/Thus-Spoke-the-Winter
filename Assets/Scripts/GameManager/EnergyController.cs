using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnergyController : MonoBehaviour
{
	private static EnergyController _instance;
	public static EnergyController Instance { get { return _instance; } }

	[SerializeField] private PlayerController _player;
	[SerializeField] private SeedGenerator _seedGenerator;
	[SerializeField] private int smallEnergyCost = 10, mediumEnergyCost = 50, largeEnergyCost = 100, miniEnergyCost = 5;
	public int currentEnergy = 0, startEnergy = 1000;

	public event Action UpdateEnergyUI, EnergyDepleted, PlayerRestingEndingRound;


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
		currentEnergy = startEnergy;
	}

	public void LoseEnergy(EnergyCost size)
	{
		int cost = 0;
		switch (size)
		{
			// here we should drain a certain amount of energy according to the size of the object/task
			case EnergyCost.Mini:
				cost = miniEnergyCost;
				break;
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
		/*print($"EnergyCost: {cost}");*/
		currentEnergy -= cost;
		UpdateEnergyUI?.Invoke();

		if (currentEnergy <= 0)
		{
			EnergyDepleted?.Invoke();
		}
	}

	/// <summary>
	/// Set currentEnergy to max again.
	/// </summary>
	public void Rest()
	{
		if (currentEnergy > 0)
		{
			return;
		}

		//Resting. Lock player inputs.
		_player.lockInput = true;
		//_player.playerInput.DeactivateInput(); // This could probably only be used if we would only use the PlayerInput-component to connect the actions to the buttons.

		//Run sleep animation (Crossfade to/from black).
		//The animation executes an event in the end of the animation,
		//setting the _player.lockInput back to false again.
		UIManager.Instance.Crossfade();

		print($"{this}: Resting");
		PlayerRestingEndingRound?.Invoke();
	}

	public void RegainEnergy() // This function is now called from the Fade-animation "Both" on the Crossfade object.
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

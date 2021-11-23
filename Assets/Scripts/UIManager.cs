using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class UIManager : MonoBehaviour
{
	private static UIManager _instance;
	public static UIManager Instance { get { return _instance; } }

	[SerializeField] private TextMeshProUGUI _inventoryText, _foodWoodCountTxt, _trapCounterTxt, _currentSeedTxt, _worldPositionTxt;
	[SerializeField] private Image _energyBar;
	[SerializeField] private Animator _animator;
	[SerializeField] private PlayerController _player;
	[SerializeField] private GameObject _inventoryGfx, _touchControlsObjectGroup;
	//public bool isTouchActive = false;

	private bool _isInventoryActive = false, _isStatsActive = false;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
	}

	private void Start()
	{
		Inventory.Instance.UpdateUI += UpdateInventoryUI;
		EnergyController.Instance.UpdateEnergyUI += UpdateEnergyUI;
		StorageController.Instance.UpdateUI += UpdateInventoryUI;
		TrapController.Instance.UpdateTrapUI += UpdateTrapUI;
		ScreenWrap.Instance.PlayerTraveling += CloseInventory;
		SeedGenerator.Instance.SendSeed += UpdateUIStats;
		UpdateInventoryUI();
		CloseInventory();

		_currentSeedTxt.text = $"Current Seed: -1";
		_worldPositionTxt.text = $"World Position:\nX=5000 , Y=5000";
	}

	private void UnlockPlayerInput() // Being called from animation event
	{
		_player.UnlockInput();
	}

	private void RegainEnergy() // Being called from animation event
	{
		EnergyController.Instance.RegainEnergy();
	}

	public void Crossfade()
	{
		_animator.SetTrigger("BOTH");
	}

	public void FadeToBlack()
	{
		_animator.SetTrigger("START");
	}

	public void FadeFromBlack(Latitude obj)
	{
		_animator.SetTrigger("END");
	}

	private void UpdateInventoryUI()
	{
		_inventoryText.text = $"{Inventory.Instance.currentTotalInventory} / {Inventory.Instance.inventoryMaxCapacity}";
		_foodWoodCountTxt.text = $"{Inventory.Instance.food}\n{Inventory.Instance.wood}";
	}

	private void UpdateEnergyUI()
	{
		float currentEnergy = EnergyController.Instance.currentEnergy / (float)EnergyController.Instance.startEnergy;
		_energyBar.fillAmount = currentEnergy;
		//print($"Current Energy UI: {currentEnergy}");
		//print($"Current Energy: {EnergyController.Instance.currentEnergy}");
	}

	private void UpdateTrapUI(int trapCount)
	{
		_trapCounterTxt.text = $"x{trapCount}";
	}

	public void ToggleInventoryActive()
	{
		if (_player.lockInput) return;

		_isInventoryActive = !_isInventoryActive;
		_inventoryGfx.SetActive(_isInventoryActive);
	}

	private void CloseInventory(Latitude noUse = Latitude.North)
	{
		_isInventoryActive = false;
		_inventoryGfx.SetActive(_isInventoryActive);
	}

	public void TouchInputObjectGroupActivation(bool isActive)
	{
		_touchControlsObjectGroup.SetActive(isActive);
	}

	private void UpdateUIStats(int seed)
	{
		_currentSeedTxt.text = $"Current Seed: {seed}";
		_worldPositionTxt.text = $"World Position:\nX={SeedGenerator.Instance.position.x} , Y={SeedGenerator.Instance.position.y}";
	}

	public void ToggleStatsActive()
	{
		_isStatsActive = !_isStatsActive;
		_currentSeedTxt.gameObject.SetActive(_isStatsActive);
		_worldPositionTxt.gameObject.SetActive(_isStatsActive);
	}

	private void OnDestroy()
	{
		Inventory.Instance.UpdateUI -= UpdateInventoryUI;
		EnergyController.Instance.UpdateEnergyUI -= UpdateEnergyUI;
		StorageController.Instance.UpdateUI -= UpdateInventoryUI;
		TrapController.Instance.UpdateTrapUI -= UpdateTrapUI;
	}
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class UIManager : MonoBehaviour
{
	private static UIManager _instance;
	public static UIManager Instance { get { return _instance; } }

	[SerializeField] private TextMeshProUGUI _inventoryText, _foodWoodCountTxt, _trapCounterTxt;
	[SerializeField] private Image _energyBar;
	[SerializeField] private Animator _animator;
	[SerializeField] private PlayerController _player;
	[SerializeField] private GameObject _inventoryGfx;

	private bool isInventoryActive = false;


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
		UpdateInventoryUI();
		CloseInventory();
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
		isInventoryActive = !isInventoryActive;
		_inventoryGfx.SetActive(isInventoryActive);
	}

	private void CloseInventory(Latitude noUse = Latitude.North)
	{
		isInventoryActive = false;
		_inventoryGfx.SetActive(isInventoryActive);
	}

	private void OnDestroy()
	{
		Inventory.Instance.UpdateUI -= UpdateInventoryUI;
		EnergyController.Instance.UpdateEnergyUI -= UpdateEnergyUI;
		StorageController.Instance.UpdateUI -= UpdateInventoryUI;
		TrapController.Instance.UpdateTrapUI -= UpdateTrapUI;
	}
}

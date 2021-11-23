using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class UIManager : MonoBehaviour
{
	private static UIManager _instance;
	public static UIManager Instance { get { return _instance; } }

	[SerializeField] private TextMeshProUGUI _inventoryText, _foodWoodCountTxt, _trapCounterTxt, _currentSeedTxt, 
		_worldPositionTxt, _storageTypeTxt, _storageStatusNumbersTxt;
	[SerializeField] private Image _energyBar;
	[SerializeField] private Animator _animator;
	[SerializeField] private PlayerController _player;
	[SerializeField] private GameObject _inventoryGroup, _touchControlsObjectGroup, _statsGroup;
	[SerializeField] private RectTransform _storageStatusGroup;

	private bool _isInventoryActive = false, _isStatsActive = false;
	private Vector2 _woodStorageStatusUI = new Vector2(-70f, 80f), _foodStorageStatusUI = new Vector2(225f, 75f);


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

		_inventoryGroup.SetActive(false);
		_storageStatusGroup.gameObject.SetActive(false);
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
		_inventoryGroup.SetActive(_isInventoryActive);
	}

	private void CloseInventory(Latitude noUse = Latitude.North)
	{
		_isInventoryActive = false;
		_inventoryGroup.SetActive(_isInventoryActive);
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
		_statsGroup.SetActive(_isStatsActive);
	}

	public void StorageStatusUIActivation(bool active)
	{
		_storageStatusGroup.gameObject.SetActive(active);
	}

	public void UpdateStorageStatusUI(string storageTypeTitle, int currentAmount, StorageType type)
	{
		_storageTypeTxt.text = $"{storageTypeTitle}";
		_storageStatusNumbersTxt.text = $"{currentAmount}\n{StorageController.Instance.foodDayGoal}\n{StorageController.Instance.foodWinterGoal}";

		if (type == StorageType.Wood)
			_storageStatusGroup.localPosition = _woodStorageStatusUI;
		else
			_storageStatusGroup.localPosition = _foodStorageStatusUI;
	}

	private void OnDestroy()
	{
		Inventory.Instance.UpdateUI -= UpdateInventoryUI;
		EnergyController.Instance.UpdateEnergyUI -= UpdateEnergyUI;
		StorageController.Instance.UpdateUI -= UpdateInventoryUI;
		TrapController.Instance.UpdateTrapUI -= UpdateTrapUI;
	}
}

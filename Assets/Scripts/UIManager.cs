using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI inventoryText;
	[SerializeField] private Image energyBar;

	[SerializeField] private Inventory _inventory;
	[SerializeField] private StorageController _storageController;


	private void Start() 
	{
		_inventory.UpdateUI += UpdateInventoryUI;
		EnergyController.Instance.UpdateEnergyUI += UpdateEnergyUI;
		_storageController.UpdateUI += UpdateInventoryUI;
		UpdateInventoryUI();
	}

	private void UpdateInventoryUI()
	{
		inventoryText.text = $"{_inventory.currentInventory} / {_inventory.inventoryMaxCapacity}";
	}

	private void UpdateEnergyUI()
	{
		float currentEnergy = EnergyController.Instance.currentEnergy / (float)EnergyController.Instance.startEnergy; 
		energyBar.fillAmount = currentEnergy;
		print($"Current Energy UI: {currentEnergy}");
		print($"Current Energy: {EnergyController.Instance.currentEnergy}");
	}

	private void OnDestroy() 
	{
		_inventory.UpdateUI -= UpdateInventoryUI;
		EnergyController.Instance.UpdateEnergyUI -= UpdateEnergyUI;
	}
}

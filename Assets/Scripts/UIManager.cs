using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI inventoryText;
	[SerializeField] private Image energyBar;


	private void Start() 
	{
		Inventory.Instance.UpdateUI += UpdateInventoryUI;
		EnergyController.Instance.UpdateEnergyUI += UpdateEnergyUI;
		StorageController.Instance.UpdateUI += UpdateInventoryUI;
		UpdateInventoryUI();
	}

	private void UpdateInventoryUI()
	{
		inventoryText.text = $"{Inventory.Instance.currentInventory} / {Inventory.Instance.inventoryMaxCapacity}";
	}

	private void UpdateEnergyUI()
	{
		float currentEnergy = EnergyController.Instance.currentEnergy / (float)EnergyController.Instance.startEnergy; 
		energyBar.fillAmount = currentEnergy;
		//print($"Current Energy UI: {currentEnergy}");
		//print($"Current Energy: {EnergyController.Instance.currentEnergy}");
	}

	private void OnDestroy() 
	{
		Inventory.Instance.UpdateUI -= UpdateInventoryUI;
		EnergyController.Instance.UpdateEnergyUI -= UpdateEnergyUI;
	}
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI inventoryText;
	[SerializeField] private Image energyBar;

	private PlayerController _player;
	private Inventory _inventory;


	private void Awake() 
	{
		_player = FindObjectOfType<PlayerController>();
		_inventory = FindObjectOfType<Inventory>();
	}

	private void Start() 
	{
		_inventory.UpdateUI += UpdateInventoryUI;
		EnergyController.Instance.UpdateEnergyUI += UpdateEnergyUI;
	}

	private void UpdateInventoryUI()
	{
		inventoryText.text = $"{_inventory.currentInventory} / {_inventory.inventoryMaxCapacity}";
	}

	private void UpdateEnergyUI()
	{
		energyBar.fillAmount = EnergyController.Instance.totalEnergy;
	}

	private void OnDestroy() 
	{
		_inventory.UpdateUI -= UpdateInventoryUI;
		EnergyController.Instance.UpdateEnergyUI -= UpdateEnergyUI;
	}
}

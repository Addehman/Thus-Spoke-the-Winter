using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class UIManager : MonoBehaviour
{
	private static UIManager _instance;
	public static UIManager Instance { get { return _instance; } }

	[SerializeField] private TextMeshProUGUI _inventoryText;
	[SerializeField] private Image _energyBar;
	/*[SerializeField] private ScreenWrap _screenWrap;*/
	[SerializeField] private Animator _animator;


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
		//_screenWrap.PlayerTraveling += FadeFromBlack;
		UpdateInventoryUI();
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
		_inventoryText.text = $"{Inventory.Instance.currentInventory} / {Inventory.Instance.inventoryMaxCapacity}";
	}

	private void UpdateEnergyUI()
	{
		float currentEnergy = EnergyController.Instance.currentEnergy / (float)EnergyController.Instance.startEnergy;
		_energyBar.fillAmount = currentEnergy;
		//print($"Current Energy UI: {currentEnergy}");
		//print($"Current Energy: {EnergyController.Instance.currentEnergy}");
	}

	private void OnDestroy()
	{
		Inventory.Instance.UpdateUI -= UpdateInventoryUI;
		EnergyController.Instance.UpdateEnergyUI -= UpdateEnergyUI;
		StorageController.Instance.UpdateUI -= UpdateInventoryUI;
		/*_screenWrap.PlayerTraveling -= FadeFromBlack;*/
	}
}

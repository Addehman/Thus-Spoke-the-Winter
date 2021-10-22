using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
	public TextMeshProUGUI inventoryText;

	private PlayerController _player;

	private void Start() 
	{
		_player = FindObjectOfType<PlayerController>();
		_player.ResourceGathered += UpdateUI;	//			<-----// Optimal?
	}

	private void UpdateUI(GameObject obj)
	{
		inventoryText.text = $"{Inventory.Instance.currentInventory} / {Inventory.Instance.inventoryMaxCapacity}";
	}
}

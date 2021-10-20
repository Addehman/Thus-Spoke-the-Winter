using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI inventoryText;

    void Update()
    {
        inventoryText.text = $"{Inventory.Instance.currentInventory} / {Inventory.Instance.inventoryMaxCapacity}";
    }
}
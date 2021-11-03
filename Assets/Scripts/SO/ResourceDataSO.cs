using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Data", menuName = "ResourceData")]
public class ResourceDataSO : ScriptableObject
{
	public new string name;
	public Sprite resourceSprite;
	public ResourceType type;
	public int health;
	public int damage;
	public int resourceAmount;
	public EnergyCost energyCostSize;
}

public enum ResourceType {
	leafTree, fruitTree, pineTree, bush, blueberry, lingonberry, emptyLowBush, mushroom, apple, venison, 
}

public enum EnergyCost {
	Small, Medium, Large, Mini, 
}
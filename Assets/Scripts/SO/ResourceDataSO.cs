using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Data", menuName = "ResourceData")]
public class ResourceDataSO : ScriptableObject
{
	public new string name;
	public Sprite earlySpring_Sprite, lateSpring_Sprite, earlySummer_Sprite, lateSummer_Sprite,
		earlyFall_Sprite, lateFall_Sprite, winter_Sprite, depleted_Sprite;
	public ResourceType type;
	public int health;
	public int damage;
	public int resourceAmount;
	public EnergyCost energyCostSize;
	public Seasons beginSpawnPeriod, endSpawnPeriod;
}

public enum ResourceType {
	leafTree, fruitTree, pineTree, bush, blueberry, lingonberry, emptyLowBush, mushroom, apple, bunny, 
}

public enum EnergyCost {
	Small, Medium, Large, Mini, 
}

public enum Status: Byte {
	Alive, Dead,
} // max 8 enums i en Byte

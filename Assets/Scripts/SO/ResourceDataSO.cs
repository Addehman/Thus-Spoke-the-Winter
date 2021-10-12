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

}

public enum ResourceType {
	leafTree, fruitTree, pineTree, bush, blueberryBush, rock, 
}
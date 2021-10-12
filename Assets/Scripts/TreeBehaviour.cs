using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : MonoBehaviour
{
	[SerializeField] private ResourceDataSO _data;

	private SpriteRenderer _sr;
	private int orderIncrease = 100, newOrder, _health;
	private ResourceType _type;


	void Start()
	{
		_sr = GetComponent<SpriteRenderer>();

		_health = _data.health;
		_type = _data.type;

		UpdateOrder();
	}

	/// <summary>
	/// Use this to create the number to be used for the Sorting Layer order on the SpriteRenderer.
	/// </summary>
	private void UpdateOrder()
	{
		// Here we create the order number for the sprite to be ordered with,
		// thus we first cast it as an int, which happens after we increase it to not be too low,
		// then we reverse it, so that up on the screen is further away, thus towards negative.
		newOrder = (int)(transform.position.y * orderIncrease) * -1;
		_sr.sortingOrder = newOrder;
	}
}
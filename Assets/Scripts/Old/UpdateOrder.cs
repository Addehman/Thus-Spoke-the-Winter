using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateOrder : MonoBehaviour
{
	private SpriteRenderer spriteRend;
	private int orderIncrease = 100, newOrder = 0;

	private void Start()
	{
		spriteRend = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		// Here we create the order number for the sprite to be ordered with,
		// thus we first cast it as an int, which happens after we increase it to not be too low,
		// then we reverse it, so that up on the screen is further away, thus towards negative.
		newOrder = (int)(transform.position.y * orderIncrease) * -1;
		spriteRend.sortingOrder = newOrder;

		if (spriteRend.sortingOrder == newOrder)
			Destroy(this);
	}
}

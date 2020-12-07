using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateOrder : MonoBehaviour
{
	private int orderIncrease = 100, newOrder = 0;

	private SpriteRenderer spriteRend;

	private void Start() {
		spriteRend = GetComponentInChildren<SpriteRenderer>();
	}

	private void Update() {

		newOrder = (int)(transform.position.y * orderIncrease) * -1;
		spriteRend.sortingOrder = newOrder;

		if (spriteRend.sortingOrder == newOrder)
			Destroy(this);
	}
}

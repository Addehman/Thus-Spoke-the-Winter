using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfrontOrBehindChecker : MonoBehaviour
{
	[SerializeField] private SpriteRenderer _outsideCabinSpriteRenderer;


	private void OnTriggerEnter(Collider other)
	{
		// if player is behind cabin
		if (other.CompareTag("Player"))
		{
			_outsideCabinSpriteRenderer.sortingOrder = 1;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		// if player is infront of cabin
		if (other.CompareTag("Player"))
		{
			_outsideCabinSpriteRenderer.sortingOrder = -1;
		}
	}
}

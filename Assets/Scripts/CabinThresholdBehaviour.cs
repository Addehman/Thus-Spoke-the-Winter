using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinThresholdBehaviour : MonoBehaviour
{
	[SerializeField] private GameObject _player, _outsideCabin, _insideCabin;


	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == _player)
		{
			_outsideCabin.SetActive(false);
			_insideCabin.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == _player)
		{
			_outsideCabin.SetActive(true);
			_insideCabin.SetActive(false);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedBehaviour : MonoBehaviour, IInteractable
{


	public void OnInteract()
	{
		print("Resting");
		EnergyController.Instance.Rest();
	}

	public void OnDestruction() {}
}

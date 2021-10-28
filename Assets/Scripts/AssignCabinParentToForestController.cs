using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignCabinParentToForestController : MonoBehaviour
{
	//private ForestController _forestController;


	private void Start()
	{
		//_forestController = ForestController.Instance;
		ForestController.Instance.SetCabinParent(gameObject);
	}

	private void OnDisable()
	{
		ForestController.Instance.SetCabinParent(null);
	}
}

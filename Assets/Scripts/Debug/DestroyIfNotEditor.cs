using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfNotEditor : MonoBehaviour
{
	private void Awake()
	{
		if (!Application.isEditor)
			Destroy(gameObject);
	}
}

using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
	private void Awake()
	{
		GameObject[] objs = GameObject.FindGameObjectsWithTag("DontDestroy");
		if (objs.Length > 1)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);


		/*GameObject[] worldGroups = GameObject.FindGameObjectsWithTag("WorldGroup");

		if (worldGroups != null) {
			if (worldGroups.Length > 1) {
				Destroy(this.gameObject);
			}
			DontDestroyOnLoad(this.gameObject);
			return;
		}

		var camera = GetComponent<Camera>();

		if (camera != null) {
			Camera[] cameras = FindObjectsOfType<Camera>();

			if (cameras.Length > 1) {
				Destroy(this.gameObject);
			}
			DontDestroyOnLoad(this.gameObject);
			return;
		}


		*//*var player = GetComponent<PlayerController>();

        if (player != null)
        {
            PlayerController[] players = FindObjectsOfType<PlayerController>();

            if (players.Length > 1)
            {
                Destroy(this.gameObject);
            }
            DontDestroyOnLoad(this.gameObject);
            return;
        }

        GameObject[] forestParents = GameObject.FindGameObjectsWithTag("ForestParent");

        if (forestParents != null)
        {
            if (forestParents.Length > 1)
            {
                Destroy(this.gameObject);
            }
            DontDestroyOnLoad(this.gameObject);
            return;
        }*//*

		var forestController = GetComponent<ForestController>();

		if (forestController != null) {
			ForestController[] forestControllers = FindObjectsOfType<ForestController>();

			if (forestControllers.Length > 1) {
				Destroy(this.gameObject);
			}
			DontDestroyOnLoad(this.gameObject);
			return;
		}*/
	}
}

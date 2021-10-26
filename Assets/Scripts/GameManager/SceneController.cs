using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
	private static SceneController _instance;
	public static SceneController Instance { get { return _instance; } }


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
	}

	public void LoadScene(int index)
	{
		SceneManager.LoadScene(index);
	}

	public void LoadScene(string name)
	{
		SceneManager.LoadScene(name);
	}

	public bool IsCurrentSceneName(string name)
	{
		if (SceneManager.GetActiveScene().name == name)
			return true;
		else
			return false;
	}
}

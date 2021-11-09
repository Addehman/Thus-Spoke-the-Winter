using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
	private static SceneController _instance;
	public static SceneController Instance { get { return _instance; } }

	[SerializeField] private SeedGenerator _seedGenerator;

	private GameObject _cabinParent;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
	}

	private void Start()
	{
		_seedGenerator.SendSeed += ChangeScene;
	}

	private void ChangeScene(int seed)
	{
		if (seed == -1)
		{
			print("Spawning Cabin");
			LoadScene("CabinScene");
		}
		else if (IsCurrentSceneName("CabinScene"))
		{
			_cabinParent.SetActive(false);
			LoadScene("ForestScene");
		}
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

	public void SetCabinParent(GameObject obj)
	{
		_cabinParent = obj;
	}
}

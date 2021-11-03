using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Seasons { earlySpring, lateSpring, earlySummer, lateSummer, earlyFall, lateFall, winter, }

public class SeasonController : MonoBehaviour
{
	private static SeasonController _instance;
	public static SeasonController Instance { get { return _instance; } }

	[SerializeField] private Seasons currentSeason = Seasons.earlySpring;

	public event Action GameOver;
	public event Action<Seasons> UpdateSeason;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
	}

	private void Start()
	{
		EnergyController.Instance.EnergyRestored += IncrementSeason;
		UpdateSeason?.Invoke(currentSeason);
	}

	private void IncrementSeason()
	{
		if (currentSeason != Seasons.winter)
		{
			currentSeason++;
			UpdateSeason?.Invoke(currentSeason);
		}
		else
		{
			GameOver?.Invoke();
		}
	}

	private void OnDestroy()
	{
		EnergyController.Instance.EnergyRestored -= IncrementSeason;
	}
}


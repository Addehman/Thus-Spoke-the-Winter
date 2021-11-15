using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBehaviour : MonoBehaviour
{
	[SerializeField] private Material material;
	[SerializeField] private Color spring, summer, fall, winter;


	private void Awake()
	{
		UpdateGFX(SeasonController.Instance.currentSeason);
	}

	void Start()
	{
		/*print($"{this} was Started");*/
		SeasonController.Instance.UpdateSeason += UpdateGFX;
	}

	private void UpdateGFX(Seasons currentSeason)
	{
		switch (currentSeason)
		{
			case Seasons.earlySpring:
				material.color = spring;
				break;
			case Seasons.lateSpring:
				break;
			case Seasons.earlySummer:
				material.color = summer;
				break;
			case Seasons.lateSummer:
				break;
			case Seasons.earlyFall:
				material.color = fall;
				break;
			case Seasons.lateFall:
				break;
			case Seasons.winter:
				material.color = winter;
				break;
			default:
				break;
		}
	}

	private void OnDestroy()
	{
		SeasonController.Instance.UpdateSeason -= UpdateGFX;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossfadeBehaviour : MonoBehaviour
{
    [SerializeField] private Image _crossfadeImage;
    [SerializeField] private Color spring, summer, fall, winter;

    void Start()
    {
		UpdateColor(SeasonController.Instance.currentSeason);
		SeasonController.Instance.UpdateSeason += UpdateColor;
    }

    void UpdateColor(Seasons currentSeason)
    {
		switch (currentSeason)
		{
			case Seasons.earlySpring:
				_crossfadeImage.color = spring;
				break;
			case Seasons.lateSpring:
				break;
			case Seasons.earlySummer:
				_crossfadeImage.color = summer;
				break;
			case Seasons.lateSummer:
				break;
			case Seasons.earlyFall:
				_crossfadeImage.color = fall;
				break;
			case Seasons.lateFall:
				break;
			case Seasons.winter:
				_crossfadeImage.color = winter;
				break;
			default:
				break;
		}
	}
}

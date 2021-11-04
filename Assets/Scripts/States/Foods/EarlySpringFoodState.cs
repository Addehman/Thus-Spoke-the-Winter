using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarlySpringFoodState : IFoodState
{
	public IFoodState DoUpdateState(FoodBehaviour food, ResourceType type, Seasons season)
	{
		StateBehaviour();

		return ShouldChangeState(food, type, season);
	}

	public void StateBehaviour()
	{
		// Change sprite according to season
	}

	public IFoodState ShouldChangeState(FoodBehaviour food, ResourceType type, Seasons season)
	{
		if (food.health <= 0)
		{
			return food._depletedState;
		}
		else
		{
			return food._earlySpringState;
		}
	}
}

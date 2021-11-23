using System;
using UnityEngine;

[Serializable]
public class DayGoalValues
{
	public int woodDayGoal_earlySpring, woodDayGoal_lateSpring, woodDayGoal_earlySummer, woodDayGoal_lateSummer, woodDayGoal_earlyFall, woodDayGoal_lateFall/*,
		foodDayGoal_earlySpring, foodDayGoal_lateSpring, foodDayGoal_earlySummer, foodDayGoal_lateSummer, foodDayGoal_earlyFall, foodDayGoal_lateFall*/;
}

public class StorageController : MonoBehaviour
{
	private static StorageController _instance;
	public static StorageController Instance { get { return _instance; } }

	public event Action UpdateUI, GoalAccomplished, GameOver;

	[SerializeField] private Inventory _inventory;
	[Space(10)]
	[SerializeField] private DayGoalValues _dayGoalValues;
	[Space(10)]
	public int woodStorage;
	public int foodStorage;
	public int woodDayGoal, foodDayGoal, woodWinterGoal = 300, foodWinterGoal = 300;

	private StorageHandler woodStorageHandler, foodStorageHandler;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;
	}

	private void Start()
	{
		EnergyController.Instance.PlayerRestingEndingRound += IsGoalReached;
		SeasonController.Instance.UpdateSeason += SetGoalForSeason;
		SetGoalForSeason();
	}

	public void InitHandler(StorageHandler handler)
	{
		if (handler.type == StorageType.Wood) {
			woodStorageHandler = handler;
			woodStorageHandler.IncomingWood += StoreWoodResource;
		}
		else if (handler.type == StorageType.Food) {
			foodStorageHandler = handler;
			foodStorageHandler.IncomingFood += StoreFoodResource;
		}
	}

	public void TerminateHandler(StorageHandler handler)
	{
		if (handler.type == StorageType.Wood) {
			woodStorageHandler.IncomingWood -= StoreWoodResource;
			woodStorageHandler = null;
		}
		else if (handler.type == StorageType.Food) {
			foodStorageHandler.IncomingFood -= StoreFoodResource;
			foodStorageHandler = null;
		}
	}

	private void StoreWoodResource()
	{
		woodStorage += _inventory.wood;
		_inventory.ClearWood();
		UpdateUI?.Invoke();
	}
	
	private void StoreFoodResource()
	{
		foodStorage += _inventory.food;
		_inventory.ClearFood();
		UpdateUI?.Invoke();
	}

	private void SetGoalForSeason(Seasons currentSeason = Seasons.earlySpring)
	{
		// Here we will check what season is given to this function and set the goals accordingly.
		// OBS: we are currently and temporarily(maybe?) using the same value for both woodDayGoal and foodDayGoal. This is something that might be needing a change later on!
		switch (currentSeason)
		{
			case Seasons.earlySpring:
				woodDayGoal = foodDayGoal = _dayGoalValues.woodDayGoal_earlySpring;
				break;
			case Seasons.lateSpring:
				woodDayGoal = foodDayGoal = _dayGoalValues.woodDayGoal_lateSpring;
				break;
			case Seasons.earlySummer:
				woodDayGoal = foodDayGoal = _dayGoalValues.woodDayGoal_earlySummer;
				break;
			case Seasons.lateSummer:
				woodDayGoal = foodDayGoal = _dayGoalValues.woodDayGoal_lateSummer;
				break;
			case Seasons.earlyFall:
				woodDayGoal = foodDayGoal = _dayGoalValues.woodDayGoal_earlyFall;
				break;
			case Seasons.lateFall:
				woodDayGoal = foodDayGoal = _dayGoalValues.woodDayGoal_lateFall;
				break;
			case Seasons.winter:
				break;
			default:
				break;
		}
	}

	/// <summary>
	/// Here we check whether the goals are met, otherwise it's GameOver - the player simply won't survive.
	/// </summary>
	private void IsGoalReached()
	{
	// If it's not the Winter-season, we check whether the daily goals are reached and gives a response accordingly, and if the goals are met - we move onto the next chapter/day-round, but if not we end the game here.
		bool isWinter = SeasonController.Instance.currentSeason == Seasons.winter;
		if (!isWinter && woodStorage >= woodDayGoal && foodStorage >= foodDayGoal)
		{
			print($"Great Job! You survive until the next Seasonal Chapter!\nBe aware that you consume {woodDayGoal} of Wood,\nand {foodDayGoal} of Food to survive until then!");
			woodStorage -= woodDayGoal;
			foodStorage -= foodDayGoal;
			print($"However, this much remains in your Storage:\nWood: {woodStorage}\nFood: {foodStorage}.\n Beware that much is needed to survive the Winter:\nWood: {woodWinterGoal}\nFood: {foodWinterGoal}.\nBest of Luck to you.");
			GoalAccomplished?.Invoke();
			return;
		}
		else if (!isWinter)
		{
			if (woodStorage < woodDayGoal && foodStorage < foodDayGoal)
			{
				print($"GAME OVER.\nPlayer didn't have enough of any of the Resources and thusly couldn't survive..");
			}
			else if (woodStorage < woodDayGoal)
			{
				print($"GAME OVER.\nNot enough Wood to heat the cabin, thusly the Player got sick and passed away..");
			}
			else if (foodStorage < foodDayGoal)
			{
				print($"GAME OVER.\nNot enough Food, thusly the Player starved..");
			}

			GameOver?.Invoke();
			return;
		}
	// If it's the Winter-season, the if-statements above are skipped and we check instead if the EndGoal was reached and gives the player a response according to how well the player fared in gathering resources!
		if (woodStorage >= woodWinterGoal && foodStorage >= foodWinterGoal)
		{
			print($"Congratulations!\nYou managed to survive the Winter!");
			GameOver?.Invoke();
		}
		else
		{
			if (woodStorage < woodWinterGoal && foodStorage < foodWinterGoal)
			{
				print($"GAME OVER.\nPlayer didn't have enough of any of the Resources\nand thusly couldn't survive through the harsh Winter..");
			}
			else if (woodStorage < woodWinterGoal)
			{
				print($"GAME OVER.\nNot enough Wood to heat the cabin, thusly the Player froze to death..");
			}
			else if (foodStorage < foodWinterGoal)
			{
				print($"GAME OVER.\nNot enough Food, thusly the Player starved..");
			}

			GameOver?.Invoke();
		}
	}
#if UNITY_EDITOR
	private void Update()
	{
		UnityEngine.InputSystem.Keyboard kb = UnityEngine.InputSystem.InputSystem.GetDevice<UnityEngine.InputSystem.Keyboard>();
		if (kb.lKey.wasPressedThisFrame) woodStorage = foodStorage = woodDayGoal;
	}
#endif
}



public enum StorageType
{
	Wood, Food,
}

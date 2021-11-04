using UnityEngine;
using System;

public class FoodBehaviour : MonoBehaviour, IInteractable
{
	public ResourceDataSO data;

	public event Action<GameObject> OnDestruct;
	public ResourceType type;
	public EnergyCost size;
	public int resourceAmount, health;
	public EarlySpringFoodState _earlySpringState = new EarlySpringFoodState();
	public DepletedFoodState _depletedState = new DepletedFoodState();
	public SpriteRenderer sr;

	private GameObject _gameObject;
	private int _damage;

	//private IFoodState _currentState;'


	private void OnEnable()
	{
		if (type == ResourceType.blueberry || type == ResourceType.lingonberry)
			sr.sprite = data.earlySpring_Sprite;
	}

	private void Start()
	{
		_gameObject = gameObject;
		sr = GetComponent<SpriteRenderer>();

		type = data.type;
		size = data.energyCostSize;
		resourceAmount = data.resourceAmount;
		health = data.health;
		_damage = data.damage;
		sr.sprite = data.earlySpring_Sprite;

		//_currentState = _earlySpringState;

		//SeasonController.Instance.UpdateSeason += UpdateState;
	}

	public void OnInteract()
	{
		health -= _damage;

		if (health <= 0) OnDestruction();
	}

	public void OnDestruction()
	{
		print($"Gathered {_gameObject}");
		OnDestruct?.Invoke(_gameObject);

		if (type == ResourceType.apple || type == ResourceType.mushroom)
			_gameObject.SetActive(false);
		else
			sr.sprite = data.depleted_Sprite;
	}

	//private void UpdateState(Seasons season)
	//{
	//	if (type == ResourceType.apple || type == ResourceType.blueberry || type == ResourceType.lingonberry || type == ResourceType.mushroom) return;

	//	switch (season)
	//	{
	//		case Seasons.earlySpring:
	//			_sr.sprite = _data.earlySpring_Sprite;
	//			break;
	//		case Seasons.lateSpring:
	//			_sr.sprite = _data.lateSpring_Sprite;
	//			break;
	//		case Seasons.earlySummer:
	//			_sr.sprite = _data.earlySummer_Sprite;
	//			break;
	//		case Seasons.lateSummer:
	//			_sr.sprite = _data.lateSummer_Sprite;
	//			break;
	//		case Seasons.earlyFall:
	//			_sr.sprite = _data.earlyFall_Sprite;
	//			break;
	//		case Seasons.lateFall:
	//			_sr.sprite = _data.lateFall_Sprite;
	//			break;
	//		case Seasons.winter:
	//			_sr.sprite = _data.winter_Sprite;
	//			break;
	//		default:
	//			Debug.LogWarning($"The Season: {season} can't be found!");
	//			break;
	//	}
	//}

	//private void OnDestroy()
	//{
	//	SeasonController.Instance.UpdateSeason -= UpdateState;
	//}
}

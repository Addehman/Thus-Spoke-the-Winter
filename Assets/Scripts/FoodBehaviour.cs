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


	//private void OnEnable()
	//{
	//	if (type == ResourceType.blueberry || type == ResourceType.lingonberry)
	//		sr.sprite = data.earlySpring_Sprite;
	//}

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

	public void IsDepleted(bool isDepleted)
	{
		if (isDepleted)
			sr.sprite = data.depleted_Sprite;
		else
			sr.sprite = data.earlySpring_Sprite;
	}
}

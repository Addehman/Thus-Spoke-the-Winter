using UnityEngine;
using System;

public class FoodBehaviour : MonoBehaviour, IInteractable
{
	[SerializeField] private Status status = Status.Alive;

	public ResourceDataSO data;
	public event Action<GameObject> OnDestruct;
	public ResourceType type;
	public EnergyCost costSize;
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
		costSize = data.energyCostSize;
		resourceAmount = data.resourceAmount;
		health = data.health;
		_damage = data.damage;
		sr.sprite = data.earlySpring_Sprite;

		//_currentState = _earlySpringState;

		//SeasonController.Instance.UpdateSeason += UpdateState;
	}

	private void OnEnable()
	{
		health = data.health;
	}

	public void OnInteract()
	{
		if (health <= 0 || status == Status.Dead) return;

		health -= _damage;

		if (health <= 0) OnDestruction();
	}

	public void OnDestruction()
	{
		print($"Gathered {_gameObject}");
		OnDestruct?.Invoke(_gameObject);
		status = Status.Dead;

		if (type == ResourceType.apple || type == ResourceType.mushroom)
			_gameObject.SetActive(false);
		else
			sr.sprite = data.depleted_Sprite;
	}

	public void IsDepleted(bool isDepleted)
	{
		if (isDepleted)
		{
			sr.sprite = data.depleted_Sprite;
			status = Status.Dead;
		}
		else
		{
			sr.sprite = data.earlySpring_Sprite;
			status = Status.Alive;
		}
	}
}

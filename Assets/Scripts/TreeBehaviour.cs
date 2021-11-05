using UnityEngine;
using System;
using System.Collections.Generic;

public class TreeBehaviour : MonoBehaviour, IInteractable
{
	private enum State: Byte {Alive, Depleted,} // max 8 enums i en Byte
	[SerializeField] private State state = State.Alive;
	[SerializeField] private ResourceDataSO _data;

	public event Action<GameObject> OnDestruct;
	public ResourceType type;
	public EnergyCost size;
	public int resourceAmount;
	public List<FoodBehaviour> fruits = new List<FoodBehaviour>();

	private Transform _transform;
	private GameObject _gameObject;
	private SpriteRenderer _sr;
	private int _health, _damage;


	private void Awake()
	{
		_transform = transform;
		_gameObject = gameObject;
		_sr = GetComponent<SpriteRenderer>();

		type = _data.type;
		size = _data.energyCostSize;
		resourceAmount = _data.resourceAmount;
		_health = _data.health;
		_damage = _data.damage;
		_sr.sprite = _data.earlySpring_Sprite;
	}

	//private void Start()
	//{
	//	SeasonController.Instance.UpdateSeason += UpdateState;
	//}

	private void OnEnable()
	{
		_health = _data.health;
		UpdateState(SeasonController.Instance.currentSeason);
	}

	public void AddFruitsToList()
	{
		if (fruits.Count > 0) return;

		foreach (FoodBehaviour fruit in _transform.GetComponentsInChildren<FoodBehaviour>())
		{
			if (fruit.gameObject.activeSelf)
				fruits.Add(fruit);
		}
	}

	private void OnDisable()
	{
		fruits.Clear();
	}

	public void OnInteract()
	{
		if (state == State.Depleted) return;

		if (fruits.Count > 1)
		{
			int randomFruit = UnityEngine.Random.Range(0, fruits.Count);
			fruits[randomFruit].OnInteract();
			fruits.RemoveAt(randomFruit);
			return;
		}
		else if (fruits.Count == 1)
		{
			fruits[0].OnInteract();
			fruits.Clear();
			return;
		}

		_health -= _damage;
		print($"Remaining Health: {_health}");

		if (_health <= 0) OnDestruction();
	}

	public void OnDestruction()
	{
		print($"{_gameObject} is falling!");
		OnDestruct?.Invoke(_gameObject);
		//_gameObject.SetActive(false);
		_sr.sprite = _data.depleted_Sprite;
		state = State.Depleted;
	}

	public void UpdateState(Seasons season)
	{
		switch (season)
		{
			case Seasons.earlySpring:
				_sr.sprite = _data.earlySpring_Sprite;
				state = State.Alive;
				break;
			case Seasons.lateSpring:
				_sr.sprite = _data.lateSpring_Sprite;
				state = State.Alive;
				break;
			case Seasons.earlySummer:
				_sr.sprite = _data.earlySummer_Sprite;
				state = State.Alive;
				break;
			case Seasons.lateSummer:
				_sr.sprite = _data.lateSummer_Sprite;
				state = State.Alive;
				break;
			case Seasons.earlyFall:
				_sr.sprite = _data.earlyFall_Sprite;
				state = State.Alive;
				break;
			case Seasons.lateFall:
				_sr.sprite = _data.lateFall_Sprite;
				state = State.Alive;
				break;
			case Seasons.winter:
				_sr.sprite = _data.winter_Sprite;
				state = State.Alive;
				break;
			default:
				Debug.LogWarning($"The Season: {season} can't be found!");
				break;
		}
	}

	public void SetSpriteToDepleted()
	{
		_sr.sprite = _data.depleted_Sprite;
		state = State.Depleted;
	}

	//private void OnDestroy()
	//{
	//	SeasonController.Instance.UpdateSeason -= UpdateState;
	//}
}
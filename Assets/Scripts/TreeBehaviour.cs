using UnityEngine;
using System;
using System.Collections.Generic;

public class TreeBehaviour : MonoBehaviour, IInteractable
{
	[SerializeField] private ResourceDataSO _data;
	[SerializeField] private SphereCollider stumpCollider;
	[SerializeField] private CapsuleCollider standingCollider;

	public Status status = Status.Alive;
	public event Action<GameObject> OnDestruct;
	public ResourceType type;
	public Seasons beginSpawnPeriod, endSpawnPeriod;
	public EnergyCost costSize;
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
		costSize = _data.energyCostSize;
		resourceAmount = _data.resourceAmount;
		_health = _data.health;
		_damage = _data.damage;
		_sr.sprite = _data.earlySpring_Sprite;
		beginSpawnPeriod = _data.beginSpawnPeriod;
		endSpawnPeriod = _data.endSpawnPeriod;
	}

	//private void Start()
	//{
	//	SeasonController.Instance.UpdateSeason += UpdateState;
	//}

	private void OnEnable()
	{
		_health = _data.health;
		UpdateState(SeasonController.Instance.currentSeason);
		standingCollider.enabled = true;
		stumpCollider.enabled = false;
	}

	public void AddFruitsToList()
	{
		if (fruits.Count > 0) return;

		foreach (FoodBehaviour fruit in _transform.GetComponentsInChildren<FoodBehaviour>()) // This could probably be changed to simply look for "fruit in _transform", I am doing this somewhere else and it works fine, though possibly with another type.
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
		if (status == Status.Dead) return;

		if (fruits.Count > 1)
		{
			/*int randomFruit = UnityEngine.Random.Range(0, fruits.Count);
			fruits[randomFruit].OnInteract();
			fruits.RemoveAt(randomFruit);*/
			fruits[0].OnInteract();
			fruits.RemoveAt(0);
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
		SetTreeToDead();
	}

	public void UpdateState(Seasons season)
	{
		switch (season)
		{
			case Seasons.earlySpring:
				_sr.sprite = _data.earlySpring_Sprite;
				status = Status.Alive;
				break;
			case Seasons.lateSpring:
				_sr.sprite = _data.lateSpring_Sprite;
				status = Status.Alive;
				break;
			case Seasons.earlySummer:
				_sr.sprite = _data.earlySummer_Sprite;
				status = Status.Alive;
				break;
			case Seasons.lateSummer:
				_sr.sprite = _data.lateSummer_Sprite;
				status = Status.Alive;
				break;
			case Seasons.earlyFall:
				_sr.sprite = _data.earlyFall_Sprite;
				status = Status.Alive;
				break;
			case Seasons.lateFall:
				_sr.sprite = _data.lateFall_Sprite;
				status = Status.Alive;
				break;
			case Seasons.winter:
				_sr.sprite = _data.winter_Sprite;
				status = Status.Alive;
				break;
			default:
				Debug.LogWarning($"The Season: {season} can't be found!");
				break;
		}
	}

	public void SetTreeToDead()
	{
		_sr.sprite = _data.depleted_Sprite;
		status = Status.Dead;
		standingCollider.enabled = false;
		stumpCollider.enabled = true;

		if (type == ResourceType.fruitTree)
		{
			fruits.Clear();
			foreach(Transform item in _transform)
			{
				item.gameObject.SetActive(false);
			}
		}
	}

	//private void OnDestroy()
	//{
	//	SeasonController.Instance.UpdateSeason -= UpdateState;
	//}
}
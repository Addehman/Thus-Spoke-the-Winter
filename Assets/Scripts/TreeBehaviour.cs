using UnityEngine;
using System;
using System.Collections.Generic;

public class TreeBehaviour : MonoBehaviour, IInteractable
{
	[SerializeField] private ResourceDataSO _data;

	public event Action<GameObject> OnDestroy;
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
		_sr.sprite = _data.resourceSprite;

		if (type == ResourceType.fruitTree)
		{
			foreach (FoodBehaviour fruit in _transform.GetComponentsInChildren<FoodBehaviour>())
			{
				fruits.Add(fruit);
			}
		}
	}

	public void OnInteract()
	{
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
		OnDestroy?.Invoke(_gameObject);
		_gameObject.SetActive(false);
	}
}
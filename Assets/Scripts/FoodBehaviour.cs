using UnityEngine;
using System;

public class FoodBehaviour : MonoBehaviour, IInteractable
{
	[SerializeField] private ResourceDataSO _data;

	public event Action<GameObject> OnDestroy;
	public ResourceType type;
	public int resourceAmount;

	private GameObject _gameObject;
	private SpriteRenderer _sr;
	private int _health, _damage;


	private void Start()
	{
		_gameObject = gameObject;
		_sr = GetComponent<SpriteRenderer>();

		type = _data.type;
		resourceAmount = _data.resourceAmount;
		_health = _data.health;
		_damage = _data.damage;
		_sr.sprite = _data.resourceSprite;
	}

	public void OnInteract()
	{
		_health -= _damage;

		if (_health <= 0) OnDestruction();
	}

	public void OnDestruction()
	{
		print($"Gathered {_gameObject}");
		Destroy(_gameObject);
		OnDestroy?.Invoke(_gameObject);
	}
}

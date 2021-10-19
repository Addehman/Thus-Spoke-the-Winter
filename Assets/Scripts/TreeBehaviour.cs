using UnityEngine;
using System;

public class TreeBehaviour : MonoBehaviour, IInteractable
{
	[SerializeField] private ResourceDataSO _data;

	public event Action<GameObject> OnDestroy;

	public ResourceType type;
	public int resourceAmount;

	private SpriteRenderer _sr;
	private int _health, _damage;


	void Start()
	{
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
		print($"Remaining Health: {_health}");

		if (_health <= 0) OnDestruction();
	}

	public void OnDestruction()
	{
		print($"{gameObject} is falling!");
		Destroy(gameObject);
		OnDestroy?.Invoke(gameObject);
	}
}
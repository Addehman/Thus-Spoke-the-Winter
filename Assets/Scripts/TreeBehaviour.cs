using UnityEngine;
using System;

public class TreeBehaviour : MonoBehaviour, IInteractable
{
	[SerializeField] private ResourceDataSO _data;

	public event Action<GameObject> OnDestroy;

	private SpriteRenderer _sr;
	private int _health, _damage;
	private ResourceType _type;


	void Start()
	{
		_sr = GetComponent<SpriteRenderer>();

		_health = _data.health;
		_type = _data.type;
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
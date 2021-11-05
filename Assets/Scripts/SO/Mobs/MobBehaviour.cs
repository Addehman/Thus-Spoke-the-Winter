using System;
using UnityEngine;

public class MobBehaviour : MonoBehaviour, IInteractable
{
	[SerializeField] private ResourceDataSO _data;
	[SerializeField] private Status status = Status.Alive;

	public event Action<GameObject> OnButcher;

	public ResourceType type;
	public EnergyCost costSize;
	public int resourceAmount;

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
	}

	private void OnEnable() 
	{
		if (status == Status.Alive)
		{
			_sr.sprite = _data.earlySpring_Sprite;
		}
		else
		{
			_sr.sprite = _data.depleted_Sprite;
		}
	}

	// private void Update()
	// {
	// 	if (status == Status.Alive)
	// 	{
			
	// 	}
	// }

	public void OnInteract()
	{
		switch (status)
		{
			case Status.Dead:
				Butcher();
				break;
			default:
				OnDestruction();
				break;
		}
	}

	public void OnDestruction()
	{
		status = Status.Dead;
	}

	private void Butcher()
	{
		print($"{_gameObject} was butchered and harvested.");
		_gameObject.SetActive(false);
		OnButcher?.Invoke(_gameObject);
	}

	// private void Movement()
	// {

	// }

	// private IEnumerator MoveInDirection()
	// {
	// 	yield 
	// }
}

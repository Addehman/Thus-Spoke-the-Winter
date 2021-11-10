using System;
using System.Collections;
using UnityEngine;

public class MobBehaviour : MonoBehaviour, IInteractable
{
	[SerializeField] private ResourceDataSO _data;
	[SerializeField] private Status status = Status.Alive;
	[SerializeField] private Camera _camera;

	public event Action<GameObject> OnButcher;

	public ResourceType type;
	public EnergyCost costSize;
	public int resourceAmount;
	public float speed = 0.5f, duration = 3f, elapsedTime, runSpeed = 1f;

	private Transform _transform;
	private GameObject _gameObject;
	private SpriteRenderer _sr;
	private int _health, _damage;
	private Coroutine activeCoroutine;
	private Vector3 currentVelocityRef;
	

	private void Awake() 
	{
		_transform = transform;
		_gameObject = gameObject;
		_sr = GetComponent<SpriteRenderer>();
		_camera = Camera.main;

		type = _data.type;
		costSize = _data.energyCostSize;
		resourceAmount = _data.resourceAmount;
		_health = _data.health;
		_damage = _data.damage;
		_sr.sprite = _data.earlySpring_Sprite;
	}

	private void OnEnable() 
	{
		if (status == Status.Alive)
		{
			_sr.sprite = _data.earlySpring_Sprite;
			RandomBehaviour();
		}
		else
		{
			_sr.sprite = _data.depleted_Sprite;
		}
	}

	private void Update()
	{
		if (!_gameObject.activeSelf) return;

		Vector2 temp = _camera.WorldToViewportPoint(_transform.position);
		if (temp.x > 1f || temp.x < 0f || temp.y > 1f || temp.y < 0f)
		{
			_gameObject.SetActive(false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			StopAllCoroutines();
			StartCoroutine(Fleeing(other.transform.position));
		}
	}

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
		StopAllCoroutines();
		IsDepleted(true);
	}

	private void Butcher()
	{
		print($"{_gameObject} was butchered and harvested.");
		_gameObject.SetActive(false);
		OnButcher?.Invoke(_gameObject);
	}

	public void IsDepleted(bool isDepleted)
	{
		if (isDepleted)
			_sr.sprite = _data.depleted_Sprite;
		else
			_sr.sprite = _data.earlySpring_Sprite;
	}

	private void RandomBehaviour()
	{
		int randomBehaviour = UnityEngine.Random.Range(0, 3);

		switch (randomBehaviour)
		{
			case 0:
				float randomX = UnityEngine.Random.Range(_transform.position.x - 0.5f, _transform.position.x + 0.5f);
				float randomZ = UnityEngine.Random.Range(_transform.position.z - 0.5f, _transform.position.z + 0.5f);
				Vector3 moveToPosition = new Vector3(randomX, _transform.position.y, randomZ);
				StartCoroutine(Move(moveToPosition));
				break;
			case 1:
				StartCoroutine(Eating());
				break;
			default:
				StartCoroutine(Scouting());
				break;
		}
	}

	private IEnumerator Move(Vector3 newPosition)// Bunny moves towards a random but close position
	{
		//print($"Bunny: {_gameObject}, is Moving.");
		float step = speed * Time.deltaTime;

		while ((_transform.position - newPosition).sqrMagnitude > 0.1f)
		{
			_transform.position = Vector3.MoveTowards(_transform.position, newPosition, step);

			yield return null;
		}

		RandomBehaviour();
	}

	private IEnumerator Eating() // Bunny is eating 
	{
		//print($"Bunny: {_gameObject}, is Eating.");
		yield return new WaitForSeconds(1f);
		RandomBehaviour();
	}

	private IEnumerator Scouting()// Bunny stops to look around for dangers 
	{
		//print($"Bunny: {_gameObject}, is Scouting.");
		yield return new WaitForSeconds(1f);
		RandomBehaviour();
	}

	private IEnumerator Fleeing(Vector3 playerPos) // Bunny flees the scene due to being scared by something(player), consider them having a trigger than reacts to the player, this way we can easily set the size for how close the player needs to get to scare them away.
	{
		float runStep = runSpeed * Time.deltaTime;
		//print($"playerPos: {playerPos}\nfleePos: {fleePos}");
		while (_gameObject.activeSelf || status == Status.Alive)
		{
			_transform.position = Vector3.MoveTowards(_transform.position, playerPos, -runStep);
			yield return null;
		}
	}
}

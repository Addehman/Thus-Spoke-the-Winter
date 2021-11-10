using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
	[SerializeField] private Rigidbody _rb;
	[SerializeField] private LayerMask _ground;
	[SerializeField] private float disableTimeLimit = 3f;
	public Transform _arrowParent;
	public ScreenWrap screenWrap;

	private Transform _transform;
	private GameObject _gameObject;
	private Camera _camera;


	private void Awake()
	{
		_transform = transform;
		_gameObject = gameObject;
		_camera = Camera.main;
	}

	private void Update()
	{
		if (!_gameObject.activeSelf) return;

		Vector2 temp = _camera.WorldToViewportPoint(_transform.position); // Maybe this should be replaced with triggers around the screen instead to catch things leaving the screen? at least these smaller things and not player?
		if (temp.x > 1f || temp.x < 0f || temp.y > 1f || temp.y < 0f)
		{
			StopAllCoroutines();
			_rb.isKinematic = true;
			_gameObject.SetActive(false);
			_transform.parent = _arrowParent;
		}
	}

	private void OnEnable()
	{
		BowBehaviour.Instance.OnReleaseArrow += ReleasedArrow;
	}

	private void Start()
	{
		screenWrap.PlayerTraveling += OnScreenWrap;
	}

	private void ReleasedArrow(float strength, Vector3 playerPos, Vector2 mousePoint)
	{
		if (!_gameObject.activeSelf) return;

		BowBehaviour.Instance.OnReleaseArrow -= ReleasedArrow;

		_rb.isKinematic = false;
		_transform.parent = null;

		Ray ray = _camera.ScreenPointToRay(mousePoint);
		RaycastHit hit;
		Vector3 direction = Vector3.zero;

		if (Physics.Raycast(ray, out hit, float.MaxValue, _ground))
		{
			direction = hit.point - playerPos;
			direction.y = 0f;
		}
		//print($"arrow direction: {direction}");
		//_rb.AddForce(direction * strength, ForceMode.Impulse);
		StartCoroutine(MoveArrowRoutine(direction, strength));
	}

	private IEnumerator MoveArrowRoutine(Vector3 direction, float strength) // This doesn't shoot the arrow straight..
	{
		for (int i = 0; i < 1000000; i++)
		{
			_rb.velocity = direction * strength;
			yield return null;
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		_rb.velocity = Vector3.zero;
		StopAllCoroutines();
		if (other.transform.TryGetComponent(out MobBehaviour mob))
		{
			// Here it's possible to make the arrow stick to the object it hits, by letting other become the parent.
			// This should be used when hitting something that needs more than one hit to die.
			// In the case mentioned just above, then the arrow object shouldn't be disabled either. It would be a nice effect if the arrow was stuck to what it hit.
			// _transform.parent = other.transform; 
			mob.OnDestruction();
			_gameObject.SetActive(false);

			_rb.isKinematic = true;
			_transform.parent = _arrowParent;
		}
		else
		{
			StartCoroutine(TimeUntilDisabling());
			_transform.parent = _arrowParent;
		}
	}

	private IEnumerator TimeUntilDisabling()
	{
		float timeElapsed = 0f;
		while (timeElapsed < disableTimeLimit)
		{
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		_gameObject.SetActive(false);
		_rb.isKinematic = true;
	}

	private void OnScreenWrap(Latitude latitude)
	{
		StopAllCoroutines();
		_rb.isKinematic = true;
		_transform.parent = _arrowParent;
		_gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		BowBehaviour.Instance.OnReleaseArrow -= ReleasedArrow;
		screenWrap.PlayerTraveling -= OnScreenWrap;
	}
}

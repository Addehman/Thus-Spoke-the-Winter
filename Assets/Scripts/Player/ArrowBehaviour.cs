using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
	[SerializeField] private Rigidbody _rb;
	[SerializeField] private LayerMask _ground;

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
			_gameObject.SetActive(false);
		}
	}

	private void OnEnable()
	{
		BowBehaviour.Instance.OnReleaseArrow += ReleasedArrow;
	}

	private void ReleasedArrow(float strength, Vector3 playerPos, Vector2 mousePoint)
	{
		if (!_gameObject.activeSelf) return;

		BowBehaviour.Instance.OnReleaseArrow -= ReleasedArrow;

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
		if (other.transform.TryGetComponent(out MobBehaviour mob))
		{
			mob.OnDestruction();
			_gameObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		BowBehaviour.Instance.OnReleaseArrow -= ReleasedArrow;
	}
}

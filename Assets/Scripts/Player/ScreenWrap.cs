using System;
using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
	public event Action<string> PlayerTraveling;

	private Transform _transform;
	private Camera _cam;

	[SerializeField]
	private Vector3 playerViewPortPos;

	void Start()
	{
		_transform = transform;
		_cam = Camera.main;
	}

	void Update()
	{
		Wrap();
	}

	private void Wrap()
	{
		playerViewPortPos = _cam.WorldToViewportPoint(_transform.position);

		/*print(playerViewPortPos);*/

		if (playerViewPortPos.x > 1f) {
			playerViewPortPos.x = 0f;

			Vector3 newPos = _cam.ViewportToWorldPoint(playerViewPortPos);
			newPos.y = _transform.position.y;
			_transform.position = newPos;

			PlayerTraveling?.Invoke("east");
		}
		else if (playerViewPortPos.x < 0f) {
			playerViewPortPos.x = 1f;

			Vector3 newPos = _cam.ViewportToWorldPoint(playerViewPortPos);
			newPos.y = _transform.position.y;
			_transform.position = newPos;

			PlayerTraveling?.Invoke("west");
		}
		else if (playerViewPortPos.y > 1f) {
			playerViewPortPos.y = -1f;

			Vector3 newPos = _cam.ViewportToWorldPoint(playerViewPortPos);
			newPos.y = _transform.position.y;
			_transform.position = newPos;

			PlayerTraveling?.Invoke("north");
		}
		else if (playerViewPortPos.y < 0f) {
			playerViewPortPos.y = 2f;

			Vector3 newPos = _cam.ViewportToWorldPoint(playerViewPortPos);
			newPos.y = _transform.position.y;
			_transform.position = newPos;

			PlayerTraveling?.Invoke("south");
		}
	}
}

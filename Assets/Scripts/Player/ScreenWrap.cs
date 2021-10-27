using System;
using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
	public event Action<string> PlayerTraveling;

	private Transform _transform;
	private Camera _cam;

	[SerializeField]
	private Vector3 playerViewPortPos;
	[SerializeField] private bool hasEnergy = true;
	[SerializeField] private SeedGenerator _seedGenerator;


	void Start()
	{
		_transform = transform;
		_cam = Camera.main;

		EnergyController.Instance.OutOfEnergy += ToggleHasEnergyBool;
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
			if (!hasEnergy && _seedGenerator.worldGrid[_seedGenerator.position.x + 1, _seedGenerator.position.y] == 0) {
				playerViewPortPos.x = 1f;
				Vector3 notNewPos = _cam.ViewportToWorldPoint(playerViewPortPos);
				notNewPos.y = _transform.position.y;
				_transform.position = notNewPos;
				return;
			}
			playerViewPortPos.x = 0f;

			Vector3 newPos = _cam.ViewportToWorldPoint(playerViewPortPos);
			newPos.y = _transform.position.y;
			_transform.position = newPos;

			PlayerTraveling?.Invoke("east");
		}
		else if (playerViewPortPos.x < 0f) {
			if (!hasEnergy && _seedGenerator.worldGrid[_seedGenerator.position.x - 1, _seedGenerator.position.y] == 0) {
				playerViewPortPos.x = 0f;
				Vector3 notNewPos = _cam.ViewportToWorldPoint(playerViewPortPos);
				notNewPos.y = _transform.position.y;
				_transform.position = notNewPos;
				return;
			}
			playerViewPortPos.x = 1f;

			Vector3 newPos = _cam.ViewportToWorldPoint(playerViewPortPos);
			newPos.y = _transform.position.y;
			_transform.position = newPos;

			PlayerTraveling?.Invoke("west");
		}
		else if (playerViewPortPos.y > 1f) {
			if (!hasEnergy && _seedGenerator.worldGrid[_seedGenerator.position.x, _seedGenerator.position.y + 1] == 0) {
				playerViewPortPos.y = 1f;
				Vector3 notNewPos = _cam.ViewportToWorldPoint(playerViewPortPos);
				notNewPos.y = _transform.position.y;
				_transform.position = notNewPos;
				return;
			}
			playerViewPortPos.y = -1f;

			Vector3 newPos = _cam.ViewportToWorldPoint(playerViewPortPos);
			newPos.y = _transform.position.y;
			_transform.position = newPos;

			PlayerTraveling?.Invoke("north");
		}
		else if (playerViewPortPos.y < 0f) {
			if (!hasEnergy && _seedGenerator.worldGrid[_seedGenerator.position.x, _seedGenerator.position.y - 1] == 0) {
				playerViewPortPos.y = 0f;
				Vector3 notNewPos = _cam.ViewportToWorldPoint(playerViewPortPos);
				notNewPos.y = _transform.position.y;
				_transform.position = notNewPos;
				return;
			}
			playerViewPortPos.y = 2f;

			Vector3 newPos = _cam.ViewportToWorldPoint(playerViewPortPos);
			newPos.y = _transform.position.y;
			_transform.position = newPos;

			PlayerTraveling?.Invoke("south");
		}
	}

	private void ToggleHasEnergyBool()
	{
		hasEnergy = !hasEnergy;
	}
}

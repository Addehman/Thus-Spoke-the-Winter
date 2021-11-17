using System;
using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
	public event Action<Latitude> PlayerTraveling;
	[SerializeField] private float borderLeft = 0f, borderRight = 1f, borderTop = 0.95f, 
	borderBottom = -0.05f, extraStepVertical = 0.05f, extraStepHorizontal = 0.01f, entryTop = 2f, entryBottom = -1f;

	private Transform _transform;
	private Camera _cam;

	[SerializeField] private Vector3 _playerViewPortPos;
	[SerializeField] private bool _hasEnergy = true;
	[SerializeField] private SeedGenerator _seedGenerator;
	private bool _northIsNotExplored, _eastIsNotExplored, _southIsNotExplored, _westIsNotExplored;


	void Start()
	{
		_transform = transform;
		_cam = Camera.main;

		_seedGenerator.UpdateExploration += UpdateExploration;
		EnergyController.Instance.EnergyDepleted += SetHasEnergyFalse;
		StorageController.Instance.GoalAccomplished += SetHasEnergyTrue;
	}

	void Update()
	{
		Wrap();
	}

	void UpdateExploration(bool north, bool east, bool south, bool west)
	{
		_northIsNotExplored = north;
		_eastIsNotExplored = east;
		_southIsNotExplored = south;
		_westIsNotExplored = west;
	}

	private void Wrap()
	{
		_playerViewPortPos = _cam.WorldToViewportPoint(_transform.position);

		if (_playerViewPortPos.x > borderRight)
		{
			if (!_hasEnergy && _eastIsNotExplored)
			{
				ConvertPosFromViewPortToWorldPoint(false, true, borderRight, Latitude.East);
				return;
			}
			ConvertPosFromViewPortToWorldPoint(true, true, borderLeft + extraStepHorizontal, Latitude.East);
		}
		else if (_playerViewPortPos.x < borderLeft)
		{
			if (!_hasEnergy && _westIsNotExplored)
			{
				ConvertPosFromViewPortToWorldPoint(false, true, borderLeft, Latitude.West);
				return;
			}

			ConvertPosFromViewPortToWorldPoint(true, true, borderRight - extraStepHorizontal, Latitude.West);
		}
		else if (_playerViewPortPos.y > borderTop)
		{
			if (!_hasEnergy && _northIsNotExplored)
			{
				ConvertPosFromViewPortToWorldPoint(false, false, borderTop, Latitude.North);
				return;
			}
			ConvertPosFromViewPortToWorldPoint(true, false, -0.9f/* + extraStepVertical*/, Latitude.North);
		}
		else if (_playerViewPortPos.y < borderBottom)
		{
			if (!_hasEnergy && _southIsNotExplored)
			{
				ConvertPosFromViewPortToWorldPoint(false, false, borderBottom, Latitude.South);
				return;
			}

			ConvertPosFromViewPortToWorldPoint(true, false, 1.9f/* - extraStepVertical*/, Latitude.South);
		}
	}

	private void ConvertPosFromViewPortToWorldPoint(bool shouldMove, bool changeX, float value, Latitude latitude)
	{
		if (changeX) // horizontal screenwrapping
		{
			_playerViewPortPos.x = value;
		}
		else // vertical screenwrapping
		{
			_playerViewPortPos.y = value;
		}

		Vector3 newPos = _cam.ViewportToWorldPoint(_playerViewPortPos);
		newPos.y = _transform.position.y;
		_transform.position = newPos;

		if (!shouldMove) return;

		//Run FadeFromBlack animation.
		UIManager.Instance.FadeFromBlack(Latitude.North);

		PlayerTraveling?.Invoke(latitude);
	}

	private void SetHasEnergyFalse() => _hasEnergy = false;
	private void SetHasEnergyTrue() => _hasEnergy = true;

	private void OnDestroy()
	{
		_seedGenerator.UpdateExploration -= UpdateExploration;
		EnergyController.Instance.EnergyDepleted -= SetHasEnergyFalse;
		StorageController.Instance.GoalAccomplished -= SetHasEnergyTrue;
	}

}

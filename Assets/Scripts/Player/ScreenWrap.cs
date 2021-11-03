using System;
using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
	public event Action<Latitude> PlayerTraveling;

	private Transform _transform;
	private Camera _cam;

	[SerializeField]
	private Vector3 playerViewPortPos;
	[SerializeField] private bool _hasEnergy = true;
	[SerializeField] private SeedGenerator _seedGenerator;
	private bool _northIsNotExplored, _eastIsNotExplored, _southIsNotExplored, _westIsNotExplored;


	void Start()
	{
		_transform = transform;
		_cam = Camera.main;

		_seedGenerator.UpdateExploration += UpdateExploration;
		EnergyController.Instance.EnergyDepleted += SetHasEnergyFalse;
		EnergyController.Instance.PlayerRestingEndingRound += SetHasEnergyTrue;
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
		playerViewPortPos = _cam.WorldToViewportPoint(_transform.position);

		if (playerViewPortPos.x > 1f)
		{
			if (!_hasEnergy && _eastIsNotExplored)
			{
				ConvertPosFromViewPortToWorldPoint(false, true, 1f, Latitude.East);
				return;
			}
			ConvertPosFromViewPortToWorldPoint(true, true, 0f, Latitude.East);
		}
		else if (playerViewPortPos.x < 0f)
		{
			if (!_hasEnergy && _westIsNotExplored)
			{
				ConvertPosFromViewPortToWorldPoint(false, true, 0f, Latitude.West);
				return;
			}

			ConvertPosFromViewPortToWorldPoint(true, true, 1f, Latitude.West);
		}
		else if (playerViewPortPos.y > 1f)
		{
			if (!_hasEnergy && _northIsNotExplored)
			{
				ConvertPosFromViewPortToWorldPoint(false, false, 1f, Latitude.North);
				return;
			}
			ConvertPosFromViewPortToWorldPoint(true, false, -1f, Latitude.North);
		}
		else if (playerViewPortPos.y < 0f)
		{
			if (!_hasEnergy && _southIsNotExplored)
			{
				ConvertPosFromViewPortToWorldPoint(false, false, 0f, Latitude.South);
				return;
			}

			ConvertPosFromViewPortToWorldPoint(true, false, 2f, Latitude.South);
		}
	}

	private void ConvertPosFromViewPortToWorldPoint(bool shouldMove, bool changeX, float value, Latitude latitude)
	{
		if (changeX)
		{
			playerViewPortPos.x = value;
		}
		else
		{
			playerViewPortPos.y = value;
		}

		Vector3 newPos = _cam.ViewportToWorldPoint(playerViewPortPos);
		newPos.y = _transform.position.y;
		_transform.position = newPos;

		if (!shouldMove) return;

		PlayerTraveling?.Invoke(latitude);
	}

	private void SetHasEnergyFalse() => _hasEnergy = false;
	private void SetHasEnergyTrue() => _hasEnergy = true;

	private void OnDestroy()
	{
		_seedGenerator.UpdateExploration -= UpdateExploration;
		EnergyController.Instance.EnergyDepleted -= SetHasEnergyFalse;
		EnergyController.Instance.PlayerRestingEndingRound -= SetHasEnergyTrue;
	}

}

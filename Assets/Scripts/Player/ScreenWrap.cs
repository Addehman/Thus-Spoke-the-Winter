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

        EnergyController.Instance.OutOfEnergy += SetEnergyToFalse;
    }

    void Update()
    {
        Wrap();
    }

    private void Wrap()
    {
        playerViewPortPos = _cam.WorldToViewportPoint(_transform.position);

        if (playerViewPortPos.x > 1f)
        {
            if (!hasEnergy && _seedGenerator.worldGrid[_seedGenerator.position.x + 1, _seedGenerator.position.y] == 0)
            {
                ConvertPosFromViewPortToWorldPoint(false, true, 1f, "east");
                return;
            }
            ConvertPosFromViewPortToWorldPoint(true, true, 0f, "east");
        }
        else if (playerViewPortPos.x < 0f)
        {
            if (!hasEnergy && _seedGenerator.worldGrid[_seedGenerator.position.x - 1, _seedGenerator.position.y] == 0)
            {
                ConvertPosFromViewPortToWorldPoint(false, true, 0f, "west");
                return;
            }

            ConvertPosFromViewPortToWorldPoint(true, true, 1f, "west");
        }
        else if (playerViewPortPos.y > 1f)
        {
            if (!hasEnergy && _seedGenerator.worldGrid[_seedGenerator.position.x, _seedGenerator.position.y + 1] == 0)
            {
                ConvertPosFromViewPortToWorldPoint(false, false, 1f, "north");
                return;
            }
            ConvertPosFromViewPortToWorldPoint(true, false, -1f, "north");
        }
        else if (playerViewPortPos.y < 0f)
        {
            if (!hasEnergy && _seedGenerator.worldGrid[_seedGenerator.position.x, _seedGenerator.position.y - 1] == 0)
            {
                ConvertPosFromViewPortToWorldPoint(false, false, 0f, "south");
                return;
            }

            ConvertPosFromViewPortToWorldPoint(true, false, 2f, "south");
        }
    }

    private void ConvertPosFromViewPortToWorldPoint(bool shouldMove, bool changeX, float value, string latitude)
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

    private void SetEnergyToFalse()
    {
        hasEnergy = false;
    }

    private void OnDestroy()
    {
        EnergyController.Instance.OutOfEnergy -= SetEnergyToFalse;
    }
}

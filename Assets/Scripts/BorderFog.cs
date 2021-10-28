using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderFog : MonoBehaviour
{
    [SerializeField] private SeedGenerator _seedGenerator;
    [SerializeField] private GameObject _northFog, _eastFog, _southFog, _westFog;
    private bool _fogActive = false;


    void Start()
    {
        _seedGenerator.UpdateExploration += UpdateFog;
        EnergyController.Instance.EnergyDepleted += ActivateFog;
    }

    void ActivateFog()
    {
        _fogActive = true;
    }

    void UpdateFog(bool north, bool east, bool south, bool west)
    {
        if (_fogActive)
        {
            _northFog.SetActive(north);
            _eastFog.SetActive(east);
            _southFog.SetActive(south);
            _westFog.SetActive(west);
        }
    }

    private void OnDestroy()
    {
        _seedGenerator.UpdateExploration -= UpdateFog;
        EnergyController.Instance.EnergyDepleted -= ActivateFog;
    }
}

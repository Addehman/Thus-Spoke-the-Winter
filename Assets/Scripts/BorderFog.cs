using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderFog : MonoBehaviour
{
    [SerializeField] private SeedGenerator _seedGenerator;
    [SerializeField] private GameObject _northFog, _eastFog, _southFog, _westFog;
    private bool _fogActive = false;
    private bool _northActive = true, _eastActive = true, _southActive = true, _westActive = true;


    void Start()
    {
        _seedGenerator.UpdateExploration += UpdateFog;
        SpawnFog();
        /*EnergyController.Instance.EnergyDepleted += ActivateFog;*/
    }

    /*void ActivateFog()
    {
        _fogActive = true;

        SpawnFog();
    }*/

    void UpdateFog(bool north, bool east, bool south, bool west)
    {
        _northActive = north;
        _eastActive = east;
        _southActive = south;
        _westActive = west;

        /*if (_fogActive)
        {
            SpawnFog();
        }*/
        SpawnFog();
    }

    void SpawnFog()
    {
        _northFog.SetActive(_northActive);
        _eastFog.SetActive(_eastActive);
        _southFog.SetActive(_southActive);
        _westFog.SetActive(_westActive);
    }

    private void OnDestroy()
    {
        _seedGenerator.UpdateExploration -= UpdateFog;
        /*EnergyController.Instance.EnergyDepleted -= ActivateFog;*/
    }
}

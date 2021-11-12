using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrapObjectPoolQuantitySetup
{
    [HideInInspector] public int[] quantities;
    public int trap_Amount;
}

[Serializable]
public class TrapObjectPoolPrefabLibrary
{
    public GameObject[] prefabs = new GameObject[3];
}

public class TrapController : MonoBehaviour
{
    private static TrapController _instance;
    public static TrapController Instance { get { return _instance; } }

    [SerializeField] private PlayerController _player;
    [SerializeField] private SeedGenerator _seedGenerator;
    [SerializeField] private Transform _trapParent;
    [SerializeField] private TrapObjectPoolQuantitySetup _trapObjectPoolQuantitySetup;
    [SerializeField] private TrapObjectPoolPrefabLibrary _trapObjectPoolPrefabLibrary;

    private Dictionary<int, List<int>> _savedTrapsDict = new Dictionary<int, List<int>>();
    private List<int> _tempSavedTrapsList;
    private List<Transform> _tempSpawns;
    private List<float> _startTime;
    private List<float> _timeToCatch;
    private int _currentSeed;
    private int _trapIndex;

    public event Action<Vector3> OnSpawnSavedTraps;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this);
        else
            _instance = this;
    }

    void Start()
    {
        _seedGenerator.SendSeed += UpdateCurrentSeed;
        _player.OnPlaceTrap += PlaceTrap;

        InitializeObjectPool();

        _trapIndex = 0;
        _startTime = new List<float>();
        _timeToCatch = new List<float>();
    }

    //This triggers from "OnPlaceTrap" event in PlayerController that sends the players position.
    //The OnPlaceTrap event i PlayerController is triggered from keyboard input "E".
    private void PlaceTrap(Vector3 position) 
    {
        if (_trapIndex < _trapObjectPoolQuantitySetup.trap_Amount)
        {
            Transform newTrap = TrapObjectPool.Instance.trapObjectPool[_trapIndex];

            //OBS: Use PositionCorrection here if we lower the ground again
            newTrap.position = position /*PositionCorrection(position)*/;
            newTrap.gameObject.SetActive(true);
            SaveTrapToDictionary(_trapIndex);
            SetStartTime();
            SetTimeToCatch();
            _trapIndex++;
        }
    }

    //Need this to take into account which index we want to set the startTime for since we want to be able to pick up traps later on.

    private void SetStartTime()
    {
        _startTime.Add(Time.time);
    }

    private void SetTimeToCatch()
    {
        float randomTime = UnityEngine.Random.Range(60f, 300f);

        _timeToCatch.Add(10f); //Change this to randomTime when done testing.
    }

    private bool HasTrapCaughtAnimal(int trapIndex)
    {
        float elapsedTime = Time.time - _startTime[trapIndex];

        if (elapsedTime >= _timeToCatch[trapIndex])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ClearTraps()
    {
        for (int i = 0; i < TrapObjectPool.Instance.trapObjectPool.Length; i++)
        {
            TrapObjectPool.Instance.trapObjectPool[i].gameObject.SetActive(false);
        }
    }

    void SpawnSavedTraps()
    {
        if (_savedTrapsDict.TryGetValue(_currentSeed, out List<int> result))
        {
            for (int i = 0; i < result.Count; i++)
            {
                Transform newTrap = TrapObjectPool.Instance.trapObjectPool[result[i]];

                if (HasTrapCaughtAnimal(result[i]))
                {
                    newTrap.gameObject.name = "CAUGHT ANIMAL";
                    //ChangeSprite etc.
                }

                newTrap.gameObject.SetActive(true);
            }
        }
    }

    void UpdateCurrentSeed(int seed)
    {
        _currentSeed = seed;
        ClearTraps();
        SpawnSavedTraps();
    }

    void SaveTrapToDictionary(int index)
    {
        if (_savedTrapsDict.TryGetValue(_currentSeed, out List<int> result))
        {
            _tempSavedTrapsList = result;
            _tempSavedTrapsList.Add(index);
        }
        else
        {
            _tempSavedTrapsList = new List<int>();
            _tempSavedTrapsList.Add(index);
        }

        if (_tempSavedTrapsList.Count != 1)
        {
            _savedTrapsDict[_currentSeed] = _tempSavedTrapsList;
        }
        else
        {
            _savedTrapsDict.Add(_currentSeed, _tempSavedTrapsList);
        }
    }

    //OBS: Use this if we lower the ground again
    //Vector3 PositionCorrection(Vector3 position)
    //{
    //    position.y = 0;
    //    return position;
    //}

    /// <summary>
    /// Here we fill the ObjectPool up with managable accuracy concerning the amounts for each type - Spawns all the objects that will be possible to spawn on each block of forest.
    /// </summary>
    private void InitializeObjectPool()
    {
        _trapObjectPoolQuantitySetup.quantities = new int[1] { _trapObjectPoolQuantitySetup.trap_Amount };

        _tempSpawns = new List<Transform>();

        for (int i = 0; i < _trapObjectPoolQuantitySetup.quantities.Length; i++)
        {
            if (_trapObjectPoolQuantitySetup.quantities[i] <= 0) continue;

            InitializeThisTypeThisMany(_trapObjectPoolPrefabLibrary.prefabs[i], _trapObjectPoolQuantitySetup.quantities[i]);
        }
        TrapObjectPool.Instance.AddTrapObjectsToList(_tempSpawns);
    }

    private void InitializeThisTypeThisMany(GameObject type, int typeAmount)
    {
        GameObject spawn = null;
        for (int i = 0; i < typeAmount; i++)
        {
            spawn = Instantiate(type, _trapParent);
            spawn.SetActive(false);
            _tempSpawns.Add(spawn.transform);
        }
    }

    private void OnDestroy()
    {
        _seedGenerator.SendSeed -= UpdateCurrentSeed;
        _player.OnPlaceTrap -= PlaceTrap;
    }
}

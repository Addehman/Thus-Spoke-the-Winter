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

    private Dictionary<int, List<Vector3>> _savedTrapsDict = new Dictionary<int, List<Vector3>>();
    private List<Vector3> _tempSavedTrapsList;
    private int _currentSeed;

    private List<Transform> tempSpawns;

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

        InitializeObjectPool();
    }

    /// <summary>
	/// Here we fill the ObjectPool up with managable accuracy concerning the amounts for each type - Spawns all the objects that will be possible to spawn on each block of forest.
	/// </summary>
	private void InitializeObjectPool()
    {
        _trapObjectPoolQuantitySetup.quantities = new int[1] { _trapObjectPoolQuantitySetup.trap_Amount };

        tempSpawns = new List<Transform>();

        for (int i = 0; i < _trapObjectPoolQuantitySetup.quantities.Length; i++)
        {
            if (_trapObjectPoolQuantitySetup.quantities[i] <= 0) continue;

            InitializeThisTypeThisMany(_trapObjectPoolPrefabLibrary.prefabs[i], _trapObjectPoolQuantitySetup.quantities[i]);
        }
        TrapObjectPool.Instance.AddTrapObjectsToList(tempSpawns);
    }

    private void InitializeThisTypeThisMany(GameObject type, int typeAmount)
    {
        GameObject spawn = null;
        for (int i = 0; i < typeAmount; i++)
        {
            spawn = Instantiate(type, _trapParent);
            spawn.SetActive(false);
            tempSpawns.Add(spawn.transform);
        }
    }

    void UpdateCurrentSeed(int seed)
    {
        _currentSeed = seed;
        SpawnSavedTraps();
    }

    void SaveTrapToDictionary(Vector3 position) //This should trigger of an event from the player "OnPlacedTrap" or something like that.
    {
        if (_savedTrapsDict.TryGetValue(_currentSeed, out List<Vector3> result))
        {
            _tempSavedTrapsList = result;
            _tempSavedTrapsList.Add(position);
        }
        else
        {
            _tempSavedTrapsList = new List<Vector3>();
            _tempSavedTrapsList.Add(position);
        }


        //Double check that this works as intended.
        if (_tempSavedTrapsList.Count != 1)
        {
            _savedTrapsDict[_currentSeed] = _tempSavedTrapsList;
        }
        else
        {
            _savedTrapsDict.Add(_currentSeed, _tempSavedTrapsList);
        }
    }

    void SpawnSavedTraps()
    {
        if (_savedTrapsDict.TryGetValue(_currentSeed, out List < Vector3 > result))
        {
            foreach (Vector3 position in result)
            {
                OnSpawnSavedTraps?.Invoke(position);
            }
        }
    }
}

using UnityEngine;
using System;
using System.Collections.Generic;

public class ForestController : MonoBehaviour
{
    private static ForestController _instance;
    public static ForestController Instance { get { return _instance; } }

    public event Action OnClearForest;

    [SerializeField] private Transform forestParent;
    [SerializeField] private LayerMask ground;
    [SerializeField] private PlayerController _player;

    /*public GameObject[] forestObjects = new GameObject[6];*/
    private SeedGenerator _seedGenerator;
    private Camera _camera;
    private List<string> blacklist = new List<string>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this);
        else
            _instance = this;


        _camera = Camera.main;
        _player = FindObjectOfType<PlayerController>();
        _seedGenerator = FindObjectOfType<SeedGenerator>();
    }

    private void Start()
    {
        _seedGenerator.SendSeed += SpawnForest;
        _player.ResourceGathered += SaveIDToBlacklist;
    }

    public void SpawnForest(int seed)
    {
        List<int> randomNumbers = new List<int>();

        print($"Seed: {seed}");

        ClearForest();

        UnityEngine.Random.InitState(seed); // To be used to control the seed of the random forest, whether it should be random or not.

        int spawnCount = (int)UnityEngine.Random.Range(5, 50);
        print("Amount of new Objects: " + spawnCount);

        for (int i = 0; i < spawnCount; i++)
        {
            //This needs to check so that we don't random the same number twice in a row or something like that.
            int randomObject = (int)UnityEngine.Random.Range(0, ForestObjectPool.Instance.forestObjectPool.Length);

            while (randomNumbers.Contains(randomObject))
            {
                randomObject = (randomObject + 1) % ForestObjectPool.Instance.forestObjectPool.Length;
            }

            randomNumbers.Add(randomObject);

            Transform newObject = ForestObjectPool.Instance.forestObjectPool[randomObject];

            float randomViewPortPosX = UnityEngine.Random.Range(0f, 1f);
            float randomViewPortPosY = UnityEngine.Random.Range(0f, 1f);

            float randomOffset = UnityEngine.Random.Range(-1f, 1f);

            //A way to make the seed control what the random name is going to be.
            int randomID1 = UnityEngine.Random.Range(0, 1000000);
            int randomID2 = UnityEngine.Random.Range(0, 1000000);


            if (blacklist.Count > 0 && blacklist.Contains(newObject.name))
            {
                print($"{newObject.name} is blacklisted!");
                continue;
            }

            Vector3 randomWorldPos = Vector3.zero;
            Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width * randomViewPortPosX, Screen.height * randomViewPortPosY));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, ground))
            {
                randomWorldPos = hit.point;
            }

            if (hit.collider is null)
            {
                throw new System.Exception($"{ray} did not hit");
            }

            randomWorldPos.z += randomOffset;


            newObject.position = randomWorldPos;
            newObject.gameObject.SetActive(true);

            newObject.gameObject.name = $"{randomID1}{randomID2}";

            //Another way to give the spawned object a unique ID as name. Does not utilize seed though, so not for us right now.
            //newObject.name = Guid.NewGuid().ToString();

            // Here we make sure that the spawned object is not in the air.
            Vector3 positionCorrection = newObject.position;
            positionCorrection.y = 0f;
            newObject.position = positionCorrection;
        }
    }

    private void SaveIDToBlacklist(GameObject obj)
    {
        print($"{obj.name} is now blacklisted!");
        blacklist.Add(obj.name);
    }

    private void ClearForest()
    {
        foreach (Transform forestObject in forestParent)
        {
            OnClearForest?.Invoke();
            forestObject.gameObject.SetActive(false);
        }
    }
}
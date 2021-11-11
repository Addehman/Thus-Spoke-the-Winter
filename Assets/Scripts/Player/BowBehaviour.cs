using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using System;

public class BowBehaviour : MonoBehaviour
{
	private static BowBehaviour _instance;
	public static BowBehaviour Instance { get { return _instance; } }

	[SerializeField] private ScreenWrap _screenWrap;
	[SerializeField] private Transform[] _arrowPool;
	[SerializeField] private GameObject _arrowPrefab;
	[SerializeField] private Transform _arrowParent;
	[SerializeField] private int arrowAmount = 10;
	[SerializeField] private float _chargeSpeed = 0.1f;
	[SerializeField] private float _chargeMax = 3f;
	[SerializeField] private float _chargeMin = 1f;
	[SerializeField] private int _arrowIndex;

	public event Action<float, Vector3, Vector2> OnReleaseArrow;
	public float arrowStrength;

	private Transform _transform;
	private InputMaster _controls;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;

		_transform = transform;
		_controls = new InputMaster();

		_arrowPool = new Transform[arrowAmount];
		GameObject arrowSpawn = null;
		for (int i = 0; i < arrowAmount; i++)
		{
			arrowSpawn = Instantiate(_arrowPrefab, _arrowParent);
			_arrowPool[i] = arrowSpawn.transform;
			arrowSpawn.TryGetComponent(out ArrowBehaviour arrow);
			arrow._arrowParent = _arrowParent;
			arrow.screenWrap = _screenWrap;
			arrowSpawn.SetActive(false);
		}

		_arrowIndex = 0;
	}

	private void Start()
	{
		//_controls.Player.Bow.started += ctx => ChargeArrow();
		//_controls.Player.Bow.canceled += ctx => ReleaseArrow();
	}

	public void ChargeArrow() //Being run from PlayerController
	{
		if (_arrowPool[_arrowIndex].gameObject.activeSelf) return;

		_arrowPool[_arrowIndex].position = _arrowParent.position;
		_arrowPool[_arrowIndex].gameObject.SetActive(true);
		StartCoroutine(ArrowChargeRoutine());
		// Insert somewhere here that the position should be stuck to player, if itsn't already due to now being a child to player
	}

	public void ReleaseArrow(Vector2 mousePos)  //Being run from PlayerController
	{
		StopAllCoroutines();

		if (arrowStrength < _chargeMin) // Check if released too early, then return arrow to pool.
		{
			_arrowPool[_arrowIndex].gameObject.SetActive(false);
			return;
		}
		
		OnReleaseArrow?.Invoke(arrowStrength, _arrowParent.position, mousePos);

		if (_arrowIndex == _arrowPool.Length - 1)
			_arrowIndex = 0;
		else
			_arrowIndex++;
	}

	private IEnumerator ArrowChargeRoutine()
	{
		for (float i = 0; i < _chargeMax; i += _chargeSpeed * Time.deltaTime)
		{
			arrowStrength = i;
			yield return null;
		}
	}
}

using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using System;

public class BowBehaviour : MonoBehaviour
{
	private static BowBehaviour _instance;
	public static BowBehaviour Instance { get { return _instance; } }

	[SerializeField] private PlayerController _player;
	[SerializeField] private Camera _camera;
	[SerializeField] private LayerMask _ground;
	[SerializeField] private ScreenWrap _screenWrap;
	[SerializeField] private Transform[] _arrowPool;
	[SerializeField] private GameObject _arrowPrefab;
	[SerializeField] private Transform _arrowParent;
	[SerializeField] private int arrowAmount = 10;
	[SerializeField] private float _chargeSpeed = 0.1f;
	[SerializeField] private float _chargeMax = 3f;
	[SerializeField] private float _chargeMin = 1f;
	[SerializeField] private int _arrowIndex;
	[SerializeField] private Vector3 _arrowDirection;
	[SerializeField] private bool isHoldingArrow = false;
	[SerializeField] private bool hasInversedDirection = false;

	public event Action<float, Vector3, Vector3> OnReleaseArrow;
	public float arrowStrength;

	//private Transform _transform;
	private InputMaster _controls;


	private void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(this);
		else
			_instance = this;

		//_transform = transform;
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

	private void Update()
	{
		if (isHoldingArrow)
		{
			Ray ray = _camera.ScreenPointToRay(_player.mousePoint);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, float.MaxValue, _ground))
			{
				_arrowDirection = hit.point - _arrowParent.position;
				_arrowDirection.z -= _arrowParent.position.y;

				if (!hasInversedDirection)
				{
					hasInversedDirection = false;
					_arrowDirection = _arrowPool[_arrowIndex].InverseTransformDirection(hit.point);
				}

				_arrowPool[_arrowIndex].up = _arrowDirection;
			}
		}
	}

	public void ChargeArrow() //Being called from PlayerController
	{
		if (_arrowPool[_arrowIndex].gameObject.activeSelf) return;

		_arrowPool[_arrowIndex].position = _arrowParent.position;
		_arrowPool[_arrowIndex].gameObject.SetActive(true);
		hasInversedDirection = true;
		isHoldingArrow = true;
		StartCoroutine(ArrowChargeRoutine());
		// Insert somewhere here that the position should be stuck to player, if itsn't already due to now being a child to player
	}

	public void ReleaseArrow(Vector2 mousePos)  //Being called from PlayerController
	{
		StopAllCoroutines();
		isHoldingArrow = false;

		if (arrowStrength < _chargeMin) // Check if released too early, then return arrow to pool.
		{
			_arrowPool[_arrowIndex].gameObject.SetActive(false);
			return;
		}
		
		OnReleaseArrow?.Invoke(arrowStrength, _arrowParent.position, _arrowDirection);

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

﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;


public enum ControlSchemes
{
	KeyboardAndMouse, Gamepad, Touch,
}
public class PlayerController : MonoBehaviour
{
	[SerializeField] private float _walkSpeed = 20f, _sprintSpeed = 40f, aimParentYOffset = 0.1f;
	[SerializeField] private List<GameObject> _interactablesInRange;
	[SerializeField] private bool _hasEnergy = true;
	[SerializeField] private bool _doSprint, _arrowCanceled = false;
	[SerializeField] private RectTransform _moveDigitalJoystick;
	[SerializeField] private Vector2 _movement;

	public Transform aimParent, aimChild;
	public event Action<GameObject> ResourceGathered;
	public event Action<EnergyCost> EnergyDrain;
	public event Action<Vector3> OnPlaceTrap;
	public Vector2 aimPoint;
	public bool lockInput = false;
	public PlayerInput playerInput;
	public ControlSchemes currentControlScheme;

	private InputMaster _controls;
	private Transform _transform;
	private Animator _animator;
	//private SpriteRenderer _spriteRenderer;
	private Rigidbody _rb;
	private float _horizontal, _vertical;
	//private InputActionReference actionReference; // use later for hold interactions in touch


	private void Awake()
	{
		_transform = transform;
		_rb = GetComponent<Rigidbody>();
		//_spriteRenderer = GetComponent<SpriteRenderer>();
		_animator = GetComponent<Animator>();

		_controls = new InputMaster();
		_controls.Player.Movement.ReadValue<Vector2>();
		_controls.Player.MousePoint.ReadValue<Vector2>();
		_controls.Player.AimPoint.ReadValue<Vector2>();
	}

	private void OnEnable()
	{
/*		InputSystem.onDeviceChange += (device, change) => // this is for checking whether a new device is plugged in or out, or etc. however, as it seems, not if there simply is any change between control schemes.
		{
			switch (change)
			{
				case InputDeviceChange.Added:
					break;
				case InputDeviceChange.Removed:
					break;
				case InputDeviceChange.Disconnected:
					break;
				case InputDeviceChange.Reconnected:
					break;
				case InputDeviceChange.Enabled:
					break;
				case InputDeviceChange.Disabled:
					break;
				case InputDeviceChange.UsageChanged:
					break;
				case InputDeviceChange.ConfigurationChanged:
					break;
				case InputDeviceChange.Destroyed:
					break;
				default:
					break;
			}
		};*/
		_controls.Enable();
		_doSprint = false;
		_controls.Player.Sprint.started += ctx => _doSprint = true;
		_controls.Player.Sprint.canceled += ctx => _doSprint = false;
		_controls.Player.Interact.started += ctx => Interact();
		_controls.Player.PlaceTrap.started += ctx => PlaceTrap();
		_controls.Player.ShootArrow.started += ctx => ChargeArrow();
		_controls.Player.ShootArrow.canceled += ctx => ReleaseArrow(true);
		_controls.Player.CancelArrow.started += ctx => ReleaseArrow(false);
		_controls.Player.CancelArrow.canceled += ctx => ReleaseArrow(false);
		_controls.Player.Inventory.started += ctx => UIManager.Instance.ToggleInventoryActive();
		_controls.Player.ToggleStats.started += ctx => UIManager.Instance.ToggleStatsActive();

		TreeController.Instance.OnClearTrees += ClearInteractablesInRangeList;
		FoodController.Instance.OnClearFoods += ClearInteractablesInRangeList;
		MobController.Instance.OnClearMobs += ClearInteractablesInRangeList;
		EnergyController.Instance.EnergyDepleted += SetHasEnergyFalse;
		StorageController.Instance.GoalAccomplished += SetHasEnergyTrue;

		playerInput.onControlsChanged += OnControlsChanged;

		// Android Long touch / Hold
		//actionReference.action.Enable();
		//actionReference.action.started += ctx =>
		//{
		//	if (ctx.interaction is HoldInteraction)
		//	{
		//		ChargeArrow();
		//	}
		//};
		//actionReference.action.canceled += ctx =>
		//{
		//	if (ctx.interaction is HoldInteraction)
		//	{
		//		ReleaseArrow(true);
		//	}
		//};
	}


	private void Update()
	{
		if (!lockInput)
			GetPlayerInput();

		PlayerAnimation();
	}

	private void OnControlsChanged(PlayerInput input)
	{
	// Here we can tell what Control Scheme is currently being used:
		switch (input.currentControlScheme)
		{
			case "Gamepad":
				currentControlScheme = ControlSchemes.Gamepad;
				UIManager.Instance.TouchInputObjectGroupActivation(false);
				aimPoint = Vector2.zero;
				break;
			case "Touch":
				currentControlScheme = ControlSchemes.Touch;
				UIManager.Instance.TouchInputObjectGroupActivation(true);
				aimPoint = Vector2.zero;
				break;
			case "Keyboard and Mouse":
				currentControlScheme = ControlSchemes.KeyboardAndMouse;
				UIManager.Instance.TouchInputObjectGroupActivation(false);
				aimPoint = Vector2.zero;
				break;
			default:
				break;
		}
		print($"control scheme changed now to: {input.currentControlScheme}");
	}

	private void FixedUpdate()
	{
		PlayerMovement();
	}

	private void OnTriggerEnter(Collider other)
	{
		other.TryGetComponent(out IInteractable interactable);

		if (interactable == null) return;

		// We might wanna check if this has a trigger, but isn't blueberries or lingonberries, which has triggers to be able to be interacted with.
		other.TryGetComponent(out MobBehaviour mob);
		if (other.isTrigger && mob != null) return;

		_interactablesInRange.Add(other.gameObject);

		if (other.TryGetComponent(out TreeBehaviour tree))
		{
			tree.OnDestruct += OnInteractableDestroy;
			if (tree.fruits.Count > 0)
			{
				for (int i = 0; i < tree.fruits.Count; i++)
				{
					tree.fruits[i].OnDestruct += OnInteractableDestroy;
				}
			}
			return;
		}

		if (other.TryGetComponent(out FoodBehaviour food))
		{
			food.OnDestruct += OnInteractableDestroy;
			return;
		}

		if (mob != null)
		{
			mob.OnButcher += OnInteractableDestroy;
			return;
		}

		if (other.TryGetComponent(out TrapBehaviour trap))
		{
			trap.OnCollect += OnInteractableDestroy;
			trap.PickupTrap += RemoveTrapFromInteractablesList;
			return;
		}

		if (other.TryGetComponent(out StorageHandler storage))
		{
			if (storage.type == StorageType.Wood)
				UIManager.Instance.UpdateStorageStatusUI("Wood Stack", StorageController.Instance.woodStorage, storage.type);
			else
				UIManager.Instance.UpdateStorageStatusUI("Food Stash", StorageController.Instance.foodStorage, storage.type);

			UIManager.Instance.StorageStatusUIActivation(true);
			return;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		var interactable = other.transform.GetComponent<IInteractable>();
		if (interactable == null) return;
		/*print($"{other.transform} has left trigger");*/
		_interactablesInRange.Remove(other.gameObject);

		if (other.TryGetComponent(out TreeBehaviour tree))
		{
			tree.OnDestruct -= OnInteractableDestroy;
			if (tree.fruits.Count > 0)
			{
				for (int i = 0; i < tree.fruits.Count; i++)
				{
					tree.fruits[i].OnDestruct -= OnInteractableDestroy;
				}
			}
			return;
		}

		if (other.TryGetComponent(out FoodBehaviour food))
		{
			food.OnDestruct -= OnInteractableDestroy;
			return;
		}

		if (other.TryGetComponent(out MobBehaviour mob))
		{
			mob.OnButcher -= OnInteractableDestroy;
			return;
		}

		if (other.TryGetComponent(out TrapBehaviour trap))
		{
			trap.OnCollect -= OnInteractableDestroy;
			trap.PickupTrap -= RemoveTrapFromInteractablesList;
			return;
		}

		if (other.TryGetComponent(out StorageHandler storage))
		{
			UIManager.Instance.StorageStatusUIActivation(false);
			return;
		}
	}

	public void UnlockInput()
	{
		lockInput = false;
		//playerInput.ActivateInput(); // This could probably only be used if we would only use the PlayerInput-component to connect the actions to the buttons.
	}

	private void GetPlayerInput()
	{
		_movement = _controls.Player.Movement.ReadValue<Vector2>();
		aimPoint = /*_controls.Player.MousePoint.ReadValue<Vector2>() + _controls.Player.AimPoint.ReadValue<Vector2>()*/currentControlScheme == ControlSchemes.KeyboardAndMouse ? _controls.Player.MousePoint.ReadValue<Vector2>() : _controls.Player.AimPoint.ReadValue<Vector2>();

		_horizontal = _movement.x;
		_vertical = _movement.y;
		//print($"horizontal: {horizontal}\nvertical: {vertical}");

		// Touch sprint:
		if (currentControlScheme == ControlSchemes.Touch && (Mathf.Abs(_moveDigitalJoystick.position.x) >= 70f || Mathf.Abs(_moveDigitalJoystick.position.y) >= 70f))
		{
			_doSprint = true;
		}
		else if (currentControlScheme == ControlSchemes.Touch && (Mathf.Abs(_moveDigitalJoystick.position.x) < 70f || Mathf.Abs(_moveDigitalJoystick.position.y) < 70f))
		{
			_doSprint = false;
		}

		AimHelper();
	}

	private void AimHelper()
	{
		//aimBall.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(mousePoint.x, mousePoint.y) * -180 / Mathf.PI + 90f);
		aimChild.localPosition = aimPoint;
		Vector3 aimParentPosition = _transform.position;
		aimParentPosition.y = _transform.position.y + aimParentYOffset;
		aimParent.position = aimParentPosition;

		//mousePoint = _controls.Player.MousePoint.ReadValue<Vector2>();
	}

	/// <summary>
	/// The way the Player is moving: Starts with low speed but quickly builds up speed in the direction its moving.
	/// Press Left Shift to Sprint
	/// </summary>
	private void PlayerMovement()
	{
		//print(movement);

		// Here we make sure that we still can slowly increase or build up from 0 to 1 when starting to move, but then if the input becomes higher than it should it will be normalized.
		if (_movement.sqrMagnitude > 1f)
			_movement = _movement.normalized;

		_rb.velocity = new Vector3(_movement.x, 0f, _movement.y) * MovementSpeed(_doSprint) * Time.deltaTime;
	}

	private float MovementSpeed(bool sprint)
	{
		float speed = sprint ? _sprintSpeed : _walkSpeed;

		return speed;
	}

	private void PlaceTrap()
	{
		if (lockInput) return;

		OnPlaceTrap?.Invoke(_transform.position);
	}

	private void Interact()
	{
		if (lockInput || _interactablesInRange.Count == 0 || _interactablesInRange[0] == null) return;

		GameObject nearestObject = NearestObject();
		int index = GetIndexFromList(_interactablesInRange, nearestObject);

		// Here we make sure that if the energy is depleted, then we shouldn't be able to gather anymore resource, thus no interaction with either food or tree objects.
		if (!_hasEnergy && (_interactablesInRange[index].TryGetComponent(out TreeBehaviour _)
					  || _interactablesInRange[index].TryGetComponent(out FoodBehaviour _)
					  || _interactablesInRange[index].TryGetComponent(out MobBehaviour _)
					  || _interactablesInRange[index].TryGetComponent(out TrapBehaviour _))) return;

		var interactable = _interactablesInRange[index].GetComponent<IInteractable>();
		if (interactable == null) return;
		interactable.OnInteract();

		// Let's turn the player's animation in the direction of what it is interacting with.
		SetPlayerAnimationDirection(nearestObject);
	}

	private void ChargeArrow()
	{
		if (lockInput || !_hasEnergy) return; 

		_arrowCanceled = false;
		BowBehaviour.Instance.ChargeArrow();
	}

	private void ReleaseArrow(bool doShoot)
	{
		if (lockInput || !_hasEnergy) return;

		if (!doShoot)
		{
			BowBehaviour.Instance.CancelArrow();
			_arrowCanceled = true;
			return;
		}

		if (_arrowCanceled)
		{
			_arrowCanceled = false;
			return;
		}

		BowBehaviour.Instance.ReleaseArrow(aimPoint);
	}

	private void SetHasEnergyFalse() => _hasEnergy = false;
	private void SetHasEnergyTrue() => _hasEnergy = true;

	private void PlayerAnimation() // needs love!
	{
		_animator.SetFloat("SpeedX", _horizontal);
		_animator.SetFloat("SpeedY", _vertical);
	}

	public void SetPlayerAnimationDirection(GameObject faceThisObject)
	{
		if (faceThisObject == null) return;
		Vector3 direction = FindDirectionFromTwoPoints(_transform.position, faceThisObject.transform.position);
		Vector2 direction2D = new Vector2(direction.x, direction.z) * -1; // it's reversed to be more be more like a conventional grid ( Up / Right = positive )

		//print($"direction2D before: {direction2D}");

		// Round the Vector2 values to be comparable to Vector2.left/right/up/down/zero
		// Middle X
		if (direction2D.x >= -0.5f && direction2D.x <= 0.5f)
			direction2D.x = 0f;
		// Left
		else if (direction2D.x < -0.5f)
			direction2D.x = -1f;
		// Right
		else if (direction2D.x > 0.5f)
			direction2D.x = 1f;

		// Middle Y
		if (direction2D.y >= -0.5f && direction2D.y <= 0.5f)
			direction2D.y = 0f;
		// Up
		else if (direction2D.y > 0.5f)
			direction2D.y = 1f;
		// Down
		else if (direction2D.y < -0.5f)
			direction2D.y = -1f;

		// If we get doubles/borders(0,0/1,1/-1,1/etc.) - check which of the axises that are dominant.
		// If we get 0,0: we set the dominant to +/- 1.
		if (direction2D == Vector2.zero)
		{
			if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
			{
				direction2D.x = direction.x > 0f ? 1f : -1f;
			}
			else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
			{
				direction2D.y = direction.y > 0f ? 1f : -1f;
			}
		}
		// if  we get 1,1: we set the submissive to 0.
		else if (new Vector2(Mathf.Abs(direction2D.x), Mathf.Abs(direction2D.y)) == Vector2.one)
		{
			if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
			{
				direction2D.y = 0f;
			}
			else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
			{
				direction2D.x = 0f;
			}
		}

		// Find what direction it correlates to, to assign the animation accordingly.
		if (direction2D == Vector2.left)
		{
			_animator.SetTrigger("IdleRight");
		}
		else if (direction2D == Vector2.right)
		{
			_animator.SetTrigger("IdleLeft");
		}
		else if (direction2D == Vector2.up)
		{
			_animator.SetTrigger("IdleDown");
		}
		else if (direction2D == Vector2.down)
		{
			_animator.SetTrigger("IdleUp");
		}

		//print($"direction2D after: {direction2D}");
	}

	/// <summary>
	/// Find the direction from one point to another (Normalized).
	/// </summary>
	/// <param name="fromPoint"></param>
	/// <param name="toPoint"></param>
	/// <returns>A Vector3 representing the direction.</returns>
	private Vector3 FindDirectionFromTwoPoints(Vector3 fromPoint, Vector3 toPoint)
	{
		Vector3 direction = (toPoint - fromPoint).normalized/* * 10f*/;

		return direction;
	}

	private void OnInteractableDestroy(GameObject obj)
	{
		print($"OnResourceDestroy: {obj}");
		obj.TryGetComponent(out FoodBehaviour food);

		if (!_interactablesInRange.Contains(obj) && food != null && food.type != ResourceType.apple)
		{
			return;
		}
		else if (food == null || (food != null && food.type != ResourceType.apple))
		{
			//print("Not Apple");
			//int index = GetIndexFromList(_interactablesInRange, obj); // not necessary considering that we already have a reference to the object that we want to remove
			if (_interactablesInRange.Contains(obj))
				_interactablesInRange.Remove(obj);
		}

		if (obj.TryGetComponent(out TreeBehaviour tree))
		{
			tree.OnDestruct -= OnInteractableDestroy;
			ResourceGathered?.Invoke(obj);
			EnergyDrain?.Invoke(tree.costSize);
			return;
		}

		if (food != null)
		{
			food.OnDestruct -= OnInteractableDestroy;
			ResourceGathered?.Invoke(obj);
			EnergyDrain?.Invoke(food.costSize);
			return;
		}

		if (obj.TryGetComponent(out MobBehaviour mob))
		{
			mob.OnButcher -= OnInteractableDestroy;
			MobController.Instance.RemoveButcheredFromDeadMobDictionary(obj);
			ResourceGathered?.Invoke(obj);
			EnergyDrain?.Invoke(mob.costSize);
			return;
		}

		if (obj.TryGetComponent(out TrapBehaviour trap))
		{
			trap.OnCollect -= OnInteractableDestroy;
			if (_interactablesInRange.Contains(obj))
				_interactablesInRange.Remove(obj);
			ResourceGathered?.Invoke(obj);
			EnergyDrain?.Invoke(trap.costSize);
			return;
		}
	}

	public void RemoveTrapFromInteractablesList(GameObject obj, TrapBehaviour trap)
	{
		_interactablesInRange.Remove(obj);
		trap.OnCollect -= OnInteractableDestroy;
		trap.PickupTrap -= RemoveTrapFromInteractablesList;
	}

	public void ClearInteractablesInRangeList()
	{
		_interactablesInRange.Clear();
	}

	private GameObject NearestObject()
	{
		GameObject nearestObject = null;
		//int index = 0;
		//if (interactablesInRange.Count > 1) {
		float nearestDistance = 100f;
		for (int i = 0; i < _interactablesInRange.Count; i++)
		{
			float distance = Vector3.Distance(_transform.position, _interactablesInRange[i].transform.position);
			if (distance < nearestDistance)
			{
				nearestDistance = distance;
				nearestObject = _interactablesInRange[i];
				//index = i;
			}
			//}
		}
		return nearestObject;
	}

	/// <summary>
	/// Find the Index of a GameObject in a List<GameObject>
	/// </summary>
	/// <param name="list"></param>
	/// <param name="objToLocate"></param>
	/// <returns>an int representing the index number in the list where the GameObject is located.</returns>
	private int GetIndexFromList(List<GameObject> list, GameObject objToLocate)
	{
		int index = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == objToLocate)
				index = i;
		}
		return index;
	}

	private void OnDisable()
	{
		_controls.Disable();
		_controls.Player.Sprint.started -= ctx => _doSprint = true;
		_controls.Player.Sprint.canceled -= ctx => _doSprint = false;
		_controls.Player.Interact.started -= ctx => Interact();
		_controls.Player.PlaceTrap.started -= ctx => PlaceTrap();
		_controls.Player.ShootArrow.started -= ctx => BowBehaviour.Instance.ChargeArrow();
		_controls.Player.ShootArrow.canceled -= ctx => ReleaseArrow(true);
		_controls.Player.CancelArrow.started -= ctx => ReleaseArrow(false);
		_controls.Player.CancelArrow.canceled -= ctx => ReleaseArrow(false);
		_controls.Player.Inventory.started -= ctx => UIManager.Instance.ToggleInventoryActive();

		TreeController.Instance.OnClearTrees -= ClearInteractablesInRangeList;
		FoodController.Instance.OnClearFoods -= ClearInteractablesInRangeList;
		MobController.Instance.OnClearMobs -= ClearInteractablesInRangeList;
		EnergyController.Instance.EnergyDepleted -= SetHasEnergyFalse;
		StorageController.Instance.GoalAccomplished -= SetHasEnergyTrue;

		playerInput.onControlsChanged -= OnControlsChanged;

		//actionReference.action.Disable();
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float walkSpeed = 20f, sprintSpeed = 40f;
	[SerializeField] private List<GameObject> interactablesInRange;

	public event Action SpawnNewForest;
	public event Action<GameObject> ResourceGathered;
	public event Action<ResourceSize> EnergyDrain;

	private InputMaster controls;
	private Transform _transform;
	private Animator _animator;
	//private SpriteRenderer _spriteRenderer;
	private Rigidbody _rb;
	private Vector2 movement;
	private float horizontal, vertical;
	private bool doSprint;


	private void Awake()
	{
		_transform = transform;
		_rb = GetComponent<Rigidbody>();
		//_spriteRenderer = GetComponent<SpriteRenderer>();
		_animator = GetComponent<Animator>();

		controls = new InputMaster();
		controls.Player.Movement.ReadValue<Vector2>();
	}

	private void OnEnable()
	{
		controls.Enable();
		doSprint = false;
		controls.Player.Sprint.started += ctx => doSprint = true;
		controls.Player.Sprint.canceled += ctx => doSprint = false;
		controls.Player.Interact.started += ctx => Interact();

		ForestController.Instance.OnClearForest += ClearInteractablesInRangeList;
	}


	private void Update()
	{
		GetPlayerInput();
		PlayerAnimation();
	}

	private void FixedUpdate()
	{
		PlayerMovement();
	}

	private void OnTriggerEnter(Collider other)
	{
		var interactable = other.transform.GetComponent<IInteractable>();
		if (interactable == null) return;
		interactablesInRange.Add(other.gameObject);

		var tree = other.transform.GetComponent<TreeBehaviour>();

		if (tree != null) {
			tree.OnDestroy += OnResourceDestroy;
			return;
		}

		var food = other.transform.GetComponent<FoodBehaviour>();

		if (food != null) {
			food.OnDestroy += OnResourceDestroy;
			return;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		var interactable = other.transform.GetComponent<IInteractable>();
		if (interactable == null) return;
		print($"{other.transform} has left trigger");
		interactablesInRange.Remove(other.gameObject);

		var tree = other.transform.GetComponent<TreeBehaviour>();

		if (tree != null) {
			tree.OnDestroy -= OnResourceDestroy;
			return;
		}

		var food = other.transform.GetComponent<FoodBehaviour>();

		if (food != null) {
			food.OnDestroy -= OnResourceDestroy;
			return;
		}
	}

	private void GetPlayerInput()
	{
		movement = controls.Player.Movement.ReadValue<Vector2>();

		horizontal = movement.x;
		vertical = movement.y;
		//print($"horizontal: {horizontal}\nvertical: {vertical}");
	}

	/// <summary>
	/// The way the Player is moving: Starts with low speed but quickly builds up speed in the direction its moving.
	/// Press Left Shift to Sprint
	/// </summary>
	private void PlayerMovement()
	{
		//print(movement);

		// Here we make sure that we still can slowly increase or build up from 0 to 1 when starting to move, but then if the input becomes higher than it should it will be normalized.
		if (movement.sqrMagnitude > 1f)
			movement = movement.normalized;

		_rb.velocity = new Vector3(movement.x, 0f, movement.y) * MovementSpeed(doSprint) * Time.deltaTime;
	}

	private float MovementSpeed(bool sprint)
	{
		float speed = sprint ? sprintSpeed : walkSpeed;

		return speed;
	}

	private void Interact()
	{
		if (interactablesInRange.Count == 0 || interactablesInRange[0] == null) return;

		GameObject nearestObject = NearestObject();
		int index = GetIndexFromList(interactablesInRange, nearestObject);

		var interactable = interactablesInRange[index].GetComponent<IInteractable>();
		if (interactable == null) return;
		interactable.OnInteract();

		// Let's turn the player's animation in the direction of what it is interacting with.
		SetPlayerAnimationDirection(nearestObject);
	}

	private void PlayerAnimation() // needs love!
	{
		_animator.SetFloat("SpeedX", horizontal);
		_animator.SetFloat("SpeedY", vertical);
	}

	private void SetPlayerAnimationDirection(GameObject faceThisObject)
	{
		if (faceThisObject == null) return;
		Vector3 direction = FindDirectionFromTwoPoints(_transform.position, faceThisObject.transform.position);
		Vector2 direction2D = new Vector2(direction.x, direction.z) * -1; // it's reversed to be more be more like a conventional grid ( Up / Right = positive )
		
		print($"direction2D before: {direction2D}");

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
		if (direction2D == Vector2.zero) {
			if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
				direction2D.x = direction.x > 0f ? 1f : -1f;
			}
			else if (Mathf.Abs(direction.x )< Mathf.Abs(direction.y)) {
				direction2D.y = direction.y > 0f ? 1f : -1f;
			}
		}
		// if  we get 1,1: we set the submissive to 0.
		else if (new Vector2(Mathf.Abs(direction2D.x), Mathf.Abs(direction2D.y)) == Vector2.one) {
			if (Mathf.Abs(direction.x )> Mathf.Abs(direction.y)) {
				direction2D.y = 0f;
			}
			else if (Mathf.Abs(direction.x )< Mathf.Abs(direction.y)) {
				direction2D.x = 0f;
			}
		}
		
	// Find what direction it correlates to, to assign the animation accordingly.
		if (direction2D == Vector2.left) {
			_animator.SetTrigger("IdleRight");
		}
		else if (direction2D == Vector2.right) {
			_animator.SetTrigger("IdleLeft");
		}
		else if (direction2D == Vector2.up) {
			_animator.SetTrigger("IdleDown");
		}
		else if (direction2D == Vector2.down) {
			_animator.SetTrigger("IdleUp");
		}

		print($"direction2D after: {direction2D}");
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

	private void OnResourceDestroy(GameObject obj)
	{
		if (!interactablesInRange.Contains(obj)) return;

		int index = GetIndexFromList(interactablesInRange, obj);
		interactablesInRange.RemoveAt(index);

		var tree = obj.transform.GetComponent<TreeBehaviour>();
		if (tree != null) {
			tree.OnDestroy -= OnResourceDestroy;
			ResourceGathered?.Invoke(obj);
			EnergyDrain?.Invoke(tree.size);
			return;
		}

		var food = obj.transform.GetComponent<FoodBehaviour>();

		if (food != null) {
			food.OnDestroy -= OnResourceDestroy;
			ResourceGathered?.Invoke(obj);
			EnergyDrain?.Invoke(food.size);
		}
	}

	private void ClearInteractablesInRangeList()
	{
		interactablesInRange.Clear();
	}

	private GameObject NearestObject()
	{
		GameObject nearestObject = null;
		//int index = 0;
		//if (interactablesInRange.Count > 1) {
			float nearestDistance = 100f;
			for (int i = 0; i < interactablesInRange.Count; i++) {
				float distance = Vector3.Distance(_transform.position, interactablesInRange[i].transform.position);
				if (distance < nearestDistance) {
					nearestDistance = distance;
					nearestObject = interactablesInRange[i];
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
		for (int i = 0; i < list.Count; i++) {
			if (list[i] == objToLocate)
				index = i;
		}
		return index;
	}
	
	private void OnDisable()
	{
		controls.Disable();
		controls.Player.Sprint.started -= ctx => doSprint = true;
		controls.Player.Sprint.canceled -= ctx => doSprint = false;
		controls.Player.Interact.started -= ctx => Interact();

		ForestController.Instance.OnClearForest -= ClearInteractablesInRangeList;
	}
}

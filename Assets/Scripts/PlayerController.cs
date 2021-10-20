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

	private void OnDisable()
	{
		controls.Disable();
		controls.Player.Sprint.started -= ctx => doSprint = true;
		controls.Player.Sprint.canceled -= ctx => doSprint = false;
		controls.Player.Interact.started -= ctx => Interact();
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
		
		if (tree != null)
        {
			tree.OnDestroy += OnResourceDestroy;
			return;
        }

		var food = other.transform.GetComponent<FoodBehaviour>();

        if (food != null)
        {
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

		if (tree != null)
		{
			tree.OnDestroy -= OnResourceDestroy;
			return;
		}

		var food = other.transform.GetComponent<FoodBehaviour>();

		if (food != null)
		{
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

		int index = GetIndexFromList(interactablesInRange, NearestObject());

		var interactable = interactablesInRange[index].GetComponent<IInteractable>();
		if (interactable == null) return;
		interactable.OnInteract();
	}

	private void PlayerAnimation() // needs love!
	{
		_animator.SetFloat("SpeedX", horizontal);
		_animator.SetFloat("SpeedY", vertical);
	}

	private void OnResourceDestroy(GameObject obj)
	{
		if (!interactablesInRange.Contains(obj)) return;

		int index = GetIndexFromList(interactablesInRange, obj);
		interactablesInRange.RemoveAt(index);

		var tree = obj.transform.GetComponent<TreeBehaviour>();
		if (tree != null)
        {
			tree.OnDestroy -= OnResourceDestroy;
			ResourceGathered?.Invoke(obj);
			return;
        }

		var food = obj.transform.GetComponent<FoodBehaviour>();

		if (food != null)
		{
			food.OnDestroy -= OnResourceDestroy;
			ResourceGathered?.Invoke(obj);
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
		if (interactablesInRange.Count > 1) {
			float nearestDistance = 100f;
			for (int i = 0; i < interactablesInRange.Count; i++) {
				float distance = Vector3.Distance(_transform.position, interactablesInRange[i].transform.position);
				if (distance < nearestDistance) {
					nearestDistance = distance;
					nearestObject = interactablesInRange[i];
					//index = i;
				}
			}
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
}

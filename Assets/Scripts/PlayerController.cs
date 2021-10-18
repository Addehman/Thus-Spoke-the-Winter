using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private Vector2 playerDirection;
	[SerializeField] private float walkSpeed = 20f, sprintSpeed = 40f;
	[SerializeField] private int doDamage_tree = 20;
	[SerializeField] private Transform _interactTrigger, interactableTransformInRange;

	public event Action SpawnNewForest;

	private InputMaster controls;
	private Transform _transform;
	private Animator _animator;
	private SpriteRenderer _spriteRenderer;
	private Rigidbody _rb;
	private BoxCollider2D interactTriggerCollider;
	private Vector2 wrapPos, movement;
	private IInteractable interactableInRange;
	private int orderIncrease = 100;
	private float horizontal, vertical,
		triggerOffsetX_Horizontal = 0.16f, triggerOffsetX_Vertical = -0.01f,
		triggerOffsetY_Up = 0.05f, triggerOffsetY_Down = -0.3f, triggerOffsetY_Horizontal = -0.22f;
	public bool doSprint;


	private void Awake()
	{
		_transform = transform;
		_rb = GetComponent<Rigidbody>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_animator = GetComponent<Animator>();

		controls = new InputMaster();
		controls.Player.Movement.ReadValue<Vector2>();
	}

	private void Start()
	{
		//Transform[] children = GetComponentsInChildren<Transform>();
		//_interactTrigger = children[1];
		_interactTrigger = _transform.GetChild(0);
		interactTriggerCollider = _interactTrigger.GetComponent<BoxCollider2D>();
	}

	private void OnEnable()
	{
		controls.Enable();
		doSprint = false;
		controls.Player.Sprint.started += ctx => doSprint = true;
		controls.Player.Sprint.canceled += ctx => doSprint = false;
	}

	private void OnDisable()
	{
		controls.Disable();
		controls.Player.Sprint.started -= ctx => doSprint = true;
		controls.Player.Sprint.canceled -= ctx => doSprint = false;
	}

	private void Update()
	{
		//horizontal = Input.GetAxis("Horizontal");
		//vertical = Input.GetAxis("Vertical");

		GetPlayerInput();
		PlayerAnimation();
		//SetPlayerDirection(); // closed due to new input system is being implemented

		//if (Input.GetKeyDown(KeyCode.Space) && interactableInRange != null && interactableTransformInRange != null) { // closed due to new input system is being implemented
		//	interactableInRange.Interact(doDamage_tree);
		//}
	}

	private void FixedUpdate()
	{
		PlayerMovement();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		var interactable = other.transform.GetComponent<IInteractable>();
		if (interactable == null) return;
		interactableInRange = interactable;
		interactableTransformInRange = interactableInRange == interactable ? other.transform : null;
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		var interactable = other.transform.GetComponent<IInteractable>();
		if (interactable == null) return;
		//print($"{other.transform} has left trigger");
		interactableInRange = null;
		interactableTransformInRange = null;
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

	//private Vector2 SetPlayerDirection()
	//{
	//	string id = IdentifyDirection(movement);

	//	switch (id) {
	//		case "left":
	//			SetTriggerRotation(id);
	//			playerDirection = Vector2.left;
	//			break;
	//		case "up":
	//			SetTriggerRotation(id);
	//			playerDirection = Vector2.up;
	//			break;
	//		case "right":
	//			SetTriggerRotation(id);
	//			playerDirection = Vector2.right;
	//			break;
	//		case "down":
	//			SetTriggerRotation(id);
	//			playerDirection = Vector2.down;
	//			break;
	//	}

	//	return playerDirection;
	//}

	//private string IdentifyDirection(Vector2 direction)
	//{
	//	if (direction.sqrMagnitude > 0f) {
	//		if (left)
	//			return "left";
	//		else if (up)
	//			return "up";
	//		else if (right)
	//			return "right";
	//		else if (down)
	//			return "down";
	//		else
	//			return "";
	//	}
	//	else {
	//		return "";
	//	}
	//}

	//private void SetTriggerRotation(string direction)
	//{
	//	float degrees, triggerOffsetX, triggerOffsetY;
	//	switch (direction) {
	//		case "left":
	//			degrees = 270f;
	//			triggerOffsetX = triggerOffsetX_Horizontal;
	//			triggerOffsetY = triggerOffsetY_Horizontal;
	//			break;
	//		case "up":
	//			degrees = 180f;
	//			triggerOffsetX = -triggerOffsetX_Vertical;
	//			triggerOffsetY = triggerOffsetY_Up;
	//			break;
	//		case "right":
	//			degrees = 90f;
	//			triggerOffsetX = -triggerOffsetX_Horizontal;
	//			triggerOffsetY = triggerOffsetY_Horizontal;
	//			break;
	//		default:
	//			degrees = 0f;
	//			triggerOffsetX = triggerOffsetX_Vertical;
	//			triggerOffsetY = triggerOffsetY_Down;
	//			break;
	//	}
	//	Vector3 newRotation = _interactTrigger.eulerAngles;
	//	newRotation.z = degrees;
	//	_interactTrigger.eulerAngles = newRotation;

	//	interactTriggerCollider.offset = new Vector2(triggerOffsetX, triggerOffsetY);
	//}

	private void SetOrder()
	{
		float order = _transform.position.y * orderIncrease * -1;
		_spriteRenderer.sortingOrder = (int)order;
	}

	private float MovementSpeed(bool sprint)
	{
		float speed = sprint ? sprintSpeed : walkSpeed;

		return speed;
	}

	private void PlayerAnimation() // needs love!
	{
		_animator.SetFloat("SpeedX", horizontal);
		_animator.SetFloat("SpeedY", vertical);

		print($"SpeedX: {_animator.GetFloat("SpeedX")}\n SpeedY: {_animator.GetFloat("SpeedY")}");
		

		//if (!Input.GetButton("Horizontal"))
		//	_animator.SetFloat("SpeedX", 0f);
		//else
		//	_animator.SetFloat("SpeedX", horizontal);

		//if (!Input.GetButton("Vertical"))
		//	_animator.SetFloat("SpeedY", 0f);
		//else
		//	_animator.SetFloat("SpeedY", vertical);
	}

	private void ResetTrigger()
	{
		interactTriggerCollider.enabled = false;
		interactTriggerCollider.enabled = true;
	}
}

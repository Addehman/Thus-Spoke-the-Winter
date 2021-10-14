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
	private Vector2 wrapPos;
	private IInteractable interactableInRange;
	private int orderIncrease = 100;
	private float horizontal, vertical,
		triggerOffsetX_Horizontal = 0.16f, triggerOffsetX_Vertical = -0.01f, 
		triggerOffsetY_Up = 0.05f, triggerOffsetY_Down = -0.3f, triggerOffsetY_Horizontal = -0.22f;
	private bool left = false, up = false, right = false, down = false;


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
	}

	private void OnDisable()
	{
		controls.Disable();
	}

	private void MovementInput(Vector2 movement)
	{
		horizontal = movement.x;
		vertical = movement.y;
		//print($"horizontal: {horizontal}\nvertical: {vertical}");
	}

	private void Update()
	{
		//horizontal = Input.GetAxis("Horizontal");
		//vertical = Input.GetAxis("Vertical");

		ScreenWrap();
		SetOrder();
		//PlayerAnimation(); // closed due to new input system is being implemented
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

	/// <summary>
	/// The way the Player is moving: Starts with low speed but quickly builds up speed in the direction its moving.
	/// Press Left Shift to Sprint
	/// </summary>
	private void PlayerMovement()
	{
		Vector2 movement = controls.Player.Movement.ReadValue<Vector2>();
		//print(movement);

		// Here we make sure that we still can slowly increase or build up from 0 to 1 when starting to move, but then if the input becomes higher than it should it will be normalized.
		if (movement.sqrMagnitude > 1f)
			movement = movement.normalized;

		_rb.velocity = new Vector3(movement.x, 0f, movement.y) * MovementSpeed() * Time.deltaTime;
	}

	private Vector2 SetPlayerDirection()
	{
		string id = IdentifyMoveInputUp();
		if (!left && !up && !right && !down) {
			return playerDirection;
		}

		if (left) {
			SetTriggerRotation(id);
			playerDirection = Vector2.left;
		}
		else if (up) {
			SetTriggerRotation(id);
			playerDirection = Vector2.up;
		}
		else if (right) {
			SetTriggerRotation(id);
			playerDirection = Vector2.right;
		}
		else {
			SetTriggerRotation(id);
			playerDirection = Vector2.down;
		}
		return playerDirection;
	}

	private string IdentifyMoveInputUp()
	{
		left = Input.GetKey(KeyCode.A);
		up = Input.GetKey(KeyCode.W);
		right = Input.GetKey(KeyCode.D);
		down = Input.GetKey(KeyCode.S);

		if (left)
			return "left";
		else if (up)
			return "up";
		else if (right)
			return "right";
		else
			return "down";
	}

	private void SetTriggerRotation(string directionName)
	{
		float degrees, triggerOffsetX, triggerOffsetY;
		switch (directionName) {
			case "left":
				degrees = 270f;
				triggerOffsetX = triggerOffsetX_Horizontal;
				triggerOffsetY = triggerOffsetY_Horizontal;
				break;
			case "up":
				degrees = 180f;
				triggerOffsetX = -triggerOffsetX_Vertical;
				triggerOffsetY = triggerOffsetY_Up;
				break;
			case "right":
				degrees = 90f;
				triggerOffsetX = -triggerOffsetX_Horizontal;
				triggerOffsetY = triggerOffsetY_Horizontal;
				break;
			default:
				degrees = 0f;
				triggerOffsetX = triggerOffsetX_Vertical;
				triggerOffsetY = triggerOffsetY_Down;
				break;
		}
		Vector3 newRotation = _interactTrigger.eulerAngles;
		newRotation.z = degrees;
		_interactTrigger.eulerAngles = newRotation;

		interactTriggerCollider.offset = new Vector2(triggerOffsetX, triggerOffsetY);
	}

	private void ScreenWrap()
	{
	//ScreenWrap and generate new forest when leaving the screen in any direction.
		if (_transform.position.x > GameManager.ScreenBorder_Right) {
			float yPos = _transform.position.z;
			wrapPos = new Vector2(GameManager.ScreenBorder_Left, yPos);
			_transform.position = wrapPos;

			SpawnNewForest?.Invoke();
		}

		if (_transform.position.x < GameManager.ScreenBorder_Left) {
			float yPos = _transform.position.z;
			wrapPos = new Vector2(GameManager.ScreenBorder_Right, yPos);
			_transform.position = wrapPos;

			SpawnNewForest?.Invoke();
		}

		if (_transform.position.y > GameManager.ScreenBorder_Top) {
			float xPos = _transform.position.x;
			wrapPos = new Vector2(xPos, GameManager.ScreenBorder_Bottom);
			_transform.position = wrapPos;

			SpawnNewForest?.Invoke();
		}

		if (_transform.position.y < GameManager.ScreenBorder_Bottom) {
			float xPos = _transform.position.x;
			wrapPos = new Vector2(xPos, GameManager.ScreenBorder_Top);
			_transform.position = wrapPos;

			SpawnNewForest?.Invoke();
		}
	}

	private void SetOrder()
	{
		float order = _transform.position.y * orderIncrease * -1;
		_spriteRenderer.sortingOrder = (int)order;
	}

	private float MovementSpeed()
	{
		float speed = walkSpeed;
		//if (Input.GetKey(KeyCode.LeftShift)) // closed due to new input system is being implemented
		//	speed = sprintSpeed;

		return speed;
	}

	private void PlayerAnimation() // needs love!
	{
		if (!Input.GetButton("Horizontal"))
			_animator.SetFloat("SpeedX", 0f);
		else
			_animator.SetFloat("SpeedX", horizontal);

		if (!Input.GetButton("Vertical"))
			_animator.SetFloat("SpeedY", 0f);
		else
			_animator.SetFloat("SpeedY", vertical);
	}

	private void ResetTrigger()
	{
		interactTriggerCollider.enabled = false;
		interactTriggerCollider.enabled = true;
	}
}

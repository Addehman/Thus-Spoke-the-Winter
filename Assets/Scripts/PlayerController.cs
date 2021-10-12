using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public event Action SpawnNewForest;
	[SerializeField] private float walkSpeed = 20f, sprintSpeed = 40f;
#if UNITY_EDITOR
	[SerializeField] private float movementInput;
#endif

	private Transform _transform;
	private Animator _animator;
	private SpriteRenderer _spriteRenderer;
	private Rigidbody2D _rb;
	private Vector2 wrapPos;
	private int orderIncrease = 100;
	private float horizontal, vertical;


	private void Start()
	{
		_transform = transform;
		_rb = GetComponent<Rigidbody2D>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_animator = GetComponent<Animator>();

#if UNITY_EDITOR
		//sprintSpeed = 80f;
#endif
	}

	private void Update()
	{
		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");

		ScreenWrap();
		SetOrder();
		PlayerAnimation();
	}

	private void FixedUpdate()
	{
		PlayerMovement();
	}
	/// <summary>
	/// The way the Player is moving: Starts with low speed but quickly builds up speed in the direction its moving.
	/// Press Left Shift to Sprint
	/// </summary>
	private void PlayerMovement()
	{
		Vector2 movement = new Vector2(horizontal, vertical);
		// Here we make sure that we still can slowly increase or build up from 0 to 1 when starting to move, but then if the input becomes higher than it should it will be normalized.
		if (movement.sqrMagnitude > 1f)
			movement = movement.normalized;

		_rb.velocity = movement * MovementSpeed() * Time.deltaTime;

#if UNITY_EDITOR
		movementInput = movement.sqrMagnitude; // for Debug and testing purposes
#endif
	}

	private void ScreenWrap()
	{
	//ScreenWrap and generate new forest when leaving the screen in any direction.
		if (_transform.position.x > GameManager.ScreenBorder_Right) {
			float yPos = _transform.position.y;
			wrapPos = new Vector2(GameManager.ScreenBorder_Left, yPos);
			_transform.position = wrapPos;

			SpawnNewForest?.Invoke();
		}

		if (_transform.position.x < GameManager.ScreenBorder_Left) {
			float yPos = _transform.position.y;
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
		if (Input.GetKey(KeyCode.LeftShift))
			speed = sprintSpeed;

		return speed;
	}

	private void PlayerAnimation()
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
}

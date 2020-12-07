using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	[SerializeField] GameManager gameManager = null;

	public float speed, playerBorderLeft = -2.4f, playerBorderRight = 2.4f, playerBorderTop = 1.35f, playerBorderBottom = -1.35f;
	private int orderIncrease = 100;

	public Animator animator;

	SpriteRenderer spriteRenderer;
	Vector2 wrapPos;


	private void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update() {
		PlayerMovement();
		ScreenWrap();
		SetOrder();
	}

	private void PlayerMovement() {
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");

		Vector3 normalMovement = new Vector3(x, y, 0);

		if (normalMovement.sqrMagnitude > 1)
			normalMovement = normalMovement.normalized;
		
		Vector3 movement = normalMovement * speed * Time.deltaTime;

		transform.Translate(movement);

		// Just realised that the variable "speed" here is really just here because
		// I want to have a finger in here between the cogwheels so I can modify the 
		// speed out in unity - a public variable which makes it possible for testing 
		// later to fine tune the movement speed later on. Maybe it should really be 
		// named "speedModifier" from now on..

		if (!Input.GetButton("Horizontal"))
			animator.SetFloat("SpeedX", 0f);
		else
			animator.SetFloat("SpeedX", x);

		if (!Input.GetButton("Vertical"))
			animator.SetFloat("SpeedY", 0f);
		else
			animator.SetFloat("SpeedY", y);
	}

	private void ScreenWrap() {
		//ScreenWrap when leaving 
		if (transform.position.x > playerBorderRight) {
			float yPos = transform.position.y;
			wrapPos = new Vector2(playerBorderLeft, yPos);
			transform.position = wrapPos;

			gameManager.SpawnNewForest();
		}

		if (transform.position.x < playerBorderLeft) {
			float yPos = transform.position.y;
			wrapPos = new Vector2(playerBorderRight, yPos);
			transform.position = wrapPos;

			// gameManager.ClearForest();
			gameManager.SpawnNewForest();
		}

		if (transform.position.y > playerBorderTop) {
			float xPos = transform.position.x;
			wrapPos = new Vector2(xPos, playerBorderBottom);
			transform.position = wrapPos;

			gameManager.SpawnNewForest();
		}

		if (transform.position.y < playerBorderBottom) {
			float xPos = transform.position.x;
			wrapPos = new Vector2(xPos, playerBorderTop);
			transform.position = wrapPos;

			gameManager.SpawnNewForest();
		}
	}
	
	private void SetOrder() {
		float order = transform.position.y * orderIncrease * -1;
		spriteRenderer.sortingOrder = (int)order;
	}
}

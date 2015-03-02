using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour {
	
	public float defaultMovementSpeed = 3.5f;
	public float defaultSensitivity = 5.0f;
	public float jumpSpeed = 6.0f;
	public bool jumpEnabled = false;
	public bool keyboard = true;
	
	float verticalRotation = 0;
	public float upDownRange = 60.0f; // Range that you can look up or down in degrees.
	
	float verticalVelocity = 0;
	Vector3 startingPosition;
	
	CharacterController characterController;
	
	// Use this for initialization
	void Start() {
		Screen.lockCursor = true;
		characterController = GetComponent<CharacterController>();
		startingPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update() {
		if (keyboard) {
			Rotate (Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), defaultSensitivity);
			Move (Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"), defaultMovementSpeed);
		}
		// Add VR & Joystick implementation here

		// Check if out of bounds
		if (transform.position.y < -100) JumpToStart ();
	}

	// Rotate player and camera
	void Rotate(float xAxis, float yAxis, float sensitivity) {
		// Horizontal rotation (rotates player)
		float rotLeftRight = xAxis * sensitivity;
		transform.Rotate(0, rotLeftRight, 0);
		
		// Vertical rotation (rotates camera only)
		verticalRotation -= yAxis * sensitivity;
		verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
		Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
	}

	// Move player
	void Move(float xAxis, float yAxis, float movementSpeed) {
		// Horizontal movement
		float forwardSpeed = yAxis * movementSpeed;
		float sideSpeed = xAxis * movementSpeed;

		// Vertical movement
		verticalVelocity += Physics.gravity.y * Time.deltaTime;
		if(characterController.isGrounded && Input.GetButtonDown("Jump") && jumpEnabled) {
			verticalVelocity = jumpSpeed;
		}
		
		Vector3 speed = new Vector3( sideSpeed, verticalVelocity, forwardSpeed );
		speed = transform.rotation * speed;
		
		characterController.Move( speed * Time.deltaTime );
	}

	// Jump to starting position
	void JumpToStart() {
		verticalVelocity = 0;
		transform.position = startingPosition;
	}

}
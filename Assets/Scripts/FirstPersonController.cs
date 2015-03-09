using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent (typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour {
	
	public float defaultMovementSpeed = 3.5f;
	public float defaultSensitivity = 5.0f;
	public float jumpSpeed = 6.0f;
	public bool jumpEnabled = false;
	public bool stereoEnabled = false;
	public enum VR{None, Durovis, Oculus};
	public VR activeVR = VR.None;
	
	private float verticalRotation = 0;
	public float upDownRange = 60.0f; // Range that you can look up or down in degrees.
	
	float verticalVelocity = 0;
	Vector3 startingPosition;
	
	CharacterController characterController;
	public Camera monoCamera;
	public MuseumDiveSensor stereoCameraController;
	
	// Use this for initialization
	void Start() {
		if(!CrossPlatformInputManager.GetActiveInputMethod().Equals(CrossPlatformInputManager.ActiveInputMethod.Touch)
		   || activeVR != VR.None) Screen.lockCursor = true;
		characterController = GetComponent<CharacterController>();
		startingPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update() {
		// Get axes
		float horizontalAxis = CrossPlatformInputManager.GetAxis("Horizontal"); // Controls sideways movement
		float verticalAxis = CrossPlatformInputManager.GetAxis("Vertical"); // Controls forward/backward movement
		float mouseXAxis = CrossPlatformInputManager.GetAxis("Mouse X"); // Controls horizontal camera movement
		float mouseYAxis = CrossPlatformInputManager.GetAxis("Mouse Y"); // Controls vertical camera movement
		float sensitivity = defaultSensitivity;

		if (activeVR == VR.None) {
			if(CrossPlatformInputManager.GetActiveInputMethod().Equals(CrossPlatformInputManager.ActiveInputMethod.Touch)) {
				// Mobile "thumbstick" controls
				// Reduce right stick sensitivity
				sensitivity = defaultSensitivity * 0.75f;
			} else {
				// Keyboard controls
				// Clamp the movement magnitude in a circle instead of both axes separately
				Vector2 movement = Vector2.ClampMagnitude(new Vector2(horizontalAxis, verticalAxis), 1);
				horizontalAxis = movement.x;
				verticalAxis = movement.y;
			}
		} else if (activeVR == VR.Durovis) {
			// TODO: Complete Durovis VR movement implementation. Looking around already works.
			// Modify Horizontal/Vertical axis values depending on vertical rotation (look at ground = stop/start).
			// Mouse X and Y axes are not used for rotation (head tracking sets the rotation directly with SetRotation).
		} else if (activeVR == VR.Oculus) {
			// TODO: Complete Oculus Rift movement and looking implementation.
		}

		// Control cameras
		if(stereoEnabled) {
			if(!stereoCameraController.gameObject.activeSelf) stereoCameraController.gameObject.SetActive(true);
			if(monoCamera.gameObject.activeSelf) monoCamera.gameObject.SetActive(false);
			Rotate(mouseXAxis, mouseYAxis, sensitivity);
		} else {
			if(!monoCamera.gameObject.activeSelf) monoCamera.gameObject.SetActive(true);
			if(stereoCameraController.gameObject.activeSelf) stereoCameraController.gameObject.SetActive(false);
			Rotate(mouseXAxis, mouseYAxis, sensitivity);
		}

		// Only the axes are modified via the different input methods, the actual move call remains the same.
		Move(horizontalAxis, verticalAxis, defaultMovementSpeed);

		// Check if out of bounds
		if (transform.position.y < -100) JumpToStart ();
	}

	// Rotate player and camera
	void Rotate(float xAxis, float yAxis, float sensitivity) {
		if (activeVR != VR.None) {
			stereoCameraController.Rotate (xAxis, yAxis, this);
		} else {
			RotateHorizontal(xAxis * sensitivity);
			RotateVertical (-yAxis * sensitivity);
		}
	}

	// Horizontal rotation (rotates player)
	public void RotateHorizontal(float amount) {
		transform.Rotate(0, amount, 0);
	}

	// Vertical rotation (rotates camera only)
	public void RotateVertical(float amount) {
		verticalRotation += amount;
		verticalRotation = Mathf.Clamp (verticalRotation, -upDownRange, upDownRange);
		monoCamera.transform.localRotation = Quaternion.Euler (verticalRotation, 0, 0);
		stereoCameraController.transform.localRotation = Quaternion.Euler (verticalRotation, 0, 0);
	}

	// Direct rotation method, used by head tracking
	public void SetRotation(Quaternion rotation) {
		transform.localRotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
		//verticalRotation = Mathf.Clamp (rotation.eulerAngles.y, -upDownRange, upDownRange);
		verticalRotation = rotation.eulerAngles.x; // No clamp here, we want users to be able to look everywhere!
		monoCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, rotation.eulerAngles.z);
		stereoCameraController.transform.localRotation = Quaternion.Euler (verticalRotation, 0, rotation.eulerAngles.z);
	}

	// Move player
	void Move(float xAxis, float yAxis, float movementSpeed) {
		// Horizontal movement
		float forwardSpeed = yAxis * movementSpeed;
		float sideSpeed = xAxis * movementSpeed;

		// Vertical movement
		verticalVelocity += Physics.gravity.y * Time.deltaTime;
		if(characterController.isGrounded && CrossPlatformInputManager.GetButtonDown("Jump") && jumpEnabled) {
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
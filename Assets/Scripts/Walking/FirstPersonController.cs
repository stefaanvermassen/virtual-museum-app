using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// Controls character and rotates camera based on input obtained through Unity's input axes.
/// These axes are extended to mobile devices using CrossPlatformInput.
/// </summary>
[RequireComponent (typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour {

	/// <summary>
	/// Movement speed when analog stick is fully tilted. Default movement speed for keyboard.
	/// </summary>
	public float defaultMovementSpeed = 3.5f;

	/// <summary>
	/// Default sensitivity to camera rotation. Decreased for mobile joysticks input.
	/// </summary>
	public float defaultSensitivity = 5.0f;

	/// <summary>
	/// Jump speed. Only used when secret/debug jump functionality is activated.
	/// </summary>
	public float jumpSpeed = 6.0f;

	/// <summary>
	/// Enables or disables the secret/debug jump functionality
	/// </summary>
	public bool jumpEnabled = false;
	
	private bool stereoEnabled = false;

	/// <summary>
	/// Enables stereo camera view and disables mono camera view.
	/// </summary>
	public bool StereoEnabled {
		get { return stereoEnabled; }
		set { SwitchCameraMode (value); }
	}

	public enum VR{None, Durovis, Oculus};
	public VR activeVR = VR.None;

	/// <summary>
	/// The active Virtual Reality mode. Usually, this would be no VR at all.
	/// </summary>
	public VR ActiveVR {
		get { return activeVR; }
		set { SwitchVRMode (value); }
	}

	/// <summary>
	/// Vertical rotation of the cameras. The character controller doesn't rotate vertically!
	/// </summary>
	private float verticalRotation = 0;

	/// <summary>
	/// Range that you can look up or down in degrees.
	/// </summary>
	public float upDownRange = 60.0f;

	/// <summary>
	/// Whether or not test mode is active. This will allow the test to call the Start and Update methods manually
	/// </summary>
	public bool testMode = false;


	float verticalVelocity = 0;
	Vector3 startingPosition;
	
	CharacterController characterController;
	public Camera monoCamera;
    public OVRCameraRig ovrCamera;
	public MuseumDiveSensor stereoCameraController;
	public MobileControlRig mobileControlRig;

	bool started = false;

	/// <summary>
	/// Initializes First Person Controller and gets necessary components.
	/// </summary>
	/// <remarks>Also locks screen cursor.</remarks>
	void Start() {
		if((!testMode) && ((!CrossPlatformInputManager.GetActiveInputMethod().Equals(CrossPlatformInputManager.ActiveInputMethod.Touch))
		   || (activeVR != VR.None))) Screen.lockCursor = true;
		SwitchVRMode (activeVR);
		characterController = GetComponent<CharacterController>();
		startingPosition = transform.position;
		started = true;
	}

	/// <summary>
	/// Called once per frame. Updates character position and camera rotation based on input.
	/// </summary>
	void Update() {
		// Get axes
		float horizontalAxis = CrossPlatformInputManager.GetAxis("Horizontal"); // Controls sideways movement
		float verticalAxis = CrossPlatformInputManager.GetAxis("Vertical"); // Controls forward/backward movement
		float mouseXAxis = CrossPlatformInputManager.GetAxis("Mouse X"); // Controls horizontal camera movement
		float mouseYAxis = CrossPlatformInputManager.GetAxis("Mouse Y"); // Controls vertical camera movement
		float sensitivity = defaultSensitivity;

		if (activeVR == VR.Durovis) {
			// TODO: Complete Durovis VR movement implementation. Looking around already works.
			// Modify Horizontal/Vertical axis values depending on vertical rotation (look at ground = stop/start).
			verticalAxis = Mathf.Cos(Mathf.Deg2Rad*verticalRotation) * 0.4f;
			// Mouse X and Y axes are not used for rotation (head tracking sets the rotation directly with SetRotation).
		} else if (activeVR == VR.Oculus) {
            mouseXAxis = 0;
            mouseYAxis = 0;
            var center = ovrCamera.transform.FindChild("TrackingSpace").transform.FindChild("CenterEyeAnchor");
            SetRotation(center.transform.rotation);
            ovrCamera.transform.position = monoCamera.transform.position;
			// TODO: Complete Oculus Rift movement and looking implementation.
            //verticalAxis = Mathf.Cos(Mathf.Deg2Rad * verticalRotation) * 0.4f;
		} else {
			if (CrossPlatformInputManager.GetActiveInputMethod ().Equals (CrossPlatformInputManager.ActiveInputMethod.Touch)) {
				// Mobile "thumbstick" controls
				// Reduce right stick sensitivity
				sensitivity = defaultSensitivity * 0.3f;
			} else {
				// Keyboard controls
				// Clamp the movement magnitude in a circle instead of both axes separately
				Vector2 movement = Vector2.ClampMagnitude (new Vector2 (horizontalAxis, verticalAxis), 1);
				horizontalAxis = movement.x;
				verticalAxis = movement.y;
			}
		}

		// Control cameras
		Rotate(mouseXAxis, mouseYAxis, sensitivity);

		// Only the axes are modified via the different input methods, the actual move call remains the same.
		Move(horizontalAxis, verticalAxis, defaultMovementSpeed);

		// Check if out of bounds
		if (transform.position.y < -100) JumpToStart ();
	}

	/// <summary>
	/// Switches VR mode. You need to call this every time you change VR mode.
	/// </summary>
	/// <param name="vrMode">Vr mode.</param>
	public void SwitchVRMode(VR vrMode) {
		if (mobileControlRig == null) {
			mobileControlRig = FindObjectOfType<MobileControlRig> ();
		}
		if(vrMode == VR.Durovis) {
			// Disable joysticks
			mobileControlRig.overrideControls = true;
			mobileControlRig.EnableControlRig(false);
#if UNITY_EDITOR
			CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Hardware);
#endif
		} else if(vrMode == VR.Oculus) {
			// Disable joysticks
			//mobileControlRig.overrideControls = true;
            //mobileControlRig.EnableControlRig(false);
            mobileControlRig.overrideControls = false;
#if UNITY_EDITOR
			CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Hardware);
#endif
		} else {
			mobileControlRig.overrideControls = false;
#if MOBILE_INPUT
			mobileControlRig.EnableControlRig(true);
			CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Touch);
#endif
		}
		activeVR = vrMode;
	}

	/// <summary>
	/// Changes the cameras depending on whether or not stereo view is enabled.
	/// </summary>
	/// <param name="stereo">Whether or not stereo cameras should be enabled.</param>
	public void SwitchCameraMode(bool stereo) {
        if (!stereo) {
            activeVR = VR.None;
            ovrCamera.gameObject.SetActive(false);
            monoCamera.gameObject.SetActive(true);
            stereoCameraController.gameObject.SetActive(false);
            mobileControlRig.overrideControls = false;
#if MOBILE_INPUT
            mobileControlRig.EnableControlRig(true);
            CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Touch);
#endif
        }else if (Application.isMobilePlatform) {
            activeVR = VR.Durovis;
            stereoCameraController.gameObject.SetActive(true);
            ovrCamera.gameObject.SetActive(false);
            monoCamera.gameObject.SetActive(false);
            mobileControlRig.overrideControls = true;
            mobileControlRig.EnableControlRig(false);
#if UNITY_EDITOR
            CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Hardware);
#endif
        }else if (Application.isEditor) {
            activeVR = VR.Oculus;
            stereoCameraController.gameObject.SetActive(false);
            ovrCamera.gameObject.SetActive(true);
            monoCamera.gameObject.SetActive(false);
            mobileControlRig.overrideControls = false;
#if UNITY_EDITOR
            CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Hardware);
#endif
        }
        stereoEnabled = stereo;
	}

	/// <summary>
	/// Main rotation method - rotates player and camera.
	/// </summary>
	/// <param name="xAxis">X input axis - horizontal rotation strength between -1 and 1</param>
	/// <param name="yAxis">Y input axis - vertical rotation strength between -1 and 1</param>
	/// <param name="sensitivity">Sensitivity.</param>
	void Rotate(float xAxis, float yAxis, float sensitivity) {
		if (activeVR == VR.Durovis) {
			stereoCameraController.Rotate (xAxis, yAxis, this);
		} else {
			RotateHorizontal(xAxis * sensitivity);
			RotateVertical (-yAxis * sensitivity);
		}
	}

	/// <summary>
	/// Rotates the player horizontally.
	/// </summary>
	/// <param name="amount">Horizontal rotation to add in degrees</param>
	public void RotateHorizontal(float amount) {
		transform.Rotate(0, amount, 0);
	}

	/// <summary>
	/// Rotates camera vertically, relative to current rotation.
	/// </summary>
	/// <param name="amount">Vertical rotation to add in degrees</param>
	public void RotateVertical(float amount) {
		verticalRotation += amount;
		verticalRotation = Mathf.Clamp (verticalRotation, -upDownRange, upDownRange);
		monoCamera.transform.localRotation = Quaternion.Euler (verticalRotation, 0, 0);
		stereoCameraController.transform.localRotation = Quaternion.Euler (verticalRotation, 0, 0);
	}

	/// <summary>
	/// Set absolute rotation. Used for head tracking.
	/// </summary>
	/// <param name="rotation">Rotation.</param>
	public void SetRotation(Quaternion rotation) {
		transform.localRotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
		//verticalRotation = Mathf.Clamp (rotation.eulerAngles.y, -upDownRange, upDownRange);
		verticalRotation = rotation.eulerAngles.x; // No clamp here, we want users to be able to look everywhere!
		monoCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, rotation.eulerAngles.z);
		stereoCameraController.transform.localRotation = Quaternion.Euler (verticalRotation, 0, rotation.eulerAngles.z);
	}

	/// <summary>
	/// Move player relative to current rotation.
	/// </summary>
	/// <param name="xAxis">Sideways movement strength between -1 (full speed left) and 1 (full speed right)</param>
	/// <param name="yAxis">Forward/backward movement strength between - and 1.</param>
	/// <param name="movementSpeed">Maximal movement speed that is reached when xAxis or yAxis is 1 or -1.</param>
	public void Move(float xAxis, float yAxis, float movementSpeed) {
		// Calculate delta time
		float deltaTime = Time.deltaTime;
		if(testMode) deltaTime = 1f / 60f;

		// Horizontal movement
		float forwardSpeed = yAxis * movementSpeed;
		float sideSpeed = xAxis * movementSpeed;

		// Vertical movement
		verticalVelocity += Physics.gravity.y * deltaTime;
		if(characterController.isGrounded && CrossPlatformInputManager.GetButtonDown("Jump") && jumpEnabled) {
			verticalVelocity = jumpSpeed;
		}
		
		Vector3 speed = new Vector3( sideSpeed, verticalVelocity, forwardSpeed );
		speed = transform.rotation * speed;
		
		characterController.Move( speed * deltaTime );
	}

	/// <summary>
	/// Jump to starting position. Used when character falls out of bounds.
	/// </summary>
	void JumpToStart() {
		verticalVelocity = 0;
		transform.position = startingPosition;
	}

	/// <summary>
	/// Simulates the start method (testing mode only)
	/// </summary>
	public void TestStart() {
		if(testMode) Start ();
	}

	/// <summary>
	/// Simulates the update method (testing mode only)
	/// </summary>
	public void TestUpdate() {
		if (testMode) Update ();
	}

	/// <summary>
	/// Tests if the player's Start method has been called yet. For use in the test environment.
	/// </summary>
	/// <returns><c>true</c>, if Start method has been called, <c>false</c> otherwise.</returns>
	public bool HasStarted() {
		return started;
	}
}
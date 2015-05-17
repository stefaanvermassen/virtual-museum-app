using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[TestFixture]
public class WalkingTests {
	
	public WalkingTests() {
	}

	void CreateTestPlatform() {
		GameObject platform = (GameObject) GameObject.Instantiate(Resources.Load("FloorQuad")); // Test platform
		platform.transform.localPosition = new Vector3(0, 0, 0);
		platform.transform.localScale = new Vector3(1, 1, 1);
		platform.transform.Rotate(new Vector3(90, 0, 0));
	}

	[TearDown]
	public void DestroyEverything() {
		var objects = GameObject.FindObjectsOfType<GameObject>();
		foreach (var o in objects) {
			GameObject.DestroyImmediate(o);
		}
	}

	[SetUp]
	public void LoadWalkingController() {
		CreateTestPlatform ();
		GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Prefabs/WalkingController/EventSystem.prefab", typeof(GameObject)));
		GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Prefabs/WalkingController/MobileDualStickControl.prefab", typeof(GameObject)));
		GameObject player = (GameObject)GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Prefabs/WalkingController/Player.prefab", typeof(GameObject)));
		player.transform.localPosition = new Vector3(0, 0, 0);
	}
	
	[Test]
	public void TestEnvironment_LoadObjects_ObjectsLoaded() {
		Assert.NotNull (GameObject.FindObjectOfType<FirstPersonController> (), "First person controller component should be active");
		Assert.NotNull (GameObject.FindObjectOfType<EventSystem> (), "Event system component should be active");
		Assert.NotNull (GameObject.FindObjectOfType<MobileControlRig> (), "Mobile control component rig should be active");
	}

	[Test]
	public void TestEnvironment_StartPlayerInTestMode_PlayerStarted() {
		FirstPersonController player = GameObject.FindObjectOfType<FirstPersonController> ();
		Assert.IsFalse (player.HasStarted(), "Player shouldn't have started yet");
		player.testMode = true;
		player.TestStart ();
		Assert.IsTrue (player.HasStarted(), "Player should have started");
	}

	[Test]
	public void PlayerCharacter_Move_Moved() {
		FirstPersonController player = GameObject.FindObjectOfType<FirstPersonController> ();
		player.testMode = true;
		player.TestStart ();
		for(int i = 0; i < 20; i++) player.TestUpdate();
		Vector3 startPos = player.transform.position;
		for(int i = 0; i < 10; i++) player.Move (0, 1, player.defaultMovementSpeed);
		Assert.AreEqual (player.transform.position.x, startPos.x, "Forward movement: X value should be equal, player has not moved horizontally");
		Assert.AreEqual (player.transform.position.y, startPos.y, "Forward movement: Y value should be equal, player has not moved vertically");
		Assert.Greater (player.transform.position.z, startPos.z, "Forward movement: Z value should be greater, player has moved forward on this axis");
		startPos = player.transform.position;
		for(int i = 0; i < 10; i++) player.Move (-1, 0, player.defaultMovementSpeed);
		Assert.Less(player.transform.position.x, startPos.x, "Sideways movement: X value should be less, player has moved left");
		Assert.AreEqual (player.transform.position.y, startPos.y, "Sideways movement: Y value should be equal, player has not moved vertically");
		Assert.AreEqual (player.transform.position.z, startPos.z, "Sideways movement: Z value should be equal, player has not moved forward");
	}

	[Test]
	public void PlayerCharacter_MoveFall_Falling() {
		FirstPersonController player = GameObject.FindObjectOfType<FirstPersonController> ();
		player.testMode = true;
		player.TestStart ();
		for(int i = 0; i < 30; i++) player.TestUpdate();
		Vector3 startPos = player.transform.position;
		for(int i = 0; i < 40; i++) player.Move (0, 1, player.defaultMovementSpeed);
		startPos = player.transform.position;
		for(int i = 0; i < 5; i++) player.TestUpdate();
		Assert.That(player.transform.position.x, Is.EqualTo(startPos.x).Within(0.005), "Fall: Player character should remain at same horizontal position while falling");
		Assert.Less(player.transform.position.y, startPos.y, "Fall: Player character should be falling down after stepping off test platform");
		Assert.That(player.transform.position.z, Is.EqualTo(startPos.z).Within(0.005), "Fall: Player character should remain at same horizontal position while falling");
	}

	[Test]
	public void PlayerCharacter_MoveFallRespawn_Respawned() {
		FirstPersonController player = GameObject.FindObjectOfType<FirstPersonController> ();
		player.testMode = true;
		player.TestStart ();
		for(int i = 0; i < 30; i++) player.TestUpdate();
		Vector3 startPos = player.transform.position;
		for(int i = 0; i < 60; i++) player.Move (0, 1, player.defaultMovementSpeed);
		for(int i = 0; i < 300; i++) player.TestUpdate();
		Assert.That(player.transform.position.x, Is.EqualTo(startPos.x).Within(0.005), "Respawn: Player character should have respawned on test platform after falling off");
		Assert.That(player.transform.position.y, Is.EqualTo(startPos.y).Within(0.005), "Respawn: Player character should have respawned on test platform after falling off");
		Assert.That(player.transform.position.z, Is.EqualTo(startPos.z).Within(0.005), "Respawn: Player character should have respawned on test platform after falling off");
	}

	[Test]
	public void PlayerCharacter_Rotate_Rotated() {
		FirstPersonController player = GameObject.FindObjectOfType<FirstPersonController> ();
		player.testMode = true;
		player.TestStart ();
		for(int i = 0; i < 20; i++) player.TestUpdate();
		Vector3 startPos = player.transform.position;
		Vector3 startRot = player.transform.localRotation.eulerAngles;
		Camera camera = player.GetComponentInChildren<Camera> ();
		Vector3 cameraRot = camera.transform.localRotation.eulerAngles;
		for(int i = 0; i < 10; i++) player.Move (0, 1, player.defaultMovementSpeed);
		player.RotateHorizontal (180);
		Assert.AreEqual(player.transform.localRotation.eulerAngles.x, startRot.x, "180 Degrees Rotation: X value should be equal");
		Assert.AreNotEqual (player.transform.localRotation.eulerAngles.y, startRot.y, "180 Degrees Rotation: Y value should have changed");
		Assert.AreEqual (player.transform.localRotation.eulerAngles.z, startRot.z, "180 Degrees Rotation: Z value should be equal");
		startRot = player.transform.localRotation.eulerAngles;
		for(int i = 0; i < 10; i++) player.Move (0, 1, player.defaultMovementSpeed);
		Assert.That(player.transform.position.x, Is.EqualTo(startPos.x).Within(0.00001), "Rotation X: Player should have walked back to initial position after turning 180 degrees.");
		Assert.That(player.transform.position.y, Is.EqualTo(startPos.y).Within(0.00001), "Rotation Y: Player should have walked back to initial position after turning 180 degrees.");
		Assert.That(player.transform.position.z, Is.EqualTo(startPos.z).Within(0.00001), "Rotation Z: Player should have walked back to initial position after turning 180 degrees.");
		Assert.AreEqual (camera.transform.localRotation.eulerAngles, cameraRot, "Local camera rotation should not have changed before vertical rotation.");
		player.RotateVertical (50);
		Assert.AreEqual (player.transform.localRotation.eulerAngles, startRot, "Vertical rotation: Player rotation should not have changed");
		Assert.AreNotEqual(camera.transform.localRotation.eulerAngles.x, cameraRot.x, "Vertical Rotation: Camera X value should have changed");
		Assert.AreEqual (camera.transform.localRotation.eulerAngles.y, cameraRot.y, "Vertical Rotation: Camera Y value should be equal");
		Assert.AreEqual (camera.transform.localRotation.eulerAngles.z, cameraRot.z, "Vertical Rotation: Camera Z value should be equal");
		cameraRot = camera.transform.localRotation.eulerAngles;
		Assert.That(cameraRot.x, Is.EqualTo(50).Within(0.005), "Camera should have reached target rotation of 50");
		player.RotateVertical (player.upDownRange+1);
		cameraRot = camera.transform.localRotation.eulerAngles;
		Assert.That (cameraRot.x, Is.EqualTo (player.upDownRange).Within (0.005), "Camera should have rotated only until vertical camera rotation range");
	}

	[Test]
	public void PlayerCharacter_SwitchActiveCamera_CameraSwitched() {
		FirstPersonController player = GameObject.FindObjectOfType<FirstPersonController> ();
		player.testMode = true;
		player.TestStart ();
		player.TestUpdate();
		Camera[] cameras = GameObject.FindObjectsOfType<Camera> ();
		Assert.AreEqual(cameras.Length, 1, "There should only be one camera active.");
		player.CameraMode = FirstPersonController.Cam.StereoDurovis;
		player.TestUpdate ();
		cameras = GameObject.FindObjectsOfType<Camera> ();
		Assert.AreEqual(cameras.Length, 2, "There should be two cameras active in stereoscopic mode.");
		player.CameraMode = FirstPersonController.Cam.Mono;
		player.TestUpdate ();
		cameras = GameObject.FindObjectsOfType<Camera> ();
		Assert.AreEqual(cameras.Length, 1, "There should only be one camera active after switching stereoscopic mode off.");
	}

	/*[Test]
	public void MobileControlRig_MoveStick_AxesChanged() {
		// Activate mobile input
		CrossPlatformInputManager.ActiveInputMethod originalInput = CrossPlatformInputManager.GetActiveInputMethod ();
		CrossPlatformInputManager.SwitchActiveInputMethod (CrossPlatformInputManager.ActiveInputMethod.Touch);
		CrossPlatformInputManager.UnRegisterVirtualAxis ("Vertical");
		CrossPlatformInputManager.UnRegisterVirtualAxis ("Horizontal");
		MobileControlRig rig = GameObject.FindObjectOfType<MobileControlRig> ();
		rig.overrideControls = true;
		rig.EnableControlRig (true);
		// Get left joystick
		Joystick[] joysticks = rig.GetComponentsInChildren<Joystick> ();
		Joystick lj = null;
		foreach(Joystick j in joysticks) {
			if(j.horizontalAxisName.Equals("Horizontal")) {
				lj = j;
				break;
			}
		}
		Assert.NotNull (lj, "Left Joystick should be found");
		lj.Start();
		EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem> ();
		
		PointerEventData pedata = new PointerEventData (eventSystem);
		pedata.position = new Vector2(lj.transform.localPosition.x, lj.transform.localPosition.y + 100);
		lj.OnDrag (pedata); // Drag joystick up
		float verticalAxis = CrossPlatformInputManager.GetAxis("Vertical"); // Controls forward/backward movement
		Assert.AreNotEqual(verticalAxis, 0, "Axis should have changed from touch event");
		lj.OnPointerUp (pedata); // Release joystick
		verticalAxis = CrossPlatformInputManager.GetAxis("Vertical");
		Assert.That(verticalAxis, Is.EqualTo(0).Within(0.00005), "Axis should be back to normal after releasing it");

		pedata = new PointerEventData(eventSystem);
		pedata.position = new Vector2(lj.transform.localPosition.x - 100, lj.transform.localPosition.y);
		lj.OnDrag (pedata); // Drag joystick left
		float horizontalAxis = CrossPlatformInputManager.GetAxis("Horizontal"); // Controls sideways movement
		Assert.AreNotEqual(horizontalAxis, 0, "Axis should have changed from touch event");
		lj.OnPointerUp (pedata); // Release joystick
		horizontalAxis = CrossPlatformInputManager.GetAxis("Horizontal");
		Assert.That(horizontalAxis, Is.EqualTo(0).Within(0.00005), "Axis should be back to normal after releasing it");

		//CrossPlatformInputManager.ActiveInputMethod originalInput = CrossPlatformInputManager.GetActiveInputMethod ();
		CrossPlatformInputManager.UnRegisterVirtualAxis ("Vertical");
		CrossPlatformInputManager.UnRegisterVirtualAxis ("Horizontal");
		CrossPlatformInputManager.SwitchActiveInputMethod (CrossPlatformInputManager.ActiveInputMethod.Hardware);
	}

	[Test]
	public void MobileControlRig_MoveLeftStick_PlayerMoved() {
		// Init player, stabilize and get start position
		FirstPersonController player = GameObject.FindObjectOfType<FirstPersonController> ();
		player.testMode = true;
		player.TestStart ();
		for(int i = 0; i < 20; i++) player.TestUpdate();
		Vector3 startPos = player.transform.position;

		// Activate mobile input
		CrossPlatformInputManager.ActiveInputMethod originalInput = CrossPlatformInputManager.GetActiveInputMethod ();
		CrossPlatformInputManager.SwitchActiveInputMethod (CrossPlatformInputManager.ActiveInputMethod.Touch);
		CrossPlatformInputManager.UnRegisterVirtualAxis ("Vertical");
		CrossPlatformInputManager.UnRegisterVirtualAxis ("Horizontal");
		MobileControlRig rig = GameObject.FindObjectOfType<MobileControlRig> ();
		rig.overrideControls = true;
		rig.EnableControlRig (true);
		// Get left joystick
		Joystick[] joysticks = rig.GetComponentsInChildren<Joystick> ();
		Joystick lj = null;
		foreach(Joystick j in joysticks) {
			if(j.horizontalAxisName.Equals("Horizontal")) {
				lj = j;
				break;
			}
		}
		Assert.NotNull (lj, "Left Joystick should be found");
		lj.Start();
		EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem> ();


		PointerEventData pedata = new PointerEventData (eventSystem);
		pedata.position = new Vector2(lj.transform.localPosition.x, lj.transform.localPosition.y + 100);
		lj.OnDrag (pedata); // Drag joystick up
		for(int i = 0; i < 10; i++) player.TestUpdate ();
		Assert.AreEqual (startPos.x, player.transform.position.x, "Forward movement: X value should be equal, player has not moved horizontally");
		Assert.AreEqual (startPos.y, player.transform.position.y, "Forward movement: Y value should be equal, player has not moved vertically");
		Assert.Greater (player.transform.position.z, startPos.z, "Forward movement: Z value should be greater, player has moved forward on this axis");
		lj.OnPointerUp (pedata); // Release joystick
		for(int i = 0; i < 10; i++) player.TestUpdate (); // Stabilize position
		startPos = player.transform.position;


		pedata = new PointerEventData(eventSystem);
		pedata.position = new Vector2(lj.transform.localPosition.x - 100, lj.transform.localPosition.y);
		lj.OnDrag (pedata); // Drag joystick left
		for(int i = 0; i < 10; i++) player.TestUpdate ();
		Assert.Less(player.transform.position.x, startPos.x, "Sideways movement: X value should be less, player has moved left");
		Assert.AreEqual (startPos.y, player.transform.position.y, "Sideways movement: Y value should be equal, player has not moved vertically");
		Assert.AreEqual (startPos.z, player.transform.position.z, "Sideways movement: Z value should be equal, player has not moved forward");
		//CrossPlatformInputManager.ActiveInputMethod originalInput = CrossPlatformInputManager.GetActiveInputMethod ();
		CrossPlatformInputManager.UnRegisterVirtualAxis ("Vertical");
		CrossPlatformInputManager.UnRegisterVirtualAxis ("Horizontal");
		CrossPlatformInputManager.SwitchActiveInputMethod (CrossPlatformInputManager.ActiveInputMethod.Hardware);
	}

	[Test]
	public void MobileControlRig_MoveRightStick_PlayerAndCameraRotated() {
		// Init player, stabilize and get start position
		FirstPersonController player = GameObject.FindObjectOfType<FirstPersonController> ();
		player.testMode = true;
		player.TestStart ();
		for(int i = 0; i < 20; i++) player.TestUpdate();
		Vector3 startPos = player.transform.position;
		Vector3 startRot = player.transform.localRotation.eulerAngles;
		Camera camera = player.GetComponentInChildren<Camera> ();
		Vector3 cameraRot = camera.transform.localRotation.eulerAngles;
		
		// Activate mobile input
		CrossPlatformInputManager.ActiveInputMethod originalInput = CrossPlatformInputManager.GetActiveInputMethod ();
		CrossPlatformInputManager.SwitchActiveInputMethod (CrossPlatformInputManager.ActiveInputMethod.Touch);
		CrossPlatformInputManager.UnRegisterVirtualAxis ("Mouse Y");
		CrossPlatformInputManager.UnRegisterVirtualAxis ("Mouse X");
		MobileControlRig rig = GameObject.FindObjectOfType<MobileControlRig> ();
		rig.overrideControls = true;
		rig.EnableControlRig (true);
		// Get right joystick
		Joystick[] joysticks = rig.GetComponentsInChildren<Joystick> ();
		Joystick rj = null;
		foreach(Joystick j in joysticks) {
			if(j.horizontalAxisName.Equals("Mouse X")) {
				rj = j;
				break;
			}
		}
		Assert.NotNull (rj, "Right Joystick should be found");
		rj.Start();
		EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem> ();
		
		PointerEventData pedata = new PointerEventData (eventSystem);
		pedata.position = new Vector2(rj.transform.localPosition.x, rj.transform.localPosition.y + 100);
		rj.OnDrag (pedata); // Drag joystick up
		float verticalAxis = CrossPlatformInputManager.GetAxis("Mouse Y"); // Controls vertical camera rotation
		Assert.AreNotEqual(verticalAxis, 0, "Vertical axis should have changed from touch event");
		//Assert.IsNull (pedata, "Axes are: " + horizontalAxis + " " + verticalAxis);
		for(int i = 0; i < 10; i++) player.TestUpdate ();
		Assert.AreNotEqual(camera.transform.localRotation.eulerAngles.x, cameraRot.x, "Vertical Rotation: Camera X value should have changed");
		Assert.AreEqual (camera.transform.localRotation.eulerAngles.y, cameraRot.y, "Vertical Rotation: Camera Y value should be equal");
		Assert.AreEqual (camera.transform.localRotation.eulerAngles.z, cameraRot.z, "Vertical Rotation: Camera Z value should be equal");
		rj.OnPointerUp (pedata); // Release joystick
		for(int i = 0; i < 10; i++) player.TestUpdate (); // Stabilize position
		startPos = player.transform.position;

		pedata = new PointerEventData(eventSystem);
		pedata.position = new Vector2(rj.transform.localPosition.x - 100, rj.transform.localPosition.y);
		rj.OnDrag (pedata); // Drag joystick left
		float horizontalAxis = CrossPlatformInputManager.GetAxis("Mouse X"); // Controls horizontal camera rotation
		Assert.AreNotEqual(horizontalAxis, 0, "Horizontal axis should have changed from touch event");
		for(int i = 0; i < 10; i++) player.TestUpdate ();
		Assert.AreEqual(player.transform.localRotation.eulerAngles.x, startRot.x, "180 Degrees Rotation: X value should be equal");
		Assert.AreNotEqual (player.transform.localRotation.eulerAngles.y, startRot.y, "180 Degrees Rotation: Y value should have changed");
		Assert.AreEqual (player.transform.localRotation.eulerAngles.z, startRot.z, "180 Degrees Rotation: Z value should be equal");
		//CrossPlatformInputManager.ActiveInputMethod originalInput = CrossPlatformInputManager.GetActiveInputMethod ();
		CrossPlatformInputManager.UnRegisterVirtualAxis ("Mouse Y");
		CrossPlatformInputManager.UnRegisterVirtualAxis ("Mouse X");
		CrossPlatformInputManager.SwitchActiveInputMethod (CrossPlatformInputManager.ActiveInputMethod.Hardware);
	}*/
}
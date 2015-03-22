using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections.Generic;

[TestFixture]
public class WalkingTests {
	
	private static int TEST_CASES = 10;
	private static int SEED = 1234;
	
	public WalkingTests() {
		UnityEngine.Random.seed = SEED;
	}
	
	int RandomInt(int from, int until) {
		return (int)(Random.value * (from+until) - from);
	}
	
	string RandomString(int minLength, int maxLength) {
		string s = "";
		int length = RandomInt(minLength, maxLength);
		for (int i = 0; i < length; i++) {
			char c = (char)RandomInt(0, 255);
			s += c;
		}
		return s;
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
		foreach (var o in objects) GameObject.DestroyImmediate(o);
	}

	[SetUp]
	public void LoadWalkingController() {
		CreateTestPlatform ();
		GameObject player = (GameObject)MonoBehaviour.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Prefabs/WalkingController/Player.prefab", typeof(GameObject)));
		player.transform.localPosition = new Vector3(0, 0, 0);
		//GameObject eventSystem = (GameObject)MonoBehaviour.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Prefabs/WalkingController/EventSystem.prefab", typeof(GameObject)));
		//GameObject dualSticks = (GameObject)MonoBehaviour.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Prefabs/WalkingController/MobileDualStickControl.prefab", typeof(GameObject)));
	}
	
	[Test]
	public void TestEnvironment_ObjectsLoaded() {
		Assert.NotNull (GameObject.FindObjectOfType<FirstPersonController> (), "First person controller component should be active");
		//Assert.NotNull (GameObject.FindObjectOfType<EventSystem> (), "Event system component should be active");
		//Assert.NotNull (GameObject.FindObjectOfType<MobileControlRig> (), "Mobile control component rig should be active");
	}

	[Test]
	public void TestEnvironment_PlayerTestMode() {
		FirstPersonController player = GameObject.FindObjectOfType<FirstPersonController> ();
		Assert.IsFalse (player.started, "Player shouldn't have started yet");
		player.testMode = true;
		player.TestStart ();
		Assert.IsTrue (player.started, "Player should have started");
	}

	[Test]
	public void PlayerCharacter_Move() {
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
	public void PlayerCharacter_Respawn() {
		FirstPersonController player = GameObject.FindObjectOfType<FirstPersonController> ();
		player.testMode = true;
		player.TestStart ();
		for(int i = 0; i < 20; i++) player.TestUpdate();
		Vector3 startPos = player.transform.position;
		for(int i = 0; i < 60; i++) player.Move (0, 1, player.defaultMovementSpeed);
		for(int i = 0; i < 300; i++) player.TestUpdate();
		Assert.AreEqual (player.transform.position, startPos, "Respawn: Player character should have respawned on test platform after falling off");
	}

	[Test]
	public void PlayerCharacter_Rotate() {
		FirstPersonController player = GameObject.FindObjectOfType<FirstPersonController> ();
		player.testMode = true;
		player.TestStart ();
		for(int i = 0; i < 20; i++) player.TestUpdate();
		Vector3 startPos = player.transform.position;
		Vector3 startRot = player.transform.localRotation.eulerAngles;
		for(int i = 0; i < 10; i++) player.Move (0, 1, player.defaultMovementSpeed);
		player.RotateHorizontal (180);
		for(int i = 0; i < 10; i++) player.Move (0, 1, player.defaultMovementSpeed);
		Assert.That(player.transform.position.x, Is.EqualTo(startPos.x).Within(0.00001), "Rotation X: Player should have walked back to initial position after turning 180 degrees.", 0.1);
		Assert.That(player.transform.position.y, Is.EqualTo(startPos.y).Within(0.00001), "Rotation Y: Player should have walked back to initial position after turning 180 degrees.", 0.1);
		Assert.That(player.transform.position.z, Is.EqualTo(startPos.z).Within(0.00001), "Rotation Z: Player should have walked back to initial position after turning 180 degrees.", 0.1);
		player.RotateVertical (50);
		Assert.AreEqual
	}

	[Test]
	public void PlayerCharacter_CameraSwitch() {
		FirstPersonController player = GameObject.FindObjectOfType<FirstPersonController> ();
		player.testMode = true;
		player.TestStart ();
		player.TestUpdate();
		Camera[] cameras = GameObject.FindObjectsOfType<Camera> ();
		Assert.AreEqual(cameras.Length, 1, "There should only be one camera active.");
		player.stereoEnabled = true;
		player.TestUpdate ();
		cameras = GameObject.FindObjectsOfType<Camera> ();
		Assert.AreEqual(cameras.Length, 2, "There should be two cameras active in stereoscopic mode.");
		player.stereoEnabled = false;
		player.TestUpdate ();
		cameras = GameObject.FindObjectsOfType<Camera> ();
		Assert.AreEqual(cameras.Length, 1, "There should only be one camera active after switching stereoscopic mode off.");
	}

	//[Test]
	//public void PlayerCharacter_
}
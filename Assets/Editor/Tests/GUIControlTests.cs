using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using NUnit.Framework;

[TestFixture]

public class GUIControlTests
{
	private static int SEED = 123;
	private static int TEST_CASES = 10;

	public GUIControlTests ()
	{
		UnityEngine.Random.seed = SEED;
	}

	int RandomInt (int from, int until)
	{
		return (int)(UnityEngine.Random.value * (from + until) - from);
	}

	float RandomFloat (float from, float until)
	{
		return (float)(UnityEngine.Random.value * (from + until) - from);
	}

	void DestroyEverything ()
	{
		var objects = GameObject.FindObjectsOfType<GameObject> ();
		foreach (var o in objects)
			GameObject.DestroyImmediate (o);
	}

	[Test]
	public void GUIControlPositioning_ChangerelativePosition_PositionChanged ()
	{
		GUIControl test = createBasicGUIControl ();
		for (int i = 0; i < TEST_CASES; i++) {
			float x = RandomFloat (-1000, 1000);
			float y = RandomFloat (-1000, 1000);
			test.SetRelativePosition (x, y);
			Assert.AreEqual (test.GetRelativeX (), x, "Relative x position should be correctly set to " + x + " but it's " + test.GetRelativeX ());
			Assert.AreEqual (test.GetRelativeY (), y, "Relative y position should be correctly set to " + y + " but it's " + test.GetRelativeY ());
		}
		DestroyEverything ();
	}

	[Test]
	public void GUIControlPositioning_DoubleReplace_PositionDidNotChange ()
	{
		GUIControl test1 = createBasicGUIControl ();
		GUIControl test2 = createBasicGUIControl ();

		for (int i = 0; i < TEST_CASES; i++) {
			float x = test1.GetRelativeX ();
			float y = test1.GetRelativeY ();
			test1.Replace (test2);
			test1.Replace (test2);
			Assert.AreEqual (test1.GetRelativeX (), x, "Relative x position should be correctly set to " + x + " but it's " + test1.GetRelativeX ());
			Assert.AreEqual (test1.GetRelativeY (), y, "Relative y position should be correctly set to " + y + " but it's " + test1.GetRelativeY ());
		}
		DestroyEverything ();
	}

	[Test]
	public void GUIControlVisualState_OpenAndClose_ControlStateIsCloses ()
	{
		GUIControl test1 = createBasicGUIControl ();

		for (int i = 0; i < TEST_CASES; i++) {
			test1.Open ();
			test1.Close ();
			Assert.AreEqual (test1.IsOpen (), false, "The open state should be " + false + " but it's " + test1.IsOpen ());
		}
		DestroyEverything ();
	}

	[Test]
	public void GUIControlVisualState_FlipOpenClose_ControlStateDidNotChange ()
	{
		GUIControl test1 = createBasicGUIControl ();
		
		for (int i = 0; i < TEST_CASES; i++) {
			bool state = test1.IsOpen ();
			test1.FlipCloseOpen ();
			test1.FlipCloseOpen ();
			Assert.AreEqual (test1.IsOpen (), state, "The open state should be " + state + " but it's " + test1.IsOpen ());
		}
		DestroyEverything ();
	}

	[Test]
	public void GUIControlChildren_Add_ControlHasChildIndex0 ()
	{
		GUIControl test1 = createBasicGUIControl ();
		GUIControl test2 = createBasicGUIControl ();

		for (int i = 0; i < TEST_CASES; i++) {
			test1.Add (test2);
			Assert.AreEqual (test1.GetChild (0), test2, "The first child of the control should be " + test2 + " but it's " + test1.GetChild (0));
		}
		DestroyEverything ();
	}

	[Test]
	public void GUIControlChildren_RemoveAllChildren_ControlHasNoChildIndex0 ()
	{
		GUIControl test1 = createBasicGUIControl ();
		GUIControl test2;
		for (int i = 0; i < TEST_CASES; i++) {
			test2 = createBasicGUIControl ();
			test1.Add (test2);
			test1.RemoveAllChildren ();
			Assert.IsTrue (test1.GetChild (0)== null, "The first child of the control should be null but it's " + test1.GetChild (0));
		}
		DestroyEverything ();
	}

	[Test]
	public void GUIControlInitialisation_InitByCopy_InitialisedControlIsNotNull ()
	{
		GUIControl test1 = createBasicGUIControl ();

		for (int i = 0; i < TEST_CASES; i++) {
			GUIControl test2 = GUIControl.Init (test1);
			Assert.IsTrue (test2 != null, "Initialised GUIControl should be differen from null");

		}
		DestroyEverything ();
	}



	private GUIControl createBasicGUIControl ()
	{
		GameObject ob = new GameObject ();
		return ob.AddComponent<GUIControl> ();
	}
}



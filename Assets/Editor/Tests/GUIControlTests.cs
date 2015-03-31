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
			test.setRelativePosition (x, y);
			Assert.AreEqual (test.getRelativeX (), x, "Relative x position should be correctly set to " + x + " but it's " + test.getRelativeX ());
			Assert.AreEqual (test.getRelativeY (), y, "Relative y position should be correctly set to " + y + " but it's " + test.getRelativeY ());
		}
		DestroyEverything ();
	}

	[Test]
	public void GUIControlPositioning_DoubleReplace_PositionDidNotChange ()
	{
		GUIControl test1 = createBasicGUIControl ();
		GUIControl test2 = createBasicGUIControl ();

		for (int i = 0; i < TEST_CASES; i++) {
			float x = test1.getRelativeX ();
			float y = test1.getRelativeY ();
			test1.replace (test2);
			test1.replace (test2);
			Assert.AreEqual (test1.getRelativeX (), x, "Relative x position should be correctly set to " + x + " but it's " + test1.getRelativeX ());
			Assert.AreEqual (test1.getRelativeY (), y, "Relative y position should be correctly set to " + y + " but it's " + test1.getRelativeY ());
		}
		DestroyEverything ();
	}

	[Test]
	public void GUIControlVisualState_OpenAndClose_ControlStateIsCloses ()
	{
		GUIControl test1 = createBasicGUIControl ();

		for (int i = 0; i < TEST_CASES; i++) {
			test1.open ();
			test1.close ();
			Assert.AreEqual (test1.isOpen (), false, "The open state should be " + false + " but it's " + test1.isOpen ());
		}
		DestroyEverything ();
	}

	[Test]
	public void GUIControlVisualState_FlipOpenClose_ControlStateDidNotChange ()
	{
		GUIControl test1 = createBasicGUIControl ();
		
		for (int i = 0; i < TEST_CASES; i++) {
			bool state = test1.isOpen ();
			test1.flipCloseOpen ();
			test1.flipCloseOpen ();
			Assert.AreEqual (test1.isOpen (), state, "The open state should be " + state + " but it's " + test1.isOpen ());
		}
		DestroyEverything ();
	}

	[Test]
	public void GUIControlChildren_Add_ControlHasChildIndex0 ()
	{
		GUIControl test1 = createBasicGUIControl ();
		GUIControl test2 = createBasicGUIControl ();

		for (int i = 0; i < TEST_CASES; i++) {
			test1.add (test2);
			Assert.AreEqual (test1.getChild (0), test2, "The first child of the control should be " + test2 + " but it's " + test1.getChild (0));
		}
		DestroyEverything ();
	}

	[Test]
	public void GUIControlChildren_RemoveAllChildren_ControlHasNoChildIndex0 ()
	{
		GUIControl test1 = createBasicGUIControl ();
		GUIControl test2 = createBasicGUIControl ();
		for (int i = 0; i < TEST_CASES; i++) {
			test1.add (test2);
			test1.removeAllChildren ();
			Assert.IsTrue (test1.getChild (0)== null, "The first child of the control should be null but it's " + test1.getChild (0));
		}
		DestroyEverything ();
	}

	[Test]
	public void GUIControlInitialisation_InitByCopy_InitialisedControlIsNotNull ()
	{
		GUIControl test1 = createBasicGUIControl ();

		for (int i = 0; i < TEST_CASES; i++) {
			GUIControl test2 = GUIControl.init (test1);
			Assert.IsTrue (test2 != null, "Initialised GUIControl should be differen from null");

		}
		DestroyEverything ();
	}

	[Test]
	public void GUIControlInitialisation_InitByEnum_InitialisedControlIsNotNull ()
	{

		for (int i = 0; i < TEST_CASES; i++) {
			Array values = Enum.GetValues (typeof(GUIControl.types));
			GUIControl.types type = (GUIControl.types) values.GetValue (RandomInt (0, values.Length - 1));
			GUIControl test1 = GUIControl.init (type);
		
			Assert.IsTrue (test1 != null, "Initialised GUIControl should be differen from null");
		}
		DestroyEverything ();

	}

	private GUIControl createBasicGUIControl ()
	{
		GameObject ob = new GameObject ();
		return ob.AddComponent<GUIControl> ();

	}
}



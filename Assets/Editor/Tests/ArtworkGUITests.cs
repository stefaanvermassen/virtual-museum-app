using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

[TestFixture]

public class ArtworkGUITests
{
	private static int SEED = 123;
	private static int TEST_CASES = 10;


	public ArtworkGUITests ()
	{
		Random.seed = SEED;
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
	
	void DestroyEverything() {
		var objects = GameObject.FindObjectsOfType<GameObject>();
		foreach (var o in objects) GameObject.DestroyImmediate(o);
	}
	[Test]
	public void ChangingArtworkInformation_ChangingArtworkdName_NameChanged() {
		GameObject ob = new GameObject();
		ArtworkGUI artwork = ob.AddComponent<ArtworkGUI>();
		for (int i = 0; i < TEST_CASES; i++) {
			var name = RandomString(1, 100);
			artwork.Name = name;
			Assert.AreEqual(artwork.Name, name, "Name should be correctly set to "+name+" but it's "+artwork.Name);
		}
		DestroyEverything();
	}
	//TODO test all property changing, test fixed image if artwork has an id originating from server
}



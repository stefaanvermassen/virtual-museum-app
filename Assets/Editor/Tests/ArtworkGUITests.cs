using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

[TestFixture]

public class ArtworkGUITests
{
	private static int SEED = 123;

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
}



using UnityEngine;
using System.Collections;
using System;

using UnityEngine.UI;

public class BasicActions : MonoBehaviour {

	void Start () {}

	public void startGame()
	{
		Application.LoadLevel("BuildMuseum");
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
	}
}

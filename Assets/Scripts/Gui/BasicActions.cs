using UnityEngine;
using System.Collections;
using System;

using UnityEngine.UI;

public class BasicActions : MonoBehaviour {

	void Start () {}

	public void StartGame()
	{
		Application.LoadLevel("BuildMuseum");
	}
	public void ShowtestMessage(){
		PopUpWindow.ShowMessage (PopUpWindow.MessageType.INFO, "This is a test info");
	}

	public void Exit() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
		Application.Quit();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Exit ();
		}
	}
}

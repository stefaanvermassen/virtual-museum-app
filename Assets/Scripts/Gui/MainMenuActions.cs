using UnityEngine;
using System.Collections;
using System;

using UnityEngine.UI;

public class MainMenuActions : MonoBehaviour {

	void Start () {}

	public void NewMuseum() {
		MuseumLoader.currentAction = MuseumLoader.MuseumAction.Preview;
		Application.LoadLevel("BuildMuseum");
	}

	public void EditMuseum(int museumID) {
		MuseumLoader.museumID = museumID;
		MuseumLoader.currentAction = MuseumLoader.MuseumAction.Edit;
		Application.LoadLevel ("BuildMuseum");
	}

	/// <summary>
	/// Visit the museum with the designated museum ID
	/// </summary>
	/// <param name="museumID">Museum ID</param>
	/// <param name="owner">Whether or not the current user is the owner of this museum</param>
	public void VisitMuseum(int museumID, bool owner) {
		MuseumLoader.museumID = museumID;
		if (owner) {
			MuseumLoader.currentAction = MuseumLoader.MuseumAction.Edit;
		} else {
			MuseumLoader.currentAction = MuseumLoader.MuseumAction.Visit;
		}
		Application.LoadLevel ("WalkingController");
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

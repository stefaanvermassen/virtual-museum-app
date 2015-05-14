using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class MuseumSavePanel : MonoBehaviour {

	public InputField title;
	public InputField description;
	public GUIControl savePopUp;
	public ImageHighlightButton saveButton;
	public Museum museum;
	public bool exitOnSaved;
	public BuildMuseumActions actions;
	public WalkingActions walkingActions;

	void OnEnable() {
		title.text = (museum.museumName == null ? "" : museum.museumName);
		description.text = (museum.description == null ? "" : museum.description);
		bool active = (title.text.Length > 0 && description.text.Length > 0);
		saveButton.gameObject.SetActive (active);
	}

	void Update () {
		bool active = (title.text.Length > 0 && description.text.Length > 0);
		saveButton.gameObject.SetActive (active);
	}

	public void Save() {
		museum.museumName = title.text;
		museum.description = description.text;
		//Storage.Instance.SaveRemote(museum);
		museum.SaveRemote (OnSaved);
		savePopUp.Close ();
	}

	public void OnSaved(object sender, EventArgs e) {
		if(Application.loadedLevelName.Equals("WalkingController")) {
			walkingActions.OnSaved();
		}
		if (exitOnSaved) {
			if(Application.loadedLevelName.Equals("BuildMuseum")) {
				actions.BackToMain ();
			} else {
				walkingActions.BackToMain();
			}
		} else {
			//Toast.print ("Museum was successfully saved");
		}
	}

	public void SetExitOnSaved(bool exit) {
		exitOnSaved = exit;
	}
}

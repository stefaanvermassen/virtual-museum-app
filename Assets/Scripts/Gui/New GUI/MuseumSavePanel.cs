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
		if(Application.loadedLevelName.Equals("BuildMuseum")) {
			Debug.Log("credits");
			var cc = API.CreditController.Instance;
	        var creditmodel = new API.CreditModel(){ Action = API.CreditActions.EDITEDMUSEUM};
	        cc.AddCredit(creditmodel, (info) => {
				Debug.Log("credits credits cred");
	            if (info.CreditsAdded) { // check if credits are added
	                actions.toast.Notify("Thank you for editing your museum. Your total amount of tokens is: " + info.Credits);
	            } else {
	                Debug.Log("No tokens added for new build museum action.");
	            }
	        }, (error) => {
	            Debug.Log("An error occured when adding tokens for the user.");
	        });
		}
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

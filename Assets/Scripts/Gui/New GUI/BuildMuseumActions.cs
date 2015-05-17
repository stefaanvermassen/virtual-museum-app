using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class BuildMuseumActions : StatisticsBehaviour {

	public int tool;
	public ImageHighlightButton[] toolButtons;
	public DrawController drawController;
	public Museum museum;
	public ImageHighlightButton artBackButton;
	public ImageHighlightButton objectBackButton;
	public GUIControl artCollectionPopUp;
	public GUIControl savePopUp;
	public GUIControl objectsPopUp;
	public GUIControl stylesPopUp;
	public GUIControl colorPopUp;
	public Toast toast;
	public bool canScroll;

	void Start() {
		SetTool (1); // Pan tool
		if (MuseumLoader.museumID == -1) {
			var cc = API.CreditController.Instance;
			var newbuildcreditmodel = new API.CreditModel(){ Action = API.CreditActions.BUILDEDMUSEUM};
			cc.AddCredit(newbuildcreditmodel, (info) => {
				if (info.CreditsAdded) { // check if credits are added
					toast.Notify("Thank you for creating a new museum. Your total amount of tokens is: " + info.Credits);
				} else {
					Debug.Log("No tokens added for new build museum action.");
				}
			}, (error) => {
				Debug.Log("An error occured when adding tokens for the user.");
			});
		}
		StartStatistics("Build museum");
	}

	public void BackToMain() {
		Application.LoadLevel ("MainMenuScene");
	}

	void Update() {
		tool = (int)drawController.tool;
		if (Input.GetKeyDown(KeyCode.Escape)) {
			BackToMain();
		}
		for (int i = 0; i < toolButtons.Length; i++) {
			if(tool == i) {
				toolButtons[i].Highlight(true);
			} else {
				toolButtons[i].Highlight(false);
			}
		}
		/*if (DrawController.currentArt < 0) {
			artBackButton.gameObject.SetActive (false);
		} else {
			artBackButton.gameObject.SetActive (true);
		}
		if (DrawController.currentObject < 0) {
			objectBackButton.gameObject.SetActive (false);
		} else {
			objectBackButton.gameObject.SetActive (true);
		}*/
		if (DrawController.currentArt < 0 && tool == (int)DrawController.Tools.PlacingArt && !artCollectionPopUp.IsOpen ()) {
			SetTool ((int)DrawController.Tools.Moving);
		} else if (DrawController.currentObject < 0 && tool == (int)DrawController.Tools.PlacingObject && !objectsPopUp.IsOpen ()) {
			SetTool ((int)DrawController.Tools.Moving);
		}
		if (Input.GetKeyDown (KeyCode.Return)) {
			artCollectionPopUp.Close ();
			objectsPopUp.Close();
			if(colorPopUp.IsOpen ()) {
				colorPopUp.Close();
			} else {
				stylesPopUp.Close ();
			}
			/*if(artBackButton.gameObject.activeSelf) {
				artCollectionPopUp.Close ();
			}*/
		}
		if (artCollectionPopUp.IsOpen () || savePopUp.IsOpen () || objectsPopUp.IsOpen () || stylesPopUp.IsOpen()) {
			canScroll = false;
		} else {
			canScroll = true;
		}
	}

	public void SetTool(int tool) {
		this.tool = tool;
		drawController.SetTool(tool);
	}

	public void SetArt(int artID) {
		drawController.SetCurrentArt (artID);
	}

	public void SetObject(int objectID) {
		drawController.SetCurrentObject (objectID);
		objectsPopUp.Close ();
	}

	public void SetWall(int wallID) {
		drawController.SetCurrentWall (wallID);
	}

	public void SetFrame(int frameID) {
		drawController.SetCurrentFrame (frameID);
	}

	public void SetCeiling(int ceilingID) {
		drawController.SetCurrentCeiling (ceilingID);
	}

	public void SetFloor(int floorID) {
		drawController.SetCurrentFloor (floorID);
	}

	public void SetWallColor(Color color) {
		DrawController.currentWallColor = color;
	}
	
	public void SetCeilingColor(Color color) {
		DrawController.currentCeilingColor = color;
	}
	
	public void SetFloorColor(Color color) {
		DrawController.currentFloorColor = color;
	}

	public int GetArt() {
		return DrawController.currentArt;
	}

	public int GetObject() {
		return DrawController.currentObject;
	}

	public int GetFloor() {
		return DrawController.currentFloor;
	}

	public int GetWall() {
		return DrawController.currentWall;
	}

	public int GetCeiling() {
		return DrawController.currentCeiling;
	}

	public int GetFrame() {
		return DrawController.currentFrame;
	}

	public Color GetFloorColor() {
		return DrawController.currentFloorColor;
	}
	
	public Color GetWallColor() {
		return DrawController.currentWallColor;
	}
	
	public Color GetCeilingColor() {
		return DrawController.currentCeilingColor;
	}

	public void Preview() {
		MuseumLoader.CreateTempMuseum (museum);
		Application.LoadLevel("WalkingController");
	}
}

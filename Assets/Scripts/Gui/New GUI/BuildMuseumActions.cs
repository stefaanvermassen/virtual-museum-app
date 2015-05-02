using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class BuildMuseumActions : MonoBehaviour {

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
	public bool canScroll;

	void Start() {
		SetTool (1); // Pan tool
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
				//toolButtons[i].Select();
			} else {
				toolButtons[i].Highlight(false);
			}
		}
		if (drawController.currentArt < 0) {
			artBackButton.gameObject.SetActive (false);
		} else {
			artBackButton.gameObject.SetActive (true);
		}
		if (drawController.currentObject < 0) {
			objectBackButton.gameObject.SetActive (false);
		} else {
			objectBackButton.gameObject.SetActive (true);
		}
		if (Input.GetKeyDown (KeyCode.Return)) {
			if(artBackButton.gameObject.activeSelf) {
				artCollectionPopUp.Close ();
			}
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

	public int GetArt() {
		return drawController.currentArt;
	}

	public int GetObject() {
		return drawController.currentObject;
	}

	public int GetFloor() {
		return drawController.currentFloor;
	}

	public int GetWall() {
		return drawController.currentWall;
	}

	public int GetCeiling() {
		return drawController.currentCeiling;
	}

	public int GetFrame() {
		return drawController.currentFrame;
	}

	public void Preview() {
		MuseumLoader.CreateTempMuseum (museum);
		Application.LoadLevel("WalkingController");
	}
}

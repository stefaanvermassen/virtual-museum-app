using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WalkingModeFunctions : MonoBehaviour {

	public FirstPersonController player;

	public void TempSwitchVR() {
		if(player.ActiveVR == FirstPersonController.VR.None) {
			player.ActiveVR = FirstPersonController.VR.Durovis;
			player.StereoEnabled = true;
		} else {
			player.ActiveVR = FirstPersonController.VR.None;
			player.StereoEnabled = false;
		}
	}

	public void TempBack() {
		Application.LoadLevel("BuildMuseum");
	}
}

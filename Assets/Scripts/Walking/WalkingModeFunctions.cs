using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WalkingModeFunctions : MonoBehaviour {

	public FirstPersonController player;

	public void TempSwitchVR() {
        if (player.ActiveVR != FirstPersonController.VR.None) {
            player.SwitchCameraMode(false);
		} else {
            player.SwitchCameraMode(true);
		}
	}

	public void TempBack() {
		Application.LoadLevel("BuildMuseum");
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.V)) {
            TempSwitchVR();
        }
    }
}

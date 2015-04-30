using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WalkingModeFunctions : MonoBehaviour {

	public FirstPersonController player;

	public void TempSwitchVR() {
        if (player.ActiveVR == FirstPersonController.VR.None) {
#if MOBILE_INPUT
			player.ActiveVR = FirstPersonController.VR.Durovis;
			player.CameraMode = FirstPersonController.Cam.StereoDurovis;
#else
			player.ActiveVR = FirstPersonController.VR.Oculus;
			player.CameraMode = FirstPersonController.Cam.StereoOculus;
#endif
		} else {
			player.ActiveVR = FirstPersonController.VR.None;
			player.CameraMode = FirstPersonController.Cam.Mono;
		}
	}

	public void TempBack() {
		if (MuseumLoader.currentAction == MuseumLoader.MuseumAction.Visit) {
			Application.LoadLevel ("MainMenuScene");
		} else {
			Application.LoadLevel ("BuildMuseum");
		}
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.V)) {
            TempSwitchVR();
        }
		if (Input.GetKeyDown(KeyCode.Escape)) {
			TempBack ();
		}
    }
}

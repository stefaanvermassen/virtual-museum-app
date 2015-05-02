using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class WalkingActions : MonoBehaviour {

	public FirstPersonController player;
	public Museum museum;
	public bool VRActive = false;

	public GUIControl UIMobile;
	public GUIControl UIDesktop;
	public GUIControl mobileSettingsVisit;
	public GUIControl mobileSettingsEdit;
	public GUIControl pauseMenuVisit;
	public GUIControl pauseMenuEdit;
	public GUIControl pauseMenu;
	public GUIControl pauseKeyHint;
	public Text[] useVRLabels;
	public GUIControl savePopUp;

	void Start() {
		if (player.ActiveVR == FirstPersonController.VR.None) {
			VRActive = false;
		} else {
			VRActive = true;
		}
#if MOBILE_CONTROL
		UIMobile.Open();
		UIDesktop.Close();
#else
		UIMobile.Close ();
		UIDesktop.Open ();
		print (MuseumLoader.currentAction);
		if(MuseumLoader.currentAction == MuseumLoader.MuseumAction.Visit) {
			pauseMenuVisit.Open ();
			pauseMenuEdit.Close ();
		} else {
			pauseMenuVisit.Close ();
			pauseMenuEdit.Open ();
		}
		if((!player.testMode) && ((!CrossPlatformInputManager.GetActiveInputMethod().Equals(CrossPlatformInputManager.ActiveInputMethod.Touch))
		                   || VRActive)) Screen.lockCursor = true;
#endif

	}

	public void SwitchVR() {
        if (player.ActiveVR == FirstPersonController.VR.None) {
#if MOBILE_INPUT
			player.ActiveVR = FirstPersonController.VR.Durovis;
			player.CameraMode = FirstPersonController.Cam.StereoDurovis;
#else
			player.ActiveVR = FirstPersonController.VR.Oculus;
			player.CameraMode = FirstPersonController.Cam.StereoOculus;
#endif
			VRActive = true;
		} else {
			player.ActiveVR = FirstPersonController.VR.None;
			player.CameraMode = FirstPersonController.Cam.Mono;
			VRActive = false;
		}
		foreach (Text text in useVRLabels) {
			if(VRActive) {
				text.text = "Turn off virtual reality";
			} else {
				text.text = "Use virtual reality";
			}
		}
	}

	public void BackToMain() {
		Application.LoadLevel ("MainMenuScene");
	}

	public void Edit() {
		MuseumLoader.CreateTempMuseum (museum);
		MuseumLoader.currentAction = MuseumLoader.MuseumAction.Edit;
		Application.LoadLevel ("BuildMuseum");
	}

	public void Back() {
		if (MuseumLoader.currentAction == MuseumLoader.MuseumAction.Visit) {
			// TODO: FB share popup first
			BackToMain ();
		} else {
			Edit ();
		}
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.V)) {
            SwitchVR();
        }
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if(savePopUp.IsOpen()) {
				savePopUp.Close();
			} else {
#if MOBILE_INPUT
				Back ();
#else
				if(pauseMenu.IsOpen()) {
					SetPaused(false);
				} else {
					SetPaused(true);
				}
#endif
			}
		}
    }

	void OnDestroy() {
		Screen.lockCursor = false;
	}

	public void SetPaused(bool paused) {
		if(!paused) {
			if((!player.testMode) && ((!CrossPlatformInputManager.GetActiveInputMethod().Equals(CrossPlatformInputManager.ActiveInputMethod.Touch))
			                          || VRActive)) Screen.lockCursor = true;
			pauseKeyHint.Open ();
			pauseMenu.Close ();
			player.paused = false;
		} else {
			Screen.lockCursor = false;
			pauseMenu.Open ();
			pauseKeyHint.Close();
			player.paused = true;
		}
	}

	public void Save() {
		savePopUp.Open ();
	}
}

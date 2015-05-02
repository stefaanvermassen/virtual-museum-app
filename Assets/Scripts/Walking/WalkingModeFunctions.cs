using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WalkingModeFunctions : MonoBehaviour {

	public FirstPersonController player;
    public Museum museum;

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

    public void TempShareFB()
    {
        /*if (!FB.IsLoggedIn)
        {
            GUI.Label((new Rect(179, 11, 287, 160)), "Login to Facebook", MenuSkin.GetStyle("text_only"));
            if (GUI.Button(LoginButtonRect, "", MenuSkin.GetStyle("button_login")))
            {
                FB.Login("email,publish_actions");
            }
        }*/

        FB.Feed(
            linkCaption: "I just visited " + museum.museumName,
            linkName: "Join me in Virtual Museum!",
            link: "http://apps.facebook.com/" + FB.AppId + "/?virtualmuseum=" + FB.UserId
            );                   
    }
}

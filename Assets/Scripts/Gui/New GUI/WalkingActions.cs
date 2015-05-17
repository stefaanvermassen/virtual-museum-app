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
	public GUIControl[] facebookShareButtons;
	public Toast toast;

    private const string FB_LINK = "https://www.facebook.com/dialog/share?app_id=";
    private const string MUSEUM_LINK = "http://museum.awesomepeople.tv/museum/";

	void Start() {
       
		if (player.ActiveVR == FirstPersonController.VR.None) {
			VRActive = false;
		} else {
			VRActive = true;
		}
		if(MuseumLoader.museumID == -1) {
			foreach(GUIControl cont in facebookShareButtons) {
				cont.Close();
			}
		}
#if MOBILE_INPUT
		UIMobile.Open();
		UIDesktop.Close();
		if(MuseumLoader.currentAction == MuseumLoader.MuseumAction.Visit) {
			pauseMenuVisit.Open ();
			pauseMenuEdit.Close ();
		} else {
			pauseMenuVisit.Close ();
			pauseMenuEdit.Open ();
		}
#else
		UIMobile.Close ();
		UIDesktop.Open ();
		//print (MuseumLoader.currentAction);
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

		if (MuseumLoader.currentAction == MuseumLoader.MuseumAction.Visit) {
			var cc = API.CreditController.Instance;
			var newentercreditmodel = new API.CreditModel(){ Action = API.CreditActions.ENTERMUSEUM };
			newentercreditmodel.id = MuseumLoader.museumID;
			cc.AddCredit(newentercreditmodel, (info) => {
				if (info.CreditsAdded) { // check if credits are added
					toast.Notify("Thank you for visiting this museum. Your total amount of tokens is: " + info.Credits);
				} else {
					Debug.Log("No tokens added for entering this museum.");
				}
			}, (error) => {
				Debug.Log("An error occured when adding tokens for the user.");
			});
		}


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
		CloseSettingsDropdown ();
		SetPaused (false);
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
            //FBShare();
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
			} else if(VRActive) {
				SwitchVR ();
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
#if MOBILE_INPUT
		//player.paused = paused;
#else
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
#endif
	}

	public void Save() {
		CloseSettingsDropdown ();
		savePopUp.Open ();
	}

	public void OnSaved() {
		foreach(GUIControl cont in facebookShareButtons) {
			cont.Open();
		}
	}

	public void OpenSettingsDropdown() {
		if(MuseumLoader.currentAction == MuseumLoader.MuseumAction.Visit) {
			mobileSettingsVisit.FlipCloseOpen ();
		} else {
			mobileSettingsEdit.FlipCloseOpen ();
		}
	}

	void CloseSettingsDropdown() {
		mobileSettingsEdit.Close ();
		mobileSettingsVisit.Close ();
	}


    public void FBShare()
    {
		var cc = API.CreditController.Instance;
		var sharedlinkcreditmodel = new API.CreditModel(){ Action = API.CreditActions.SHAREDLINK};
		cc.AddCredit(sharedlinkcreditmodel, (info) => {
			if (info.CreditsAdded) { // check if credits are added
				toast.Notify("Thank you for sharing. Your total amount of tokens is: " + info.Credits);
			} else {
				Debug.Log("No tokens added for shared link action.");
			}
		}, (error) => {
			Debug.Log("An error occured when adding tokens for the user.");
		});
#if UNITY_ANDROID
        if (!FB.IsInitialized)
        {
            FB.Init(OnInit);
        }
        else
        {
            Share();
        }
#else
        Share();
#endif
    }

    private void Share()
    {
#if UNITY_ANDROID
        FB.Feed(
            linkCaption: "I just visited " + museum.museumName,
            linkName: "Join me in Virtual Museum!",
            link: MUSEUM_LINK + museum.museumID
            );
#else
        Application.OpenURL(FB_LINK + FBSettings.AppId
            + "&p[title]=Join me in Virtual Museum!"
            + "&p[descriptions]=I just visited " + museum.museumName
            + "&display=popup&href=" + MUSEUM_LINK + museum.museumID
            + "&redirect_uri=" + MUSEUM_LINK + museum.museumID);
#endif
    }

    private void OnInit()
    {
        if (!FB.IsLoggedIn)
        {
            FB.Login("publish_actions", OnLoggedIn);
        }
        else
        {
            Share();
        }
    }

    private void OnLoggedIn(FBResult result)
    {
        Share();
    }
}

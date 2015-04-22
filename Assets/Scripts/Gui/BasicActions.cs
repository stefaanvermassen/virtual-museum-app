using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class BasicActions : MonoBehaviour {

	void Start () {
		HandleURI ();
	}

	public void startGame()
	{
		Application.LoadLevel("BuildMuseum");
	}

	void HandleURI() {
#if UNITY_ANDROID
		var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		var datastring = currentActivity.Call<string>("getIntentDataString");
		if(datastring.Contains ("museum/")) {
			string[] splitString = datastring.Split(new string[] {"museum/"}, StringSplitOptions.RemoveEmptyEntries);
			int id = int.Parse(splitString[splitString.Length-1]);
			LoadMuseum (id);
		} else {
			MuseumLoader.museumID = -1; // Don't load a museum
		}
#endif
	}

	public void LoadMuseum(int id) {
		MuseumLoader.museumID = id;
		Application.LoadLevel ("WalkingController");
	}
}

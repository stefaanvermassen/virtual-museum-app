using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System;
using API;


#if UNITY_STANDALONE_WIN
using Microsoft.Win32;
#endif

using UnityEngine.UI;

public class StartUp : MonoBehaviour {

	public UnityEngine.UI.Text statusText;

	bool loading = false;
	
	void Start () {
		statusText.fontSize = Screen.width / 40;
		CheckLogin ();
		CheckProtocol ();
		HandleURI ();
	}
	
	void HandleURI() {
#if UNITY_ANDROID
		var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		var datastring = currentActivity.Call<string>("getIntentDataString");
		if(datastring.StartsWith("virtualmuseum://")) {
			HandleURLProtocol(datastring);
		} else if(datastring.Contains ("filter/")) {
			string[] splitString = datastring.Split(new string[] {"filter/"}, StringSplitOptions.RemoveEmptyEntries);
			LoadFilter(splitString[splitString.Length-1]);
		} else if(datastring.Contains ("museum/")) {
			string[] splitString = datastring.Split(new string[] {"museum/"}, StringSplitOptions.RemoveEmptyEntries);
			int id;
			bool success = int.TryParse(splitString[splitString.Length-1], out id);
			if(success) LoadMuseum (id);
			//int id = int.Parse(splitString[splitString.Length-1]);
			//LoadMuseum (id);
		} else if(datastring.Contains ("art/")) {
			string[] splitString = datastring.Split(new string[] {"art/"}, StringSplitOptions.RemoveEmptyEntries);
			int id;
			bool success = int.TryParse(splitString[splitString.Length-1], out id);
			if(success) LoadArt (id);
		} else {
			MuseumLoader.museumID = -1; // Don't load a museum
		}
#elif UNITY_STANDALONE_WIN
		string[] args = Environment.GetCommandLineArgs();
		for(int i = 0; i < args.Length; i++) {
			if(args[i].Equals("-museum") && args.Length > i+1) {
				int id;
				bool success = int.TryParse(args[i+1], out id);
				if(success) LoadMuseum (id);
			} else if(args[i].Equals("-url") && args.Length > i+1) {
				// parse both /art/ and /museum/
				HandleURLProtocol(args[i+1]);
			}
		}
#endif
	}

	void HandleURLProtocol(string fullURL) {
		int id;
		string url = fullURL.Replace ("virtualmuseum://", "");
		if(url.StartsWith("filter/")) {
			LoadFilter(url.Substring("filter/".Length));
		} else if(url.StartsWith("art/")) {
			bool success = int.TryParse(url.Replace("art/","").Replace ("/",""), out id);
			if(success) LoadArt (id);
		} else if (url.StartsWith("museum/")) {
			bool success = int.TryParse(url.Replace("museum/","").Replace ("/",""), out id);
			if(success) LoadMuseum (id);
		}
	}
	
	void CheckProtocol() {
#if UNITY_STANDALONE_WIN
		try {
			string iconString = Environment.GetCommandLineArgs()[0].Replace ("/","\\") + ",0";
			RegistryKey testKey = Registry.ClassesRoot.OpenSubKey("VirtualMuseum");
			if(testKey != null) {
				RegistryKey iconKey = Registry.ClassesRoot.OpenSubKey ("VirtualMuseum\\DefaultIcon");
				if(iconKey != null) {
					string keyPath = iconKey.GetValue("").ToString();
					if(iconString.Equals(keyPath)) {
						iconKey.Close();
						testKey.Close ();
						return;
					}
					iconKey.Close ();
				}
				testKey.Close();
			}
			RegisterProtocol ();
		} catch (Exception e){
			print(e);        
		}
#endif
	}
	
#if UNITY_STANDALONE_WIN
	void RegisterProtocol() {
		try {
			string exePath = Environment.GetCommandLineArgs()[0].Replace ("/","\\");
			statusText.text = "Requesting permision to link virtualmuseum:// links\nto this application";
			Process process = new Process();
			process.StartInfo.FileName = "protocol_installer.exe";
			process.StartInfo.Arguments = exePath;
			process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			process.StartInfo.CreateNoWindow = true;
			process.Start ();
			process.WaitForExit();
			statusText.text = "";
		} catch (Exception e){
			print(e);        
		}
	}
#endif
	
	public void LoadMuseum(int id) {
		statusText.text = "Loading museum...";
		MuseumLoader.museumID = id;
		loading = true;
		Application.LoadLevel ("WalkingController");
	}

	public void LoadArt(int id) {
		statusText.text = "Loading art...";
		loading = true;
		// TODO: To be implemented
	}

	public void LoadFilter(string filter) {
		statusText.text = "Parsing filter...";
		loading = true;
		// TODO: To be implemented
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		} else if(!loading) {
			Application.LoadLevel ("MainMenuScene");
		}
	}

	void CheckLogin() {
		if (!SessionManager.Instance.LoggedIn ()) {
			loading = true;
			Application.LoadLevel ("Login");
		}
	}
}

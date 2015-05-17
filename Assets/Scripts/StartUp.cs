using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System;
using API;

using UnityEngine.UI;

public class StartUp : StatisticsBehaviour {

	public UnityEngine.UI.Text statusText;
	Toast toast;

	bool loading = false;
	
	void Start () {
		StartStatistics("StartUp");
		try {
			statusText.fontSize = Screen.width / 40;
			CheckLogin ();
			CheckProtocol ();
			HandleURI ();
		} catch(Exception ex) {
			loading = false;
		} finally {
			if(!loading) {
				Application.LoadLevel ("MainMenuScene");
			}
		}
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
		string[] separators = new string[]{"?","#"};
		string url = fullURL.Replace ("virtualmuseum://", "").Split (separators,StringSplitOptions.RemoveEmptyEntries)[0];
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
		if (File.Exists("protocol.bat")) {
			// Don't add anything to registry, it's probably already done.
			return;
		}
		RegisterProtocol();
		/*try {
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
		}*/
#endif
	}
	
#if UNITY_STANDALONE_WIN
	void RegisterProtocol() {
		/*try {
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
		}*/
		try {
			string exePath = Environment.GetCommandLineArgs()[0].Replace ("\\","\\\\").Replace ("/","\\\\");
			// Start writing self-contained reg-bat file (magic!)
			var batFile = File.CreateText("protocol.bat");
			batFile.WriteLine ("REGEDIT4");
			batFile.WriteLine ("");
			batFile.WriteLine ("; @ECHO OFF");
			batFile.WriteLine ("; CLS");
			batFile.WriteLine ("; REGEDIT.EXE /S \"%~f0\"");
			batFile.WriteLine ("; EXIT");
			batFile.WriteLine ("");
			batFile.WriteLine ("[HKEY_CLASSES_ROOT\\VirtualMuseum]");
			batFile.WriteLine ("@=\"URL:VirtualMuseum Protocol\"");
			batFile.WriteLine ("\"URL Protocol\"=\"\"");
			batFile.WriteLine ("");
			batFile.WriteLine ("[HKEY_CLASSES_ROOT\\VirtualMuseum\\DefaultIcon]");
			batFile.WriteLine ("@=\"\\\"" + exePath + "\\\"\"");
			batFile.WriteLine ("");
			batFile.WriteLine ("[HKEY_CLASSES_ROOT\\VirtualMuseum\\shell]");
			batFile.WriteLine ("");
			batFile.WriteLine ("[HKEY_CLASSES_ROOT\\VirtualMuseum\\shell\\open]");
			batFile.WriteLine ("");
			batFile.WriteLine ("[HKEY_CLASSES_ROOT\\VirtualMuseum\\shell\\open\\command]");
			batFile.WriteLine ("@=\"\\\"" + exePath + "\\\" -url \\\"%1\\\"\"");
			batFile.Close();
			
			// Launch self-contained reg-bat file. This will ask for permission if UAC is enabled!
			/*Process myProcess = new Process();
			myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			myProcess.StartInfo.CreateNoWindow = true;
			myProcess.StartInfo.UseShellExecute = true;
			myProcess.StartInfo.Verb = "runas";
			myProcess.StartInfo.FileName = "cmd.exe";
			string path = exePath.Substring(0,exePath.LastIndexOf("\\\\")).Replace("\\\\","\\")+"\\protocol.bat";
			print (path);
			myProcess.StartInfo.Arguments = "/c " + path;
			myProcess.EnableRaisingEvents = true;
			myProcess.Start();
			myProcess.WaitForExit();
			int ExitCode = myProcess.ExitCode;
			print(ExitCode);*/
		} catch (Exception e){
			print(e);        
		}
	}
#endif
	
	public void LoadMuseum(int id) {
		statusText.text = "Loading museum...";
		MuseumLoader.museumID = id;
		MuseumLoader.currentAction = MuseumLoader.MuseumAction.Visit;
		loading = true;
		Application.LoadLevel ("WalkingController");
	}

	public void LoadArt(int id) {
		statusText.text = "Loading art...";
		loading = true;
		// TODO: To be implemented
	}

	public void LoadFilter(string s) {
		statusText.text = "Parsing filter...";
		loading = true;

		Scanning.ArtFilter filter = new Scanning.ArtFilter();
		filter.Configure(s);
		filter.Collect ();

		//toast.notify ("Art has been added to your collection.");
		//API.ArtWorkFilter apiFilter = new API.ArtWorkFilter ();
		//API.ArtworkFilterController c = API.ArtworkFilterController.Instance;
		//c.CreateArtWorkFilter(apiFilter);
		//pop up toast

		loading = false;
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
	}

	void CheckLogin() {
		if (!SessionManager.Instance.LoggedIn ()) {
			loading = true;
			Application.LoadLevel ("Login");
		}
	}
}

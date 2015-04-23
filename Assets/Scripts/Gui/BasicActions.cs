using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System;

using UnityEngine.UI;

public class BasicActions : MonoBehaviour {

	void Start () {
		RegisterProtocol ();
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
			int id;
			bool success = int.TryParse(splitString[splitString.Length-1], out id);
			if(success) LoadMuseum (id);
			//int id = int.Parse(splitString[splitString.Length-1]);
			//LoadMuseum (id);
		} else {
			MuseumLoader.museumID = -1; // Don't load a museum
		}
#elif UNITY_STANDALONE_WIN
		string[] args = Environment.GetCommandLineArgs();
		for(int i = 0; i < args.Length; i++) {
			if(args[i].Equals("-museum") && args.Length > i+1) {
				int id;
				bool success = int.TryParse(args[i+1].Replace ("virtualmuseum:","").Replace ("/",""), out id);
				if(success) LoadMuseum (id);
			}
		}
#endif
	}

	void RegisterProtocol() {
#if UNITY_STANDALONE_WIN
		try {
			if (File.Exists("protocol.bat")) {
				// Don't add anything to registry, it's probably already done.
				return;
			}
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
			batFile.WriteLine ("@=\"\\\"" + exePath + "\\\" -museum \\\"%1\\\"\"");
			batFile.Close();

			// Launch self-contained reg-bat file. This will ask for permission if UAC is enabled!
			Process myProcess = new Process();
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
			print(ExitCode);
		} catch (Exception e){
			print(e);        
		}
#endif
	}

	public void LoadMuseum(int id) {
		MuseumLoader.museumID = id;
		Application.LoadLevel ("WalkingController");
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
	}
}

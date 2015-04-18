using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class CallJavaCode : MonoBehaviour {

	[DllImport("javabridge")]
	private static extern IntPtr getCacheDir();

	private string cacheDir = "Push to get cache dir";
	void OnGUI ()
	{
		if (GUI.Button(new Rect (15, 125, 450, 100), cacheDir))
		{
			IntPtr stringPtr = getCacheDir();
			Debug.Log("stringPtr = " +stringPtr);
			String cache = Marshal.PtrToStringAnsi(stringPtr);
			Debug.Log("getCacheDir returned " + cache);
			cacheDir = cache;
		}
	}
}

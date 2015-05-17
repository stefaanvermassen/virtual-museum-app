using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

#if UNITY_ANDROID && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class FileBrowser: GUIControl
{
	public Text directoryLabel;
	public Text fileLabel;
	public InputField searchField;
	public GUIControl directoryView;
	public GUIControl fileView;
	public DirectoryInfo previousDirectory, currentDirectory;
	public string prevSearch = "";
	private string directoryUp = "..";
	private static int maxNrOfNameChars = 50;
	private string selectedFilePath;
	public GUIControl placeHolder;
	private FileBrowserListener listener;
	public string[] fileExtensions;

	private enum Type
	{
		FOLDER,
		FILE
    };

	public static bool PathIsValid(string path)
	{
		return  !string.IsNullOrEmpty(path);
	}

	public static string CropString(string s)
	{
		return s.Substring(Math.Max(0, s.Length - maxNrOfNameChars));
	}

	public string[] GetFileExtensions()
	{
		return fileExtensions;
	}

	public void Open(FileBrowserListener listener)
	{
		this.listener = listener;
		base.Open();

#if UNITY_ANDROID && !UNITY_EDITOR
        GetAndroidPath();
#endif
	}

	public override void Close()
	{
		base.Close();
		listener.FileIsSelected();
	}

	void Start()
	{
		currentDirectory = new DirectoryInfo(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments));
		UpdateFileAndFolder ();
        screenName = "FileBrowser";
	}
    
#if UNITY_ANDROID && !UNITY_EDITOR
    void GetAndroidPath()
    {
        // Attach our thread to the java vm; obviously the main thread is already attached but this is good practice..
		AndroidJNI.AttachCurrentThread();
		
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            activity.Call("startBrowser");
        }));
        
        while(activity.Call<string>("getPath") == null);
        selectedFilePath = WWW.UnEscapeURL(activity.Call<string>("getPath")).Replace("content://fm.clean/document/", "");
        
        activity.Call("setPath", new object[]{ null });
        Close();
    }
#endif

    private void UpdateFileAndFolder()
	{
		if(directoryLabel == null) {
			Debug.Log("DirectoryLabel is null.");
		} else if(currentDirectory == null) {
			Debug.Log("CurrentDirectory is null.");
		}

		// Only show last 100 chars
		directoryLabel.text = CropString(currentDirectory.FullName);
		DrawFiles();
		DrawDirectories();
	}

	public string GetSelectedFile()
	{
		return selectedFilePath;
	}

	private string GetSearch()
	{
		return searchField.text;
	}

	public void DrawFiles()
	{
		string search = GetSearch();
		if(previousDirectory == currentDirectory && prevSearch == search) {
			return;
		}

		// Remove the buttons
		fileView.RemoveAllChildren();
		// The selected file is now invalid
		fileLabel.text = "";

		IEnumerable<FileInfo> files = GetFileList(search.Length != 0, search);
		foreach(FileInfo file in files) {
			AddFileButton(file);
		}

		prevSearch = search;
	}

	private void DrawDirectories()
	{
		if(previousDirectory == currentDirectory) {
			return;
		}

		// Remove the buttons
		directoryView.RemoveAllChildren();
		if(currentDirectory.Parent != null) {
			AddButton(directoryUp, currentDirectory.Parent.FullName, directoryView, Type.FOLDER);
		}

		DirectoryInfo[] directories = currentDirectory.GetDirectories();
		foreach(DirectoryInfo dir in directories) {
			AddDirectoryButton(dir);
		}

		previousDirectory = currentDirectory;
	}

	private void AddFileButton(FileInfo file)
	{
		AddButton(file.Name, file.FullName, fileView, Type.FILE);
	}

	private void SetSelectedFile(String fullPath)
	{
		selectedFilePath = fullPath;
		fileLabel.text = CropString(fullPath);
	}

	private void AddDirectoryButton(DirectoryInfo dir)
	{
		AddButton(dir.Name, dir.FullName, directoryView, Type.FOLDER);
	}

	private void AddButton(string name, string fullName, GUIControl content, Type type)
	{
		// Create a clone of the contents child
		GUIControl buttonControl = content.AddDynamicChild();
		Button button = buttonControl.GetComponent<Button>();
		// Change button label
		button.GetComponentsInChildren<Text>() [0].text = name;
		string info = fullName;
		button.onClick.AddListener(() => HandleClick(info, type));
	}

	private void HandleClick(string name, Type type)
	{
		switch(type) {
		case Type.FOLDER:
			currentDirectory = new DirectoryInfo(name);
            // Clear search
			searchField.text = "";
			UpdateFileAndFolder();
			return;
		case Type.FILE:
			SetSelectedFile(name);
			return;
		default:
			break;
		}
	}

	// Only returns directories, drives and images
	private IEnumerable<FileInfo> GetFileList(bool recursive, string searchPattern)
	{
		IEnumerable<FileInfo> files = Enumerable.Empty<FileInfo>();
		FileComparer comparer = new FileComparer();

		if(searchPattern.Length == 0) {
			searchPattern = "*";
		} else if(!searchPattern.Contains("*")) {
			searchPattern = "*" + searchPattern + "*";
		}

		foreach (string ext in fileExtensions)
		{
			files = files.Union(SearchDirectory(currentDirectory, searchPattern + ext, recursive), comparer);
		}

		return files;
	}

	private FileInfo[] SearchDirectory(DirectoryInfo dir, string sp, bool recursive)
	{
		return dir.GetFiles(sp,(recursive) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
	}
}

class FileComparer : IEqualityComparer<FileInfo>
{

	public bool Equals(FileInfo x, FileInfo y)
	{
		return x.FullName == y.FullName;
	}

	public int GetHashCode(FileInfo obj)
	{
		return obj.GetHashCode();
	}
}
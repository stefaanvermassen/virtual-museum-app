using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using   System;

public class FileBrowser: GUIControl
{
	public Text directoryLabel;
	public Text fileLabel;
	public InputField searchField;
	public GUIControl directoryView;
	public GUIControl fileView;
	public DirectoryInfo previousDirectory, currentDirectory;
	public string prevSearch = "";
	public string[] fileExtensions;
	private string directoryUp = "..";
	private static int maxNrOfNameChars=50;
	private string selectedFilePath;
	public GUIControl placeHolder;
	private FileBrowserListener listener;

    private enum Type
    {
        FOLDER,
        FILE
    };
	public static bool PathIsValid(string path){
		return  !string.IsNullOrEmpty (path);
	}
	public static string CropString(string s){
		return s.Substring (Math.Max (0, s.Length - maxNrOfNameChars));
	}

	public string[] GetFileExtensions ()
	{
		return fileExtensions;
	}

	public void Open(FileBrowserListener listener){
		this.listener = listener;
		placeHolder.Replace (this);
	}

	public override void Close(){
		placeHolder.Replace (this);
		listener.FileIsSelected ();
	}

	void Start ()
	{
		currentDirectory = new DirectoryInfo (Directory.GetCurrentDirectory ());
		UpdateFileAndFolder ();
	}

	private void UpdateFileAndFolder ()
	{
		if (directoryLabel == null) {
			Debug.Log ("DirectoryLabel is null.");
		} else if (currentDirectory == null) {
			Debug.Log ("CurrentDirectory is null.");
		}

		//only show last 100 chars
		directoryLabel.text = CropString(currentDirectory.FullName);
		DrawFiles ();
		DrawDirectories ();
	}

	public string GetSelectedFile ()
	{
		return selectedFilePath;
	}

	private string GetSearch ()
	{
		return searchField.text;
	}

	public void DrawFiles ()
	{
		string search = GetSearch();
		if (previousDirectory == currentDirectory && prevSearch == search) {
			return;
		}

		// Remove the buttons
		fileView.RemoveAllChildren ();
		// The selected file is now invalid
		fileLabel.text = "";

		IEnumerable<FileInfo> files = GetFileList (search.Length != 0, search);
		foreach (FileInfo file in files) {
			AddFileButton (file, FileIsSelectable (file));
		}

		prevSearch = search;
	}

	private void DrawDirectories ()
	{
		if (previousDirectory == currentDirectory) {
			return;
		}

		// Remove the buttons
		directoryView.RemoveAllChildren ();
		if (currentDirectory.Parent != null) {
			AddButton (directoryUp, currentDirectory.Parent.FullName, directoryView, Type.FOLDER, true);
		}

		DirectoryInfo[] directories = currentDirectory.GetDirectories ();
		foreach (DirectoryInfo dir in directories) {
			AddDirectoryButton (dir);
		}

		previousDirectory = currentDirectory;
	}

	private void AddFileButton (FileInfo file, bool enabled)
	{
		AddButton(file.Name, file.FullName, fileView, Type.FILE, enabled);
	}

	private void SetSelectedFile(String fullPath){
		selectedFilePath = fullPath;
		fileLabel.text = CropString (fullPath);
	}

	private void AddDirectoryButton (DirectoryInfo dir)
	{
		AddButton(dir.Name, dir.FullName, directoryView, Type.FOLDER, true);
	}

	private void AddButton (string name, string fullName, GUIControl content, Type type, bool interactable)
	{
		//create a clone of the contents child
		GUIControl buttonControl = content.AddDynamicChild ();
		Button button = buttonControl.GetComponent<Button> ();
		button.interactable = interactable;
		//change button label
		button.GetComponentsInChildren<Text> () [0].text = name;
		string info = fullName;
		button.onClick.AddListener (() => HandleClick (info, type));
	}

	private bool FileIsSelectable (FileInfo file)
	{
		foreach (string ext in fileExtensions) {
			if (file.Name.EndsWith (ext))
				return true;
		}

		return false;
	}

	private void HandleClick (string name, Type type)
	{
		switch (type) {
		case Type.FOLDER:
			currentDirectory = new DirectoryInfo (name);
            // Clear search
			searchField.text = "";
			Debug.Log ("Foldere selected");
			UpdateFileAndFolder ();
			return;
		case Type.FILE:
			SetSelectedFile(name);
			return;
		default:
			break;
		}
	}
	//TODO search file doesn't work yet

	// Only returns directories, drives and images
	private IEnumerable<FileInfo> GetFileList (bool recursive, string searchPattern)
	{
		IEnumerable<FileInfo> files = Enumerable.Empty<FileInfo> ();
		FileComparer comparer = new FileComparer ();

		if (searchPattern.Length == 0) {
			searchPattern = "*";
		} else if (!searchPattern.Contains ("*")) {
			searchPattern = "*" + searchPattern + "*";
		}

		files = files.Union (SearchDirectory (currentDirectory, searchPattern, recursive), comparer);

		return files;
	}

	private FileInfo[] SearchDirectory (DirectoryInfo dir, string sp, bool recursive)
	{
		return dir.GetFiles (sp, (recursive) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
	}
}

class FileComparer : IEqualityComparer<FileInfo>
{

	public bool Equals (FileInfo x, FileInfo y)
	{
		return x.FullName == y.FullName;
	}

	public int GetHashCode (FileInfo obj)
	{
		return obj.GetHashCode ();
	}
}
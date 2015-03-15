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
	private int maxNrOfNameChars=60;
	private enum Type
	{
		FOLDER,
		FILE    }
	;

	public string[] getFileExtensions ()
	{
		return fileExtensions;
	}

	void Start ()
	{
		currentDirectory = new DirectoryInfo (Directory.GetCurrentDirectory ());
		updateFileAndFolder ();
	}

	private void updateFileAndFolder ()
	{
		if (directoryLabel == null) {
			Debug.Log ("DirectoryLabel is null.");
		} else if (currentDirectory == null) {
			Debug.Log ("CurrentDirectory is null.");
		}
		//only show last 100 chars
		directoryLabel.text = currentDirectory.FullName.Substring (Math.Max (0, currentDirectory.FullName.Length - maxNrOfNameChars));
		string search = getSearch ();
		drawFiles (search);
		drawDirectories ();
	}

	public string getSelectedFile ()
	{
		return fileLabel.text;
	}

	private string getSearch ()
	{
		return searchField.text;
	}

	private void drawFiles (string search)
	{
		if (previousDirectory == currentDirectory && prevSearch == search) {
			return;
		}

		// Remove the buttons
		fileView.removeAllChildren ();
		// The selected file is now invalid
		fileLabel.text = "";

		IEnumerable<FileInfo> files = getFileList (search.Length != 0, search);
		foreach (FileInfo file in files) {
			addFileButton (file, fileIsSelectable (file));
		}

		prevSearch = search;
	}

	private void drawDirectories ()
	{
		if (previousDirectory == currentDirectory) {
			return;
		}

		// Remove the buttons
		directoryView.removeAllChildren ();

		if (currentDirectory.Parent != null) {
			//TODO different color for parent directory
			addButton (directoryUp, currentDirectory.Parent.FullName, directoryView, Type.FOLDER, true);
		}

		DirectoryInfo[] directories = currentDirectory.GetDirectories ();
		foreach (DirectoryInfo dir in directories) {
			addDirectoryButton (dir);
		}

		previousDirectory = currentDirectory;
	}

	private void addFileButton (FileInfo file, bool enabled)
	{

		addButton (file.Name, file.FullName, fileView, Type.FILE, enabled);


	}
	//TODO
	private void setSelectedFile(String fullPath){
	}
	private void addDirectoryButton (DirectoryInfo dir)
	{
		addButton (dir.Name, dir.FullName, directoryView, Type.FOLDER, true);
	}

	private void addButton (string name, string fullName, GUIControl content, Type type, bool interactable)
	{
		GUIControl buttonControl = GUIControl.init (GUIControl.types.Button);
		Button button = buttonControl.GetComponent<Button> ();
		button.interactable = interactable;
		//change button label
		button.GetComponentsInChildren<Text> () [0].text = name;
		content.add (buttonControl);
		string info = fullName;
		button.onClick.AddListener (() => handleClick (info, type));

	}

	private bool fileIsSelectable (FileInfo file)
	{
		foreach (string ext in fileExtensions) {
			if (file.Name.EndsWith (ext))
				return true;
		}
		return false;
	}

	private void handleClick (string name, Type type)
	{
		switch (type) {
		case Type.FOLDER:
			currentDirectory = new DirectoryInfo (name);
                // Clear search
			searchField.text = "";
			updateFileAndFolder ();
			return;
		case Type.FILE:
			fileLabel.text = name;
			return;
		default:
			break;
		}
	}

	// Only returns directories, drives and images
	private IEnumerable<FileInfo> getFileList (bool recursive, string searchPattern)
	{
		IEnumerable<FileInfo> files = Enumerable.Empty<FileInfo> ();
		FileComparer comparer = new FileComparer ();
		if (searchPattern.Length == 0) {
			searchPattern = "*";
		} else if (!searchPattern.Contains ("*")) {
			searchPattern = "*" + searchPattern + "*";
		}


		files = files.Union (searchDirectory (currentDirectory, searchPattern, recursive), comparer);



		return files;
	}

	private FileInfo[] searchDirectory (DirectoryInfo dir, string sp, bool recursive)
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
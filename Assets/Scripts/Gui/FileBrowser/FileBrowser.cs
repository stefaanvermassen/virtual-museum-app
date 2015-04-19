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
	public string[] fileExtensions;
	private string directoryUp = "..";
	private static int maxNrOfNameChars=50;
	private string selectedFilePath;
	public GUIControl placeHolder;
	private FileBrowserListener listener;

#if UNITY_ANDROID && !UNITY_EDITOR
    private IntPtr AndroidFileBrowser;
    private int getFilePath;
    private string path = "Push to get filepath";
#endif

    private enum Type
	{
		FOLDER,
		FILE    }
	;
	public static string cropString(string s){
		return s.Substring (Math.Max (0, s.Length - maxNrOfNameChars));
	}
	public string[] getFileExtensions ()
	{
		return fileExtensions;
	}
	public void open(FileBrowserListener listener){
		this.listener = listener;
		placeHolder.replace (this);
	}
	public override void close(){
		placeHolder.replace (this);
		listener.fileIsSelected ();
	}

	void Start ()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
        // Attach our thread to the java vm; obviously the main thread is already attached but this is good practice..
		JavaVM.AttachCurrentThread();
		
		// Create an AndroidFileBrowser object...
		IntPtr cls_FileBrowser	= JNI.FindClass("be/ugent/virtualmuseum/FileBrowser");
		int mid_FileBrowser		= JNI.GetMethodID(cls_FileBrowser, "<init>", "(Landroid/app/Activity;)V");
        IntPtr obj_FileBrowser = JNI.NewObject(cls_FileBrowser, mid_FileBrowser);
		Debug.Log("FileBrowser object = " + obj_FileBrowser);

		// create a global reference to the FileBrowser object and fetch method id(s)..
		AndroidFileBrowser			= JNI.NewGlobalRef(obj_FileBrowser);
		getFilePath     	        = JNI.GetMethodID(cls_FileBrowser, "getFilePath", "()Ljava/lang/String;");
		Debug.Log("AndroidFileBrowser global ref = " + AndroidFileBrowser);
		Debug.Log("AndroidFileBrowser method id = " + getFilePath);
#endif
        currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
        updateFileAndFolder();
	}
    
#if UNITY_ANDROID && !UNITY_EDITOR
    void OnGUI()
    {
        if (GUI.Button(new Rect(15, 125, 450, 100), path))
        {
            path = getPath();
            Debug.Log("getFilePath returned " + path);
        }
    }

    private string getPath()
	{
		// again, make sure the thread is attached..
		JavaVM.AttachCurrentThread();

		// get the Java String object from the AndroidFileBrowser object
		IntPtr str_path 	= JNI.CallObjectMethod(AndroidFileBrowser, getFilePath);
		Debug.Log("str_path = " + str_path);
	
		// convert the Java String into a Mono string
		IntPtr stringPtr = JNI.GetStringUTFChars(str_path, 0);
		Debug.Log("stringPtr = " +stringPtr);
		String p = Marshal.PtrToStringAnsi(stringPtr);
		JNI.ReleaseStringUTFChars(str_path, stringPtr);

		Debug.Log("return value is = " + p);

		return p;
	}
#endif

    private void updateFileAndFolder ()
	{
		if (directoryLabel == null) {
			Debug.Log ("DirectoryLabel is null.");
		} else if (currentDirectory == null) {
			Debug.Log ("CurrentDirectory is null.");
		}
		//only show last 100 chars
		directoryLabel.text = cropString(currentDirectory.FullName);
		drawFiles ();
		drawDirectories ();
	}

	public string getSelectedFile ()
	{
		return selectedFilePath;
	}

	private string getSearch ()
	{
		return searchField.text;
	}

	public void drawFiles ()
	{
		string search =getSearch();
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
	private void setSelectedFile(String fullPath){
		selectedFilePath = fullPath;
		fileLabel.text = cropString (fullPath);
	}
	private void addDirectoryButton (DirectoryInfo dir)
	{
		addButton (dir.Name, dir.FullName, directoryView, Type.FOLDER, true);
	}

	private void addButton (string name, string fullName, GUIControl content, Type type, bool interactable)
	{
		//create a clone of the contents child
		GUIControl buttonControl = content.addDynamicChild ();
		Button button = buttonControl.GetComponent<Button> ();
		button.interactable = interactable;
		//change button label
		button.GetComponentsInChildren<Text> () [0].text = name;
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
			setSelectedFile(name);
			return;
		default:
			break;
		}
	}
	//TODO search file doesn't wokr yet

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
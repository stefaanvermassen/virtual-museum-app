using UnityEngine;
using System.Collections;

public abstract class FileBrowserListener : MonoBehaviour {

	public FileBrowser fileBrowser;
	public abstract void FileIsSelected();

}

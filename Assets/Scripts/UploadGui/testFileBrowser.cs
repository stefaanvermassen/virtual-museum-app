using UnityEngine;
using System.Collections;

public class testFileBrowser : MonoBehaviour {
	//skins and textures
	public GUISkin guiSkin;
	public Texture2D file,folder,back,drive;
	
	private string[] layoutTypes = {"Type 0","Type 1"};
	//initialize file browser
    private FileBrowser fb = new FileBrowser();
    private string output = "no file";
	// Use this for initialization
    private void Start()
    {
		//setup file browser style
        fb.guiSkin = guiSkin; //set the starting skin
		//set the various textures
		fb.fileTexture = file;
		fb.directoryTexture = folder;
		fb.backTexture = back;
		fb.driveTexture = drive;
		//show the search bar
		fb.showSearch = true;
		//search recursively (setting recursive search may cause a long delay)
		fb.searchRecursively = true;
        GUIStyle cancelStyle = new GUIStyle(guiSkin.GetStyle("button"));
        cancelStyle.alignment = TextAnchor.MiddleCenter;
        fb.cancelStyle = cancelStyle;
        fb.selectStyle = cancelStyle;
        fb.searchStyle = cancelStyle;
	}

    private void OnGUI()
    {
        if (fb.draw())
        {
            if (fb.outputFile == null)
            {
                Debug.Log("Cancel hit");
            }
            else
            {
                Debug.Log("Ouput File = \"" + fb.outputFile.ToString() + "\"");
                /*the outputFile variable is of type FileInfo from the .NET library "http://msdn.microsoft.com/en-us/library/system.io.fileinfo.aspx"*/
            }
        }
	}
}

using UnityEngine;
using System.Collections;

public class testFileBrowser : MonoBehaviour {

	public Texture2D file,folder,back,drive;
    private FileBrowser fb = new FileBrowser();

    private void Start()
    {
		fb.fileTexture = file;
		fb.directoryTexture = folder;
		fb.backTexture = back;
		fb.driveTexture = drive;

		fb.showSearch = true;
		fb.searchRecursively = true;
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

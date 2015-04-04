using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;

//class responsible for synchronizing user input data and local data
public class ArtworkGUIData : FileBrowserListener
{
	//GUI fields
	public InputField nameInput;
	public Image thumbNail;

	//internal values fields
	private Art artWork;


	public override void FileIsSelected ()
	{
		artWork.imagePathSource = fileBrowser.GetSelectedFile ();
		artWork.imageFile = File.ReadAllBytes (fileBrowser.GetSelectedFile ());
		Refresh();
	}

	public void Refresh() 
	{
		//update properties
		if (artWork.name == null) {
			nameInput.text = "";
		} else {
			nameInput.text = artWork.name;
		}
		//update image
		thumbNail.enabled = true;
		Texture2D texture = new Texture2D (0, 0);
		texture.LoadImage (artWork.imageFile);
		thumbNail.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);

	}

	public void Save ()
	{
		artWork.name = nameInput.text;
		//save changes
		artWork.Save();
	}

	public ArtworkGUIData ()
	{
		artWork = new Art ();
	}

	public void Init (Art artWork)
	{
		this.artWork = artWork;
		//check name of 
		Refresh();
	}




	public Art ArtWork {
		get {
			return this.artWork;
		}
	}


	public void Upload ()
	{
		if (artWork.name == "") {
			Debug.Log ("Empty name not allowed");
			return;
		}
		if (artWork.imageFile == null || string.IsNullOrEmpty (artWork.imagePathSource)) {
			Debug.Log ("No image selected.");
			return;
		}
		artWork.SaveRemote ();
	}

}

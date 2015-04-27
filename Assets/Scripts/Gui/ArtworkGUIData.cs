﻿using UnityEngine;
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
	//keep track if changed
	private bool changed;

	public override void FileIsSelected ()
	{
		//An image can only be added not edited
		if (!artWork.HasID () && FileBrowser.PathIsValid (fileBrowser.GetSelectedFile ())) {
			artWork.imagePathSource = fileBrowser.GetSelectedFile ();
			artWork.imageFile = File.ReadAllBytes (fileBrowser.GetSelectedFile ());
			if (artWork.imageFile != null) {
				changed = true;
				Refresh ();
			}
		}
	}

	public void Refresh ()
	{
		//update properties
		if (artWork.name == null) {
			nameInput.text = "";
		} else {
			nameInput.text = artWork.name;
		}
		//update image
		if ( artWork.image!= null) {
			thumbNail.enabled = true;
			thumbNail.sprite = Sprite.Create (artWork.image, new Rect (0, 0, artWork.image.width, artWork.image.height), Vector2.zero);

		} else if (artWork.imageFile != null) {
			thumbNail.enabled = true;
			Texture2D texture = new Texture2D (1, 1);
			texture.LoadImage (artWork.imageFile);
			thumbNail.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);
		}
	}

	public void Save ()
	{
		if (nameInput.text != "" && artWork.name != nameInput.text) {
			artWork.name = nameInput.text;
			changed = true;
		}
		//saving could lead to an upload, thus it shouldn be done if not necessary
		if (changed) {
			//save changes
			artWork.Save ();
		}
	}

	public ArtworkGUIData ()
	{
		artWork = new Art ();
	}

	public void Init (Art artWork)
	{
		this.artWork = artWork;
		//check name of 
		Refresh ();
	}

	public Art ArtWork {
		get {
			return this.artWork;
		}
	}
}

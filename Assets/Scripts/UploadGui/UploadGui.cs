using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UploadGui : GUIControl
{
	// For fileBrowser

	public FileBrowser fileBrowser;
	// UploadGUI
	public Text pathField;
	public Image thumbnail;
	private byte[] uploadableFile;
	private IEnumerator www;
	private string accessToken;
	private const string BASE_URL = "http://api.awesomepeople.tv/";
	private const string TOKEN = "Token";
	private const string ARTWORK = "api/artwork";
	private const string MIME = "image/";
	

	// Use this for initialization
	private void Start ()
	{
	}

	public override void open ()
	{
		base.open ();
		Debug.Log ("open");

		//update thumbnail and path label
		update ();
	}

	public void upload ()
	{
		string selected = fileBrowser.getSelectedFile ();
		Debug.Log (selected
		);
		//if path not empty, upload file
		if (selected.Length != 0) {
			//Upload selected file
			if (accessToken == "") {
				stubLogin ();
			}
			uploadableImage ();
			Debug.Log ("Uploaded " + selected + " successful!");
		}
                

		Debug.Log ("Please select an image file!");
              
        
	}

  


    

	// Update is called once per frame
	private void update ()
	{
		if (fileBrowser != null) {
			// Update our GUI
			string selected = fileBrowser.getSelectedFile ();
			if (selected != "") {
				pathField.text = selected;					
				thumbnail.enabled = true;
				Texture2D image = new Texture2D (0, 0);
				uploadableFile = File.ReadAllBytes (selected);
				image.LoadImage (uploadableFile);
				thumbnail.sprite = Sprite.Create (image, new Rect (0, 0, image.width, image.height), Vector2.zero);

			}
           
		}
	}

  

	/*
     * http://api.awesomepeople.tv/Token
     * {"grant_type":"password",
     * "username":"username",
     * "password":"password"}
     */
	private void stubLogin ()
	{
		API.UserController uc = API.UserController.Instance;
		uc.stubLogin ();
	}

	private void uploadableImage()
	{
		string[] splitted = pathField.text.Split(new char[]{'.'});
		string mime = splitted[splitted.Length - 1];
		splitted = pathField.text.Split(new char[] { '/', '\\' });
		string name = splitted[splitted.Length - 1];
		API.ArtworkController ac = API.ArtworkController.Instance;
		ac.uploadImage (name, mime, pathField.text, uploadableFile, 
		                ((response) => {Debug.Log("Upload was succesfull");}), 
		                ((error) => {Debug.Log("Upload failed!");}));
	}


}

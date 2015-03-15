﻿using SimpleJSON;
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
		WWW www = sendPost (BASE_URL + TOKEN, new string[] {
			"grant_type",
			"username",
			"password"
		}, new string[] {
			"password",
			"museum@awesomepeople.tv",
			"@wesomePeople_20"
		});
		JSONNode node = JSON.Parse (www.text);
		accessToken = node ["access_token"];
	}

	private void uploadableImage ()
	{
		string[] splitted = pathField.text.Split (new char[]{'.'});
		string mime = splitted [splitted.Length - 1];
		splitted = pathField.text.Split (new char[] { '/', '\\' });
		string name = splitted [splitted.Length - 1];
		WWW www = sendImagePost (BASE_URL + ARTWORK, pathField.text, name, mime);
		Debug.Log (www.text);
	}

	private WWW sendImagePost (string url, string imageLocation, string name, string mime)
	{
		WWWForm form = new WWWForm ();
		form.AddBinaryData (imageLocation, uploadableFile, name, MIME + mime);

		return postForm (url, form);
	}

	private WWW sendPost (string url, string[] name, string[] value)
	{
		WWWForm form = new WWWForm ();
		for (int i = 0; i < name.Length; i++) {
			form.AddField (name [i], value [i]);
		}

		return postForm (url, form);
	}

	private WWW postForm (string url, WWWForm form)
	{
		Dictionary<string, string> headers = form.headers;
		byte[] rawData = form.data;

		WWW www = new WWW (url, rawData, headers);
		while (!www.isDone)
			;
		return www;
	}
}

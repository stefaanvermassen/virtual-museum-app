using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using API;
using System;

public class ArtList : MonoBehaviour {
	
	public GUIControl artPopUp;
	
	public GUIControl popUpNormal;
	public Image popUpImage;
	public Text popUpTitle;
	public Text popUpArtist;
	public Text popUpDescription;
	
	// When you're the owner
	public GUIControl popUpOwner;
	public InputField popUpTitleInput;
	public InputField popUpArtistInput;
	public InputField popUpDescriptionInput;

	private bool started = false;
	GameObject listItem;
	GameObject separatorLine;
	int elementCount;
	int userID = -1;
	
	void Start () {
		listItem = (GameObject)Resources.Load("gui/ArtListItem");
		separatorLine = (GameObject)Resources.Load ("gui/ListItemSeparator");
		ClearList ();
		GetUserID ();
		InitList();
		started = true;
	}

	void OnEnable() {
		if(started) InitList ();
	}
	
	void ClearList() {
		/*ArtListItem[] listItems = GetComponentsInChildren<ArtListItem> ();
		foreach (var o in listItems) {
			Destroy(o.gameObject);
		}
		Image[] separators = GetComponentsInChildren<Image> ();
		Image currentImage = GetComponent<Image> ();
		foreach (var o in separators) {
			if(!o.Equals (currentImage)) Destroy(o.gameObject);
		}*/

		for (int i = transform.childCount - 1; i >= 0; --i) {
			GameObject.Destroy(transform.GetChild(i).gameObject);
		}
		transform.DetachChildren();
		
		elementCount = 0;
	}
	
	private Dictionary<int, Art>.ValueCollection allArt;
	
	public void InitList() {
		EventHandler handler = new EventHandler (OnArtLoaded);
		print ("init called");
		ClearList ();
		Catalog.RefreshArtWork (handler);		
	}

	public void OnArtLoaded(object sender, EventArgs e) {
		Art art = (Art)sender;
		ArtListItem item = ((GameObject) GameObject.Instantiate(listItem)).GetComponent<ArtListItem>();
		if(elementCount > 0) {
			GameObject separator = (GameObject)GameObject.Instantiate(separatorLine);
			separator.transform.SetParent(transform, false);
		}
		elementCount++;
		item.transform.SetParent (transform, false);
		item.artID = (art.ID == null ? -1 : art.ID);
		item.artArtist = "Feliciaan";
		//item.artArtist = "Yolo"; //art.owner.name;
		item.artDescription = (art.description == null ? "" : art.description);
		item.artTitle = (art.name == null ? "" : art.name);
		//print (art.name);

		item.owner = (art.owner.ID == userID);

		item.artPopUp = artPopUp;
		item.popUpImage = popUpImage;
		item.popUpArtist = popUpArtist;
		item.popUpArtistInput = popUpArtistInput;
		item.popUpDescription = popUpDescription;
		item.popUpDescriptionInput = popUpDescriptionInput;
		item.popUpNormal = popUpNormal;
		item.popUpOwner = popUpOwner;
		item.popUpTitle = popUpTitle;
		item.popUpTitleInput = popUpTitleInput;
		item.artWork = art;

		item.UpdateLabels();
	}

	void GetUserID() {
		ArtistController control = ArtistController.Instance;
		control.GetConnectedArtists ((success) => {
			ArtListItem[] items = GetComponentsInChildren<ArtListItem>();
			foreach(API.Artist a in success) {
				userID = a.ID;
			}
			foreach(ArtListItem item in items) {
				item.owner = (userID == item.artID);
				item.UpdateLabels();
			}
		});
	}
}

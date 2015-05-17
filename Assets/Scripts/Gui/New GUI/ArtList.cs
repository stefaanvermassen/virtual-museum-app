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
	public ArtEditPanel popUpOwner;
	public GUIControl popUpQR;
	public QRView popUpQRView;

	private bool started = false;
	GameObject listItem;
	GameObject separatorLine;
	int elementCount;
	public int userID = -1;
	public string userName = "";
	
	public QRGenerator generator;
	
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
		for (int i = transform.childCount - 1; i >= 0; --i) {
			GameObject.Destroy(transform.GetChild(i).gameObject);
		}
		transform.DetachChildren();
		
		elementCount = 0;
	}
	
	public void InitList() {
		EventHandler handler = new EventHandler (OnArtLoaded);
		ClearList ();
		Catalog.RefreshArtWork (handler);		
	}

	public void OnArtSaved(object sender, EventArgs e) {
		InitList ();
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
		item.list = this;
		item.artID = (art.ID == 0 ? -1 : art.ID); // all IDs in the db start from one
		item.artArtist = (art.owner.name == null ? "" : art.owner.name);
		item.artDescription = (art.description == null ? "" : art.description);
		item.artTitle = (art.name == null ? "" : art.name);

		item.owner = (art.owner.ID == userID);

		item.artPopUp = artPopUp;
		item.popUpImage = popUpImage;
		item.popUpArtist = popUpArtist;
		item.popUpDescription = popUpDescription;
		item.popUpNormal = popUpNormal;
		item.popUpOwner = popUpOwner;
		item.popUpTitle = popUpTitle;
		item.artWork = art;

		if (Application.loadedLevelName.Equals ("BuildMuseum")) {
			BuildMuseumActions actions = FindObjectOfType<BuildMuseumActions> ();
			if (actions != null && (actions.GetArt() == item.artID)) {
				Button butt = item.GetComponent<Button>();
				ColorBlock colors = butt.colors;
				colors.normalColor = new Color(Color.green.r, Color.green.g, Color.green.b, 75f/255f);
				colors.highlightedColor = new Color(Color.green.r, Color.green.g, Color.green.b, 150f/255f);
				colors.pressedColor = new Color(Color.green.r, Color.green.g, Color.green.b, 200f/255f);
				butt.colors = colors;
			}
		}

		item.UpdateLabels();
	}

	void GetUserID() {
		ArtistController control = ArtistController.Instance;
		control.GetConnectedArtists ((success) => {
			ArtListItem[] items = GetComponentsInChildren<ArtListItem>();
			foreach(API.Artist a in success) {
				if(userID == -1) {
				userID = a.ID;
				userName = a.Name;
				}
			}
			foreach(ArtListItem item in items) {
				item.owner = (userID == item.artID);
				item.UpdateLabels();
			}
		});
	}

	public void NewArt() {
		MainMenuActions actions = FindObjectOfType<MainMenuActions> ();
		if (actions != null) {
			actions.ResetArtID();
		}
		popUpOwner.artListItem = null;
		popUpOwner.gameObject.SetActive(true);
		popUpNormal.gameObject.SetActive(false);
		artPopUp.FlipCloseOpen ();
	}
}

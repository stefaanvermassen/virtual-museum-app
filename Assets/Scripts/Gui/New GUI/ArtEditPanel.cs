using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using API;
using System;

public class ArtEditPanel : FileBrowserListener {

	public CustomInputField title;
	public CustomInputField description;
	public GUIControl artPopUp;
	public ArtListItem artListItem = null;
	public ImageHighlightButton saveButton;
	public ImageHighlightButton deleteButton;
	public Button artUploadButton;
	public Image bigImage;
	public ArtList artList;
    public Toast toast;
    public Text selectImageText;
	bool imageChanged = false;
	string imagePathSource;
	byte[] imageFile;
	Art art;


	void OnEnable() {
		deleteButton.gameObject.SetActive (false);
        selectImageText.text = "Select an image file";
		if (artListItem == null) {
			//deleteButton.gameObject.SetActive(false);
			saveButton.gameObject.SetActive(false);
			title.text = "";
			description.text = "";
			artUploadButton.gameObject.SetActive(true);
			ArtListItem.SetEmptyBigImage(bigImage, 600, 300, 600, 300);
			art = new Art();
			imageChanged = false;
		} else {
			//deleteButton.gameObject.SetActive(true);
			saveButton.gameObject.SetActive(true);
			title.text = artListItem.artTitle;
			description.text = artListItem.artDescription;
			artUploadButton.gameObject.SetActive(false);
			artListItem.LoadBigImage(bigImage, 600, 300);
			art = artListItem.artWork;
			imageChanged = false;
		}
	}

	void Update () {
		saveButton.gameObject.SetActive (Changed());
	}

	bool Changed() {
		bool saveActive = true;
		saveActive = saveActive && (title.text != null && title.text.Length > 0);
		if (artListItem != null) {
			bool changed = (!title.text.Equals (artListItem.artTitle) || !description.text.Equals(artListItem.artDescription) || imageChanged);
			saveActive = saveActive && changed;
		} else {
			saveActive = saveActive && imageChanged;
		}
		return saveActive;
	}

	public void Delete() {
		if (artListItem != null) {

		}
		artListItem = null;
		artList.InitList ();
		artPopUp.FlipCloseOpen ();
	}

	public void Save() {
		bool changed = Changed ();
		if (changed) {
			art.name = title.text;
			art.description = description.text;
			if(imageChanged) {
				art.imageFile = new byte[imageFile.Length];
				Array.Copy(imageFile,art.imageFile,imageFile.Length);
				//art.imageFile = imageFile;
				art.imagePathSource = imagePathSource;
			}
		}
		if (artListItem == null) {
			art.owner.name = artList.userName;
			art.owner.ID = artList.userID;
		}
		//saving could lead to an upload, thus it shouldn be done if not necessary
		if (changed) {
			art.Save();
			art.SaveRemote(artList.OnArtSaved);
            AddCredits();
		}
		if (artListItem != null) {
			artListItem = null;
		}
		//artList.InitList ();
		artPopUp.FlipCloseOpen ();
    }

    private void AddCredits()
    {
        var cc = API.CreditController.Instance;
        var addedartworkcreditmodel = new API.CreditModel(){ Action = API.CreditActions.ADDEDARTWORK};
        cc.AddCredit(addedartworkcreditmodel, (info) => {
        if (info.CreditsAdded) { // check if credits are added
            toast.Notify("Thank you for sharing. Your total amount of tokens is: " + info.Credits);
        } else {
            Debug.Log("No tokens added for new build museum action.");
        }
        }, (error) => {
            Debug.Log("An error occured when adding tokens for the user.");
        });
    }

	public override void FileIsSelected ()
	{
		//An image can only be added not edited
		if(artListItem == null && FileBrowser.PathIsValid (fileBrowser.GetSelectedFile ())) {
			imagePathSource = fileBrowser.GetSelectedFile ();
			imageFile = File.ReadAllBytes (fileBrowser.GetSelectedFile ());
			if(imageFile != null && imagePathSource != null && imagePathSource.Length > 0) {
                selectImageText.text = "";
				imageChanged = true;
				Texture2D texture = new Texture2D (1, 1);
				texture.LoadImage (imageFile);
				ArtListItem.SetBigImage(bigImage,texture,600,300);
			}
		}
		
	}

	public void SelectImage() {
		fileBrowser.Open (this);
	}
}

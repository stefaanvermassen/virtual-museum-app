using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using AssemblyCSharp;
using System.Collections.Generic;

//class responsible for synchronizing user input data and local data
public class ArtworkGUIData : FileBrowserListener
{
	//GUI fields
	public InputField nameInput;
	public InputField artistInput;
	public Image thumbNail;

	//internal values fields
	private Art artWork;

	//TODO
	//not input field with ID but just the artistID of the current user
	//set load image button to non interactable if an image binary is present

	//upload image fields
	private string imagePathSource;
	private byte[] imageFile;

	public override void FileIsSelected ()
	{
		imagePathSource = fileBrowser.GetSelectedFile ();
		imageFile = File.ReadAllBytes (fileBrowser.GetSelectedFile ());
		UpdateGUI ();
	}

	public void UpdateGUI ()
	{
		//update properties
		nameInput.text = Name;
		//update image
		thumbNail.enabled = true;
		Texture2D texture = new Texture2D (0, 0);
		texture.LoadImage (imageFile);
		thumbNail.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);

	}

	public void ReadFromGUI ()
	{
		Name = nameInput.text;
	}

	public ArtworkGUIData ()
	{
		artWork = new Art ();
	}

	public void Init (Art artWork, byte[] imageFile)
	{
		Debug.Log (artWork.name + " " + imageFile);
		CopyArtWork (artWork);

		this.imageFile = imageFile;
		UpdateGUI ();
	}

	private void CopyArtWork (Art artWork)
	{
		this.artWork = new Art ();
		this.artWork.ID = artWork.ID;
		this.artWork.name = artWork.name;
		this.artWork.owner = artWork.owner;
		this.artWork.tags = artWork.tags;
		this.artWork.genres = artWork.genres;
	}

	private bool HasID ()
	{
		return ArtWorkID != 0;
	}

	public byte[] ImageFile {
		get {
			return this.imageFile;
		}
	}

	public Art ArtWork {
		get {
			return this.artWork;
		}
	}

	public string Name {
		get {
			return this.artWork.name;
		}
		set {
			this.artWork.name = value;
		}
	}

	public int ArtWorkID {
		get {
			return this.artWork.ID;
		}
	}

	public int ArtistID {
		get {
			return this.artWork.owner.ID;
		}
	}

	public void Upload ()
	{
		if (Name == "") {
			Debug.Log ("Empty name not allowed");
			return;
		}
		if (imageFile == null || string.IsNullOrEmpty (imagePathSource)) {
			Debug.Log ("No image selected.");
			return;
		}
		StartCoroutine (PostArt ());
	}

	//Warning: still crashes Unity
	//post the edited art 
	private IEnumerator PostArt ()
	{
		//check if ID present
		API.ArtworkController ac = API.ArtworkController.Instance;
		Debug.Log ("test" + artWork.name + " " + ArtWorkID);
		if (!HasID ()) {
			//upload artwork image
			string[] splitted = imagePathSource.Split (new char[]{'.'});
			string mime = splitted [splitted.Length - 1];
			splitted = imagePathSource.Split (new char[] { '/', '\\' });
			string name = splitted [splitted.Length - 1];
			HTTP.Request request = ac.UploadImage (name, mime, imagePathSource, this.imageFile, 

			                ((artworkResponse) => {
				//set id received from server
				Debug.Log ("receivedId " + artworkResponse.ArtWorkID);
				//only save arwork id
				this.artWork.ID = artworkResponse.ArtWorkID;
				//upload new artwork info in closure

				Debug.Log ("Upload image was succesfull");}), 
			                ((error) => {
				throw new UploadFailedException ("Failed to upload artwork image.");
			}));
			//wait for image to be uploaded
			while (!request.isDone) {
				//wait
				//TODO does not work
			}
		} 

		Debug.Log ("to send name " + Name);
		//once id present update art info
		ac.UpdateArtWork (this.artWork, ((response) => {
			Debug.Log ("Update Artwork info successfull");}), 
			                  ((error) => {
			throw new UploadFailedException ("Failed to update artwork info.");
		}));




		yield return null;

	}
}

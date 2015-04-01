using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using AssemblyCSharp;

//class responsible for synchronizing user input data and local data
public class ArtworkData : FileBrowserListener
{

	//GUI fields
	public InputField nameInput;
	public InputField artistInput;
	public Image thumbNail;

	//internal values fields
	private API.ArtWork artWork;

	//TODO
	//not input field with ID but just the artistID of the current user
	//set load image button to non interactable if an image binary is present

	//upload image fields
	private string imagePathSource;
	private byte[] imageFile;

	public override void fileIsSelected ()
	{
		imagePathSource = fileBrowser.getSelectedFile ();
		imageFile = File.ReadAllBytes (fileBrowser.getSelectedFile ());
		updateGUI ();

	}

	public void updateGUI ()
	{
		//update properties
		nameInput.text = Name;
		//update image
		if (imageFile != null) {
			thumbNail.enabled = true;
			Texture2D texture = new Texture2D (0, 0);
			texture.LoadImage (imageFile);
			thumbNail.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);
		}


	}

	public void readFromGUI ()
	{
		Name = nameInput.text;
	}

	public ArtworkData ()
	{
		artWork = new API.ArtWork ();
	}

	public void init (API.ArtWork artWork, byte[] imageFile)
	{
		Debug.Log (artWork.Name + " " + imageFile);
		copyArtWork (artWork);

		this.imageFile = imageFile;
		updateGUI ();
	}

	private void copyArtWork (API.ArtWork artWork)
	{
		this.artWork = new API.ArtWork ();
		this.artWork.ArtWorkID = artWork.ArtWorkID;
		this.artWork.Name = artWork.Name;
		this.artWork.ArtistID = artWork.ArtistID;
	}

	private bool hasID ()
	{
		return ArtWorkID != 0;
	}

	public byte[] ImageFile {
		get {
			return this.imageFile;
		}

	}

	public API.ArtWork ArtWork {
		get {
			return this.artWork;
		}

	}

	public string Name {
		get {
			return this.artWork.Name;
		}
		set {
			this.artWork.Name = value;
		}
	}

	public int ArtWorkID {
		get {
			return this.artWork.ArtWorkID;
		}

	}

	public int ArtistID {
		get {
			return this.artWork.ArtistID;
		}

	}

	public void upload ()
	{
		if (Name == "" ) {
			Debug.Log("Empty name not allowed");
			return;
		}
		if (imageFile == null || imagePathSource=="" || imagePathSource==null) {
			Debug.Log("No image selected.");
			return;
		}
		StartCoroutine (postArt ());
	}
	//Warning: still crashes Unity
	//post the edited art 
	IEnumerator postArt ()
	{
		//check if ID present
		API.ArtworkController ac = API.ArtworkController.Instance;
		Debug.Log ("tesete" + artWork.Name + " " + ArtWorkID);
		if (!hasID ()) {
			//upload artwork image
			string[] splitted = imagePathSource.Split (new char[]{'.'});
			string mime = splitted [splitted.Length - 1];
			splitted = imagePathSource.Split (new char[] { '/', '\\' });
			string name = splitted [splitted.Length - 1];
			HTTP.Request request= ac.UploadImage (name, mime, imagePathSource, this.imageFile, 
			                ((artworkResponse) => {
				//set id received from server
				Debug.Log ("receivedId " + artworkResponse.ArtWorkID);
				//only save arwork id
				this.artWork.ArtWorkID = artworkResponse.ArtWorkID;
				//upload new artwork info in closure

				Debug.Log ("Upload image was succesfull");}), 
			                ((error) => {
				throw new UploadFailedException ("Failed to upload artwork image.");
			}));
			//wait for image to be uploaded
			while (!request.isDone){
				//wait
				//TODO does not work
			}

		} 
			Debug.Log("to send name "+Name);
			//once id present update art info
			ac.UpdateArtWork (this.artWork, ((response) => {
				Debug.Log ("Update Artwork info successfull");}), 
			                  ((error) => {
				throw new UploadFailedException ("Failed to update artwork info.");
			}));




		yield return null;

	}


}

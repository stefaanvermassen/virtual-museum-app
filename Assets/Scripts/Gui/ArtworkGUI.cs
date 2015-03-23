using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using AssemblyCSharp;
//class responsible for synchronizing user input data and local data
public class ArtworkGUI : FileBrowserListener
{

	private API.ArtWork artWork;
	public InputField nameInput;
	public Image thumbNail;
	//TODO
	//not input field with ID but just the artistID of the current user
	//set load image button to non interactable if an image binary is present
	public InputField artistInput;

	//upload image fields
	private string imagePathSource;
	private byte[] imageFile;

	public override void fileIsSelected ()
	{
		imagePathSource = fileBrowser.getSelectedFile ();
		imageFile = File.ReadAllBytes (fileBrowser.getSelectedFile ());
		update ();

	}

	public void update ()
	{
		//update properties
		Name = nameInput.text;
		//update image
		if (imageFile != null) {
			thumbNail.enabled = true;
			Texture2D texture = new Texture2D (0, 0);
			texture.LoadImage (imageFile);
			thumbNail.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);
		}


	}
	public ArtworkGUI(){
		artWork = new API.ArtWork ();
	}
	public void init(API.ArtWork artWork, byte[] ImageFile){
		this.artWork = artWork;
		this.imageFile = imageFile;
	}




	private bool hasID(){
		return ArtWorkID != 0;
	}
	public byte[] ImageFile {
		get {
			return this.ImageFile;
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
			artWork.Name = value;
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
		StartCoroutine (postArt ());
	}
	//Warning: still crashes Unity
	//post the edited art 
	IEnumerator postArt ()
	{
		//check if ID present
		API.ArtworkController ac = API.ArtworkController.Instance;


		if (!hasID()) {
			//upload artwork image
			string[] splitted = imagePathSource.Split(new char[]{'.'});
			string mime = splitted[splitted.Length - 1];
			splitted = imagePathSource.Split(new char[] { '/', '\\' });
			string name = splitted[splitted.Length - 1];
			ac.uploadImage (name, mime, imagePathSource, ImageFile, 
			                ((artworkResponse) => {
				//set id received from server
				artWork.ArtWorkID=artworkResponse.ArtWorkID;
				Debug.Log ("Upload was succesfull");}), 
			                ((error) => {
				throw new UploadFailedException("Failed to upload artwork image.");
				}));

		} 

		//once id present update art info
		ac.updateArtWork (artWork, ((response) => {
			Debug.Log ("Adding Artwork successfull");}), 
		                  ((error) => {
			throw new UploadFailedException("Failed to upload artwork image.");
			}));


		yield return null;

	}


}

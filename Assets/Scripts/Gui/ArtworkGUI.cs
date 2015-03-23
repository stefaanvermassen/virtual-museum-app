using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using AssemblyCSharp;
//class responsible for synchronizing user input data and local data
public class ArtworkGUI : FileBrowserListener
{

	private API.ArtWork artWork;
	private byte[] imageFile;
	public InputField name;
	//TODO
	//not input field with ID but just the artistID of the current user
	public InputField artist;
	public Image thumbNail;

	public override void fileIsSelected ()
	{
		imageFile = File.ReadAllBytes (fileBrowser.getSelectedFile ());
		update ();
	}

	public void update ()
	{
		//update properties
		name.text = Name;
		artist.text = ArtistID;
		//update image
		if (imageFile != null) {
			thumbNail.enabled = true;
			Texture2D texture = new Texture2D (0, 0);
			texture.LoadImage (imageFile);
			thumbNail.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);
		}


	}

	public ArtworkGUI (string ArtWorkID, string ArtistID, string Name, byte[] imageFile)
	{
		artWork = new API.ArtWork ();
		artWork.ArtWorkID = ArtWorkID;
		artWork.ArtistID = ArtistID;
		artWork.Name = Name;
		this.imageFile = imageFile;
	}

	public ArtworkGUI ()
	{
		artWork = new API.ArtWork ();
		artWork.ArtWorkID = "";
		artWork.ArtistID = "";
		artWork.Name = "";

	}

	private bool hasID(){
		return ArtWorkID != "";
	}
	public byte[] ImageFile {
		get {
			return this.ImageFile;
		}
		set {
			if(hasID()){
				throw new IllegalOperationException("An image of an artwork can't be edited.");
			}
			ImageFile = value;
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

	public string ArtWorkID {
		get {
			return this.artWork.ArtWorkID;
		}

	}

	public string ArtistID {
		get {
			return this.artWork.ArtistID;
		}
		set {
			artWork.ArtistID = value;
		}
	}

	public void upload ()
	{
		StartCoroutine (postArt ());
	}
	//post the edited art 
	IEnumerator postArt ()
	{
		//check if ID present
		API.ArtworkController ac = API.ArtworkController.Instance;


		if (!hasID()) {
			//upload artwork image
			
			ac.uploadImage (Name, "", "", ImageFile, 
			                ((response) => {
				Debug.Log (response.Text);
				artWork.ArtWorkID=response.Text;
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

	public void copy (ArtworkGUI artToCopy)
	{
		artWork = artToCopy.ArtWork;
		ImageFile = artToCopy.ImageFile;

	}
}

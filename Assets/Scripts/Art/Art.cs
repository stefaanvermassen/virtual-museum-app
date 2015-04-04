using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using API;

public class Art : Savable<Art, ArtData>
{

    public int ID { get; set; }
    public string name;
    public string description;
    public User owner;
    public List<string> tags = new List<string>();
    public List<string> genres = new List<string>();
	public Texture2D image { get; set; }


	//upload image fields
	public string imagePathSource;
	public byte[] imageFile;


    public Art() {
        owner = new User();
    }

    /// <summary>
    /// Create an ArtData for serialization.
    /// </summary>
    /// <returns>The ArtData</returns>
    public ArtData Save()
    {
        var userData = owner.Save();
        //var imageData = image.EncodeToPNG();
        return new ArtData(ID, name, description, userData, tags, genres, null);
    }

    /// <summary>
    /// Load an ArtData in this art.
    /// </summary>
    /// <param name="data">The ArtData</param>
    public void Load(ArtData data)
    {
        ID = data.ID;
        name = data.Name;
        description = data.Description;
        owner.Load(data.Owner);
        tags = data.Tags;
        genres = data.Genres;
        image = new Texture2D(1, 1);
        image.LoadImage(data.Image);
    }

    public string getFolder() {
        return "Art";
    }
    public string getFileName() {
        return name;
    }
    public string getExtension(){
        return ".art";   
    }
	/// <summary>
	/// Determines whether this instance has an ID.
	/// This is important, because an ID is aquired zhen uploading tha artwork image
	/// </summary>
	/// <returns><c>true</c> if this instance has I; otherwise, <c>false</c>.</returns>
	private bool HasID ()
	{
		return ID != 0;
	}
	private HTTP.Request UploadImage(API.ArtworkController cont){
		HTTP.Request request=null;
		if (!HasID ()) {
			//upload artwork image
			string[] splitted = imagePathSource.Split (new char[]{'.'});
			string mime = splitted [splitted.Length - 1];
			splitted = imagePathSource.Split (new char[] { '/', '\\' });
			string name = splitted [splitted.Length - 1];
			 request = cont.UploadImage (name, mime, imagePathSource, imageFile, 
			       (art)=> {
				//set id received from server
				Debug.Log ("receivedId " + art.ArtWorkID);
				this.ID = art.ArtWorkID;
			}, 
			(error) => {
				throw new UploadFailedException ("Failed to update artwork image.");
			}
			);      

		} 
		return request;
	}
	private void UploadMetaData(API.ArtworkController cont){
		//local data is wrapped in API class to upload

		API.ArtWork apiArt = API.ArtWork.FromArt (this);
		//once id present update art info
		cont.UpdateArtWork (apiArt, 
		(art)=> {
			Debug.Log ("Update Artwork info successfull");
		}, 
		(error) => {
			throw new UploadFailedException ("Failed to update artwork info.");
		}
		);
	}

    public void SaveRemote() {
		Debug.Log("Start saving Remote");
		API.ArtworkController cont = API.ArtworkController.Instance;
		//TODO make sure a user is logged in
		//TODO: use async loader

		HTTP.Request req = UploadImage(cont);
		Debug.Log (req.isDone);
		AsyncLoader loader = AsyncLoader.CreateAsyncLoader(
			() => {
			Debug.Log("Started");
		},() => {
			//either wait for image to be uploaded or there was nu upload necessary
			return req==null || req.callbackCompleted;
		},
		() => {
			Debug.Log("Still loading");
		},
		() => {
			Debug.Log("Loaded");
			UploadMetaData(cont);

		});

		
	}
	public void LoadRemote(string identifier) {
		ArtworkController.Instance.GetArtwork(
            identifier, 
            success: (art) => {
                ID = art.ArtWorkID;
                name = art.Name;
                Debug.Log(name);
                },
            error: (error) => { Debug.Log("NOPE"); }
        );
        ArtworkController.Instance.GetArtworkData(
            identifier,
            success: (art) => {
                image = new Texture2D(1, 1);
                image.LoadImage(art);
            },
            error: (error) => { Debug.Log("NOPE"); }
        ); 
    }
	public void LoadRemote(int identifier) {
		LoadRemote (identifier + "");
	}
    public DateTime LastModified(string identifier) {
        return DateTime.Now;
    }

}

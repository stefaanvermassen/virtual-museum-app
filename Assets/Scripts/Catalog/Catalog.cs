using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public static class Catalog
{
	public enum CatalogType
	{
		OBJECT,
		WALL,
		FLOOR,
		CEILING,
		FRAME,
		ART
	}

	private static Dictionary<int, Art> artworksDictionary = new Dictionary<int, Art> ();
	public static string[] objects = new string[] { "texmonkey", "Vase1", "Statue",
    "Lamp"
    };
	public static string[] walls = new string[] {"Wall1", "Wall2", "Wall3"
    };
	public static string[] ceilings = new string[] {"Ceiling1", "Ceiling2", "Ceiling3"
    };
	public static string[] floors = new string[] {"Floor1", "Floor2", "Floor3", "Floor4"
    };
	public static string[] frames = new string[] {"Frame1", "Frame2"
	};
	private static Dictionary<int, GameObject> objectDictionary = new Dictionary<int, GameObject> ();
	private static Dictionary<int, GameObject> wallDictionary = new Dictionary<int, GameObject> ();
	private static Dictionary<int, GameObject> ceilingDictionary = new Dictionary<int, GameObject> ();
	private static Dictionary<int, GameObject> floorDictionary = new Dictionary<int, GameObject> ();
	private static Dictionary<int, GameObject> frameDictionary = new Dictionary<int, GameObject> ();

	private static GameObject GetResource (int id, Dictionary<int, GameObject> dictionary)
	{
		return dictionary [id];
	}

	public static int[] getResourceIDs (CatalogType type)
	{
		switch (type) {
		case Catalog.CatalogType.OBJECT:
			return objectDictionary.Keys.ToArray ();
		case Catalog.CatalogType.FRAME:
			return frameDictionary.Keys.ToArray ();

		case Catalog.CatalogType.WALL:
			return wallDictionary.Keys.ToArray ();

		case Catalog.CatalogType.FLOOR:
			return floorDictionary.Keys.ToArray ();

		case Catalog.CatalogType.CEILING:
			return ceilingDictionary.Keys.ToArray ();

		case Catalog.CatalogType.ART:
			return artworksDictionary.Keys.ToArray ();
		default:
			return null;
		}
	}

	public static GameObject GetObject (int objectID)
	{
		return GetResource (objectID, objectDictionary);
	}

	public static GameObject GetWall (int objectID)
	{
		return GetResource (objectID, wallDictionary);
	}

	public static GameObject GetCeiling (int objectID)
	{
		return GetResource (objectID, ceilingDictionary);
	}

	public static GameObject GetFloor (int objectID)
	{
		return GetResource (objectID, floorDictionary);
	}
	//check zith timestamp if catalog changed
	private static bool catalogArtChanged;
	//this method should check the server if there were objects added and if so start an update
	//use timestamp to check periodically if the catalog has to be updated
	public static void Refresh ()
	{
		//load all objects
		string folder = "Objects";
		loadResource (folder, objectDictionary, objects);
		folder = "Styles";
		loadResource (folder, floorDictionary, floors);
		loadResource (folder, wallDictionary, walls);
		loadResource (folder, ceilingDictionary, ceilings);
		folder = "Frames";
		loadResource (folder, frameDictionary, frames);
		//load all artworks
		RefreshArtWork ();

	}

	private static void loadResource (string folder, Dictionary<int, GameObject> dict, string[] names)
	{
		for (int id=0; id<names.Length; id++) {
			if(!dict.ContainsKey(id)){
				dict.Add (id, (GameObject)Resources.Load<GameObject> (folder + "/" + names [id]));
			}
		}
	}

	private static bool hasArt (int artID)
	{
		return artworksDictionary.ContainsKey (artID);

	}
	//TODO: add filters on collection of requested art
	//warning this method waits to finish, this should always be called 
	/// <summary>
	/// Loads all art from server available to user.
	/// A filter can be applied to refine the scope of the collection.
	/// </summary>
	/// <returns>A collection Art.</returns>
	public static void RefreshArtWork (EventHandler eventHandler = null)
	{
		Debug.Log ("start");
		//TODO make sure a user is logged in
		API.ArtworkController ac = API.ArtworkController.Instance;
		//load all artworks
		AsyncLoader loader = AsyncLoader.CreateAsyncLoader (
			() => {
			Debug.Log ("Started");
			ac.GetAllArtworks (success: (response) => {
				foreach (API.ArtWork child in response) {
					//we save the child, because else it is overwritten in the local scope of the closure
					var artwork = child;
					//check if catalog has it
					if (!hasArt (artwork.ArtWorkID)) {
						Art newArt = API.ArtWork.ToArt(artwork);
						//Art newArt = new Art ();
						if(eventHandler != null) {
							newArt.ArtLoaded += eventHandler;
						}
						
						Storage.Instance.Load (newArt, artwork.ArtWorkID + "");
						//we use the id from the artwork instance because there's no guarantee for the newart instance to be loaded already
						artworksDictionary.Add (artwork.ArtWorkID, newArt);
					} else {
						if(eventHandler != null) {
							Art art = getArt (artwork.ArtWorkID);
							if(getArt (artwork.ArtWorkID).loadingImage) {
								art.ArtLoaded += eventHandler;
							} else {
								eventHandler(art, new EventArgs());
							}
						}
					}
				}
			},
			error: (error) => {
				Debug.Log ("An error occured while loading all artworks");
			}); 
		},
		() => {
		},
		() => {
			Debug.Log ("Loaded");

		});

	}

	public static GameObject GetFrame (int objectID)
	{
		return GetResource (objectID,  frameDictionary);
	}

	public static Art getArt (int artID)
	{
		return artworksDictionary [artID];
	}

	public static Dictionary<int, Art> getAllArt ()
	{
		return artworksDictionary;
	}
}

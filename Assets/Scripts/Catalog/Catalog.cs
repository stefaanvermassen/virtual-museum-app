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
    "Lamp", "Column", "Chair", "PotPlant"
    };
	public static string[] walls = new string[] {"Wall1", "Wall2", "Wall3", "Wall4"
    };
	public static string[] ceilings = new string[] {"Ceiling1", "Ceiling2", "Ceiling3",
	"Ceiling4"
    };
	public static string[] floors = new string[] {"Floor1", "Floor2", "Floor3", "Floor4",
	"Floor5"
    };
	public static string[] frames = new string[] {"Frame1", "Frame2"
	};
	private static Dictionary<int, GameObject> objectDictionary = new Dictionary<int, GameObject> ();
	private static Dictionary<int, GameObject> wallDictionary = new Dictionary<int, GameObject> ();
	private static Dictionary<int, GameObject> ceilingDictionary = new Dictionary<int, GameObject> ();
	private static Dictionary<int, GameObject> floorDictionary = new Dictionary<int, GameObject> ();
	private static Dictionary<int, GameObject> frameDictionary = new Dictionary<int, GameObject> ();
	/// <summary>
	/// Gets the resource.
	/// </summary>
	/// <returns>The resource.</returns>
	/// <param name="id">Identifier.</param>
	/// <param name="dictionary">Dictionary.</param>
	private static GameObject GetResource (int id, Dictionary<int, GameObject> dictionary)
	{
		return dictionary [id];
	}
	/// <summary>
	/// Gets the resource IDs.
	/// </summary>
	/// <returns>The resource I ds.</returns>
	/// <param name="type">Type.</param>
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
	/// <summary>
	/// Gets the object.
	/// </summary>
	/// <returns>The object.</returns>
	/// <param name="objectID">Object I.</param>
	public static GameObject GetObject (int objectID)
	{
		return GetResource (objectID, objectDictionary);
	}
	/// <summary>
	/// Gets the wall.
	/// </summary>
	/// <returns>The wall.</returns>
	/// <param name="objectID">Object I.</param>
	public static GameObject GetWall (int objectID)
	{
		return GetResource (objectID, wallDictionary);
	}
	/// <summary>
	/// Gets the ceiling.
	/// </summary>
	/// <returns>The ceiling.</returns>
	/// <param name="objectID">Object I.</param>
	public static GameObject GetCeiling (int objectID)
	{
		return GetResource (objectID, ceilingDictionary);
	}
	/// <summary>
	/// Gets the floor.
	/// </summary>
	/// <returns>The floor.</returns>
	/// <param name="objectID">Object I.</param>
	public static GameObject GetFloor (int objectID)
	{
		return GetResource (objectID, floorDictionary);
	}
	//check with timestamp if catalog changed
	private static bool catalogArtChanged;


	/// <summary>
	/// Refresh this instance.
	/// This method should check the server if there were objects added and if so start an update
	/// Use timestamp to check periodically if the catalog has to be updated
	/// </summary>
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

	/// <summary>
	/// Loads the resource.
	/// </summary>
	/// <param name="folder">Folder.</param>
	/// <param name="dict">Dict.</param>
	/// <param name="names">Names.</param>
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
	/// <summary>
	/// Loads all art from server available to user.
	/// A filter can be applied to refine the scope of the collection.
	/// warning this method waits to finish, this should always be called 
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
	/// <summary>
	/// Gets the frame.
	/// </summary>
	/// <returns>The frame.</returns>
	/// <param name="objectID">Object I.</param>
	public static GameObject GetFrame (int objectID)
	{
		return GetResource (objectID,  frameDictionary);
	}
/// <summary>
/// Gets the art.
/// </summary>
/// <returns>The art.</returns>
/// <param name="artID">Art I.</param>
	public static Art getArt (int artID)
	{
		return artworksDictionary [artID];
	}
	/// <summary>
	/// Gets all art.
	/// </summary>
	/// <returns>The all art.</returns>
	public static Dictionary<int, Art> getAllArt ()
	{
		return artworksDictionary;
	}
}

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public static class Catalog {
	private static Dictionary<int, Art> artworksDictionary=new Dictionary<int, Art>();
    public static string[] objects = new string[] { "texmonkey", "Vase1", "Statue",
    "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey",
    "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey",
    "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey",
    "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey",
    "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey",
    "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey",
    "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey",
    "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey"
    };

    public static string[] walls = new string[] {"Wall1", "Wall2"
    };

    public static string[] ceilings = new string[] {"Ceiling1", "Ceiling2"
    };

    public static string[] floors = new string[] {"Floor1", "Floor2"
    };

    private static Dictionary<int, GameObject> objectDictionary = new Dictionary<int, GameObject>();
    private static Dictionary<int, GameObject> wallDictionary = new Dictionary<int, GameObject>();
    private static Dictionary<int, GameObject> ceilingDictionary = new Dictionary<int, GameObject>();
    private static Dictionary<int, GameObject> floorDictionary = new Dictionary<int, GameObject>();

    private static GameObject GetResource(int id, string[] names, string folder, Dictionary<int, GameObject> dictionary){
        if (!dictionary.ContainsKey(id)) {
            dictionary.Add(id, (GameObject)Resources.Load<GameObject>(folder+"/"+names[id]));
        }
        return dictionary[id];
    }

    public static GameObject GetObject(int objectID) {
        return GetResource(objectID, objects, "Objects", objectDictionary);
    }
    public static GameObject GetWall(int objectID) {
        return GetResource(objectID, walls, "Styles", wallDictionary);
    }
    public static GameObject GetCeiling(int objectID) {
        return GetResource(objectID, ceilings, "Styles", ceilingDictionary);
    }
    public static GameObject GetFloor(int objectID) {
        return GetResource(objectID, floors, "Styles", floorDictionary);
    }
	//check zith timestamp if catalog changed
	private static bool catalogArtChanged;
	//this method should check the server if there were objects added and if so start an update
	//use timestamp to check periodically if the catalog has to be updated
	public static void Refresh(){

		RefreshArtWork ();
	}
	private static bool hasArt(int artID){
		return artworksDictionary.ContainsKey (artID);

	}
	//TODO: add filters on collection of requested art
	//warning this method waits to finish, this should always be called 
	/// <summary>
	/// Loads all art from server available to user.
	/// A filter can be applied to refine the scope of the collection.
	/// </summary>
	/// <returns>A collection Art.</returns>
	public static void RefreshArtWork(){
		Debug.Log ("start");
		//TODO make sure a user is logged in
		API.ArtworkController ac = API.ArtworkController.Instance;
		//load all artworks
		ac.GetAllArtworks (success: (response) => {
			foreach (API.ArtWork child in response) {
				//we save the child, because else it is overwwritten in the loval scope of the closure
				var artwork = child;
				//check if catalog has it
				if(!hasArt(artwork.ArtWorkID)){
					Art newArt = new Art();
					//TODO switch back to Load after this that this is fixed in storage
					Storage.Instance.LoadRemote(newArt,artwork.ArtWorkID+"");
					artworksDictionary.Add(newArt.ID,newArt);
				}
			}
		},
		error: (error) => {
			Debug.Log ("An error occured while loading all artworks");
		}); 

	}

	public static Art getArt(int artID){
		return artworksDictionary [artID];
	}
	public static Dictionary<int, Art> getAllArt(){

		return artworksDictionary;
	}
}

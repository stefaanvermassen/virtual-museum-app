using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AddMuseumInfoInit : MonoBehaviour {
    public InputField museumAuthor;
    public InputField museumName;
    public InputField museumDescription;

	// Use this for initialization
	void Start () {
        MuseumData museumData = new MuseumData(new List<MuseumTileData>(), new List<MuseumArtData>(), 
            new List<MuseumObjectData>(), "Ikke", "Fancy", "");
        Museum museum = gameObject.AddComponent<Museum>();
        museum.Load(museumData);
        new AddMuseumInfo(museum, museumAuthor, museumName, museumDescription);
	}
}

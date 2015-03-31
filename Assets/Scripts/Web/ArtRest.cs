using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class ArtRest : MonoBehaviour
{
    private const string BASE_URL = "http://api.awesomepeople.tv/";
    private const string TOKEN = "Token";
    private const string ARTWORK = "api/artwork";

    private WWW www;

	//thread safe 
	IEnumerator  getAllArt (GUIControl content)
	{
		API.UserController uc = API.UserController.Instance;
		API.ArtworkController ac = API.ArtworkController.Instance;
		ac.getAllArtworks (success: (response) => {
			foreach (API.ArtWork child in response) {
				//we save the child, because else it is overwwritten in the loval scope of the closure
				var artwork = child;
				ac.getArtworkData (artwork.ArtWorkID.ToString(), success: (texture) => {
					//the id is differen between th 2 calls
					AddArtToCatalog (artwork,texture, content);
				}, error: (error) => {
					Debug.Log ("An error occured while loading artwork with ID: " + child.ArtWorkID.ToString ());});
				
			}
		},
		error: (error) => {
			Debug.Log ("An error occured while loading all artworks");
		}); 

		yield return null;
	}

    //show new art panel in catalog
	public void AddNewArtToCatalog (GUIControl content)
	{
		//create a clone of the contents child
		GUIControl item = content.AddDynamicChild ();
		
		//add to content of catalog
		content.Add (item);
		//update UI elements
		item.GetComponent<ArtworkData> ().UpdateGUI ();
	}
	
	public void PostArt (GUIControl content)
	{
		for (int i=0; i<content.transform.childCount; i++) {
			content.GetChild (i).GetComponent<ArtworkData> ().Upload ();
		}
	}
	
	private void AddArtToCatalog (API.ArtWork art, byte[] texture, GUIControl content)
	{
		//add new item to catalog
		GUIControl item = content.AddDynamicChild ();
        Art artwork = new Art();
        artwork.name = art.Name;
        artwork.ID = art.ArtWorkID;
        artwork.owner = new User();
        artwork.owner.ID = art.ArtistID;
		//copy info in item
		item.GetComponent<ArtworkData>().Init(artwork, texture);
	}
	
	public void FillCatalogWithAllArt (GUIControl content)
	{
		StartCoroutine (getAllArt (content));
	}
}
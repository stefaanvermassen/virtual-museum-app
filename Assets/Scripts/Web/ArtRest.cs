using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class ArtRest : MonoBehaviour
{
	private WWW www;
	//thread safe 
	IEnumerator  getAllArt (GUIControl content)
	{
		Debug.Log ("t");
		string imageArtworkUrl;
		API.SessionManager sm = API.SessionManager.Instance;
		API.ArtworkController ac = API.ArtworkController.Instance;
		ac.GetAllArtworks (success: (response) => {
			foreach (API.ArtWork child in response) {
				//we save the child, because else it is overwwritten in the loval scope of the closure
				var artwork = child;
				ac.GetArtworkData (artwork.ArtWorkID.ToString(), success: (texture) => {
					//the id is differen between th 2 calls
					addArtToCatalog (artwork,texture, content);
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
	public void addNewArtToCatalog (GUIControl content)
	{
		//create a clone of the contents child
		GUIControl item = content.addDynamicChild ();
		
		//add to content of catalog
		content.add (item);
		//update UI elements
		item.GetComponent<ArtworkData> ().updateGUI ();
	}
	
	public void postArt (GUIControl content)
	{
		for (int i=0; i<content.transform.childCount; i++) {
			content.getChild (i).GetComponent<ArtworkData> ().upload ();
		}
	}
	
	private void addArtToCatalog (API.ArtWork art,byte[] texture, GUIControl content)
	{
		//add new item to catalog
		GUIControl item = content.addDynamicChild ();
		//copy info in item
		item.GetComponent<ArtworkData> ().init (art,texture);

		
	}
	
	public void fillCatalogWithAllArt (GUIControl content)
	{
		
		StartCoroutine (getAllArt (content));
		
	}
	
	private const string BASE_URL = "http://api.awesomepeople.tv/";
	private const string TOKEN = "Token";
	private const string ARTWORK = "api/artwork";
}
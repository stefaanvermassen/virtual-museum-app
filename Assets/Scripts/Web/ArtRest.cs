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
		//crashes Unity!!!!
//		Debug.Log ("t");
//		string imageArtworkUrl;
//		API.UserController uc = API.UserController.Instance;
//		API.ArtworkController ac = API.ArtworkController.Instance;
//		ArtworkGUI artWorkGUI;
//		ac.getAllArtworks (success: (response) => {
//			Debug.Log(response.Count);
//			foreach(Hashtable child in response) {
//				ac.getArtwork(child["ArtWorkID"].ToString(), success:(imageFile) => {
//					artWorkGUI = new ArtworkGUI(child ["ArtWorkID"].ToString(), child ["ArtistID"].ToString(), child ["Name"].ToString(), imageFile);
//					addArtToCatalog (artWorkGUI,content);
//				}, error:(error) => {Debug.Log("An error occured while loading artwork with ID: " + child["ArtWorkID"]);});
//
//			}
//		},
//							error: (error) => {
//			Debug.Log("An error occured while loading all artworks");
//		}); 
		yield return null;
	}
	//show new art panel in catalog
	public void addNewArtToCatalog(GUIControl content){
		//create a clone of the contents child
		GUIControl item = content.addDynamicChild ();

		//add to content of catalog
		content.add (item);
		//update UI elements
		item.GetComponent<ArtworkGUI> ().update ();
	}


	public void postArt(GUIControl content){
		for (int i=0; i<content.transform.childCount; i++) {
			Debug.Log(content.getChild(i).GetComponent<ArtworkGUI> ().ImageFile);
			content.getChild(i).GetComponent<ArtworkGUI> ().upload ();
		}
	}

	private void addArtToCatalog (ArtworkGUI art,GUIControl content)
	{
		//add new item
		GUIControl item = content.addDynamicChild ();
		//copy info in item
		item.GetComponent<ArtworkGUI> ().copy (art);

	//update UI elements
		item.GetComponent<ArtworkGUI> ().update ();
	
	}


	public void fillCatalogWithAllArt (GUIControl content)
	{

		StartCoroutine (getAllArt (content));

	}


	private const string BASE_URL = "http://api.awesomepeople.tv/";
	private const string TOKEN = "Token";
	private const string ARTWORK = "api/artwork";
}
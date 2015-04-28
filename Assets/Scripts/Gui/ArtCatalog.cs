using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using API;
using System;

public class ArtCatalog : MonoBehaviour
{
	public GUIControl catalogContent;
	private bool started = false;

	void Start() {
		Refresh ();
		started = true;
	}

	void OnEnable() {
		if(started) Refresh ();
	}


    //show new art panel in catalog
	public void AddNewArtToCatalog (GUIControl content)
	{
		//create and add a clone of the contents defined child
		GUIControl item = content.AddDynamicChild ();

		//update UI elements
		item.GetComponent<ArtworkGUIData> ().Refresh();
	}
	// do this in IEnumerator, because the upload is blocking for the moment, replace later with asyncloader in the upload of Art
	public void PostArt (GUIControl content)
	{
		for (int i=0; i<content.transform.childCount; i++) {
			content.GetChild (i).GetComponent<ArtworkGUIData> ().Save ();
		}
	}
	
	private void AddArtToCatalog (Art art, GUIControl content)
	{
		//add new item to catalog
		GUIControl item = content.AddDynamicChild ();
       
		//copy info in item
		item.GetComponent<ArtworkGUIData>().Init(art);
	}
	//this method can be optimized a lot by not creating new objects for each refresh but byjust editing them
	private Dictionary<int, Art>.ValueCollection allArt;
	/// <summary>
	/// Refresh the specified content.
	/// </summary>
	/// <param name="content">Content.</param>
	public void Refresh()
	{
		//clear the catalog
		catalogContent.RemoveAllChildren ();
		Catalog.RefreshArtWork (new EventHandler(OnArtLoaded));
		//get all art from catalog
		/*allArt = Catalog.getAllArt ().Values;
		Debug.Log (Catalog.getAllArt ().Count);*/

		//load art in gui, check for each art is it's still loading or not
		/*foreach (Art art in allArt) {
			if(art.loadingImage) {
				art.ArtLoaded += new EventHandler(OnArtLoaded);
			} else {
				AddArtToCatalog(art,catalogContent);
			}
		}*/
	}

	public void OnArtLoaded(object sender, EventArgs e) {
		AddArtToCatalog((Art)sender,catalogContent);
	}
}
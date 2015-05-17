using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using API;
using System;

public class ArtCatalog : StatisticsBehaviour
{
	public GUIControl catalogContent;
	private bool started = false;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		Refresh ();
		started = true;
		screenName = "ArtCatalog";
	}

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable() {
		if(started) Refresh ();
	}


	/// <summary>
	/// Adds the new art to catalog.
	/// </summary>
	/// <param name="content">Content.</param>
	public void AddNewArtToCatalog (GUIControl content)
	{
		//create and add a clone of the contents defined child
		GUIControl item = content.AddDynamicChild ();

		//update UI elements
		item.GetComponent<ArtworkGUIData> ().Refresh();
	}

	/// <summary>
	/// Posts the art.
	/// </summary>
	/// <param name="content">Content.</param>
	public void PostArt (GUIControl content)
	{
		for (int i=0; i<content.transform.childCount; i++) {
			content.GetChild (i).GetComponent<ArtworkGUIData> ().Save ();
		}
	}

	/// <summary>
	/// Adds the art to catalog.
	/// </summary>
	/// <param name="art">Art.</param>
	/// <param name="content">Content.</param>
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

	}
	/// <summary>
	/// Raises the art loaded event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	public void OnArtLoaded(object sender, EventArgs e) {
		AddArtToCatalog((Art)sender,catalogContent);
	}
}
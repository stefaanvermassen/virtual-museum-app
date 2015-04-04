using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using API;

public class ArtCatalog : MonoBehaviour
{




    //show new art panel in catalog
	public void AddNewArtToCatalog (GUIControl content)
	{
		//create and add a clone of the contents defined child
		GUIControl item = content.AddDynamicChild ();

		//update UI elements
		item.GetComponent<ArtworkGUIData> ().WriteToGUI ();
	}
	
	public void PostArt (GUIControl content)
	{
		for (int i=0; i<content.transform.childCount; i++) {
			content.GetChild (i).GetComponent<ArtworkGUIData> ().Upload ();
		}
	}
	
	private void AddArtToCatalog (Art art, GUIControl content)
	{
		//add new item to catalog
		GUIControl item = content.AddDynamicChild ();
       
		//copy info in item
		item.GetComponent<ArtworkGUIData>().Init(art);
	}
	
	public void FillCatalogWithAllArt (GUIControl content)
	{
		List<Art> allArt = Art.LoadRemoteAll ();
		foreach (Art art in allArt) {
			AddArtToCatalog(art,content);
		}
	}
}
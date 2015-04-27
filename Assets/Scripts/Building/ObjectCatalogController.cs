using UnityEngine;
using System.Collections;
using System;

public class ObjectCatalogController : GUIControl
{
	//Unity editor note: for the content to be scrolled to the begin on opening, the pivot of it's rect transform should be set to 1 for Y


	public GUIControl catalogContent;
	// Objects are loaded on initialisation, a collection is dynamic, on the change of a collecition the loadObjects should be called again
	// or load objects every time the catalog is shown

	public void Open (string catalogType)
	{
		Open ();
		OnTop ();
		//change header text
		loadObjects ((Catalog.CatalogType)Enum.Parse (typeof(Catalog.CatalogType), catalogType));
		
	}


	//use objects id'd as strings in catalog to build the gui
	public void loadObjects (Catalog.CatalogType type)
	{

		//empty catalog
		catalogContent.RemoveAllChildren ();
		//fill with objects from collection
		int[] ids = Catalog.getResourceIDs (type);
		Debug.Log (ids.Length);
		for (int i = 0; i < ids.Length; i++) {
			GUIControl item = catalogContent.AddDynamicChild ();
			ObjectCatalogItemController controller = item.GetComponent<ObjectCatalogItemController> ();
			controller.init (ids[i], type);
		}
		//position content on topcatalogContent.AddDynamicChild ();
		SetRelativePosition (0, 0);
	}

	//the objects array should be loaded from the users collection of objects this is a stub


}

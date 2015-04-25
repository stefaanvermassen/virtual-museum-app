using UnityEngine;
using System.Collections;
using System;

public class ObjectCatalogController : GUIControl
{
	//Unity editor note: for the content to be scrolled to the begin on opening, the pivot of it's rect transform should be set to 1 for Y

	public enum CatalogType
	{
		OBJECT,
		WALL,
		FLOOR,
		CEILING,
		FRAME
	}
	public GUIControl catalogContent;
	// Objects are loaded on initialisation, a collection is dynamic, on the change of a collecition the loadObjects should be called again
	// or load objects every time the catalog is shown

	public void Open (string catalogType)
	{
		Open ();
		OnTop ();
		//change header text
		//TODO
		loadObjects ((CatalogType)Enum.Parse (typeof(CatalogType), catalogType));
		
	}

	private string[] getCollection (CatalogType type)
	{
		switch (type) {
		case CatalogType.OBJECT:
			return (Catalog.objects);
		case CatalogType.FRAME:
			return(Catalog.frames);
		case CatalogType.WALL:
			return(Catalog.walls);
		case CatalogType.FLOOR:
			return (Catalog.floors);
		case CatalogType.CEILING:
			return (Catalog.ceilings);
		default:
			return null;
			
		}
	}
	//use objects id'd as strings in catalog to build the gui
	public void loadObjects (CatalogType type)
	{

		//empty catalog
		catalogContent.RemoveAllChildren ();
		//fill with objects from collection
		for (int i = 0; i < getCollection(type).Length; i++) {
			GUIControl item = catalogContent.AddDynamicChild ();
			ObjectCatalogItemController controller = item.GetComponent<ObjectCatalogItemController> ();
			Debug.Log(controller);
			controller.init (i, type);
		}
		//position content on topcatalogContent.AddDynamicChild ();
		SetRelativePosition (0, 0);
	}

	//the objects array should be loaded from the users collection of objects this is a stub


}

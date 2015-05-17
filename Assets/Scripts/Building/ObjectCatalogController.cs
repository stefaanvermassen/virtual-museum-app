using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ObjectCatalogController : GUIControl
{
	//Unity editor note: for the content to be scrolled to the begin on opening, the pivot of it's rect transform should be set to 1 for Y


	public GUIControl catalogContent;
	public string catalogType;
	// Objects are loaded on initialisation, a collection is dynamic, on the change of a collecition the loadObjects should be called again
	// or load objects every time the catalog is shown

	/// <summary>
	/// Open the specified catalogType.
	/// </summary>
	/// <param name="catalogType">Catalog type.</param>
	public void Open (string catalogType)
	{
		this.catalogType = catalogType;
		Open ();
		//OnTop ();
		
	}
	/// <summary>
	/// Open this instance.
	/// </summary>
	public override void Open() {
		base.Open ();
		loadObjects ((Catalog.CatalogType)Enum.Parse (typeof(Catalog.CatalogType), catalogType));
	}
	
	/// <summary>
	/// Shows the models.
	/// </summary>
	/// <param name="show">If set to <c>true</c> show.</param>
	public void ShowModels(bool show) {
		ObjectCatalogItemController[] items = catalogContent.GetComponentsInChildren<ObjectCatalogItemController>();
		foreach(ObjectCatalogItemController item in items) {
			item.ShowModel(show);
		}
	}

	/// <summary>
	/// Loads the objects.
	/// use objects id'd as strings in catalog to build the gui
	/// </summary>
	/// <param name="type">Type.</param>
	public void loadObjects (Catalog.CatalogType type)
	{

		//empty catalog
		catalogContent.RemoveAllChildren ();
		//fill with objects from collection
		int[] ids = Catalog.getResourceIDs (type);
		//Debug.Log (ids.Length);
		for (int i = 0; i < ids.Length; i++) {
			GUIControl item = catalogContent.AddDynamicChild ();
			ObjectCatalogItemController controller = item.GetComponent<ObjectCatalogItemController> ();
			if (Application.loadedLevelName.Equals ("BuildMuseum")) {
				BuildMuseumActions actions = FindObjectOfType<BuildMuseumActions> ();
				if (actions != null) {
					bool selected = false;
					selected = selected || (type == Catalog.CatalogType.OBJECT && (actions.GetObject() == ids[i]));
					selected = selected || (type == Catalog.CatalogType.WALL && (actions.GetWall() == ids[i]));
					selected = selected || (type == Catalog.CatalogType.FLOOR && (actions.GetFloor() == ids[i]));
					selected = selected || (type == Catalog.CatalogType.CEILING && (actions.GetCeiling() == ids[i]));
					selected = selected || (type == Catalog.CatalogType.FRAME && (actions.GetFrame() == ids[i]));
					if(selected) {
						Button butt = item.GetComponent<Button>();
						ColorBlock colors = butt.colors;
						colors.normalColor = new Color(Color.green.r, Color.green.g, Color.green.b, 75f/255f);
						colors.highlightedColor = new Color(Color.green.r, Color.green.g, Color.green.b, 150f/255f);
						colors.pressedColor = new Color(Color.green.r, Color.green.g, Color.green.b, 200f/255f);
						butt.colors = colors;
					}
				}
			}
			controller.init (ids[i], type);
		}
		//position content on topcatalogContent.AddDynamicChild ();
		SetRelativePosition (0, 0);
	}

	//the objects array should be loaded from the users collection of objects this is a stub


}

using UnityEngine;
using System.Collections;

public class ObjectCatalogController : GUIControl {
	//Unity editor note: for the content to be scrolled to the begin on opening, the pivot of it's rect transform should be set to 1 for Y


	public GUIControl catalogContent;
	// Objects are loaded on initialisation, a collection is dynamic, on the change of a collecition the loadObjects should be called again
	// or load objects every time the catalog is shown
	void Start () {
		loadObjects ();
	}
	
	public void loadObjects(){
		//empty catalog
		catalogContent.removeAllChildren ();
		//fill with objects from collection
		for (int i = 0; i < getNrOfOwnedObjects(); i++) {
			GUIControl item = catalogContent.addDynamicChild();
			ObjectCatalogItemController controller = item.GetComponent<ObjectCatalogItemController>();
			controller.init(i);
		}
		//position content on top
		setRelativePosition (0, 0);
	}
	//the objects array should be loaded from the users collection of objects this is a stub

	public string[] getOwnedObjects(){
		return Catalog.objects;
	}
	public int getNrOfOwnedObjects(){
		return Catalog.objects.Length;
	}
}

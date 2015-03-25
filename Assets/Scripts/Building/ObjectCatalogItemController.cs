using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectCatalogItemController : GUIControl {
	private int index;
	public Button button;
	public DrawController drawController;
	public ObjectCatalogController catalog;

	public void init(int i){
		index = i; //important because addlistener would use i by reference instead of by value

		// on click the index of the chosen object is saved and the catalog closed
		button.onClick.AddListener(() => {
			drawController.SetCurrentObject(index);
			catalog.close();
		});
		var model = Instantiate(Catalog.GetObject(index));
		model.transform.SetParent(button.transform, false);
		model.transform.localPosition = new Vector3(0, -30, -30);
		model.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
		model.transform.localScale = new Vector3(100, 100, 100);
		ChangeLayersRecursively(model.transform, LayerMask.NameToLayer("UI"));
		//after transformation to show 3D object normalise back, it's rotated after adding
		normalise ();

	}
	void ChangeLayersRecursively(Transform trans, int layer) {
		trans.gameObject.layer = layer;
		foreach (Transform child in trans) {
			ChangeLayersRecursively(child,layer);
		}
	}
}

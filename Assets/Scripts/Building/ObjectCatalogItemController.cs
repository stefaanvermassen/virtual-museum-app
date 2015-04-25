using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ObjectCatalogItemController : GUIControl
{
	private int index;
	public Button button;
	public DrawController drawController;
	public ObjectCatalogController catalog;

	public void init (int i, ObjectCatalogController.CatalogType type)
	{
		index = i; //important because addlistener would use i by reference instead of by value
		OnSelect (index, type);
		var model = GetGameObject (index, type);
		ConfigLayout (type);

		model.transform.SetParent (button.transform, false);
		model.transform.localPosition = new Vector3 (position[0], position[1], position[2]);
		model.transform.localRotation = Quaternion.Euler (new Vector3 (rotation[0], rotation[1], rotation[2]));
		model.transform.localScale = new Vector3 (scale, scale, scale);
		ChangeLayersRecursively (model.transform, LayerMask.NameToLayer ("UI"));
		//after transformation to show 3D object normalise back, it's rotated after adding
		Normalise ();

	}
	// on click the index of the chosen object is saved and the catalog closed

	private void OnSelect (int index, ObjectCatalogController.CatalogType type)
	{
		switch (type) {
		case ObjectCatalogController.CatalogType.OBJECT:
			button.onClick.AddListener (() => {
				drawController.SetCurrentObject (index);
				catalog.Close ();
			});
			break;
		case ObjectCatalogController.CatalogType.FRAME:
			button.onClick.AddListener (() => {
				drawController.SetCurrentFrame (index);
				catalog.Close ();
			});
			break;
		case ObjectCatalogController.CatalogType.WALL:
		
			button.onClick.AddListener (() => {
				drawController.SetCurrentWall (index);
				catalog.Close ();
			});

			break;
		case ObjectCatalogController.CatalogType.FLOOR:
			button.onClick.AddListener (() => {
				drawController.SetCurrentFloor (index);
				catalog.Close ();
			});

			break;
		case ObjectCatalogController.CatalogType.CEILING:
			button.onClick.AddListener (() => {
				drawController.SetCurrentCeiling (index);
				catalog.Close ();
			});
				
			break;
		default:
			break;
		}

	}
	//rotation and scale config constants, needed to properly visualise in GUI
	private int[] rotation;
	private float scale;
	private int[] position;
	private int[] objectRotation = {0,180,0};
	private int[] frameRotation = {270,180,0};
	private int[] floorRotation = {90,180,0};
	private int[] ceilingRotation = {90,0,0};
	private int[] wallRotation = {0,180,0};

	private int[] objectPosition = {50,0,-30} ;
	private int[] framePosition = {50,50,-30} ;
	private int[] ceilingPosition = {10,90,-150} ;
	private int[] wallPosition = {83,5,-5} ;
	private int[] floorPosition = {90,90,-5} ;

	private float objectScale = 100;
	private float frameScale = 80;
	private float floorScale = 80;
	private float ceilingScale = 80;
	private float wallScale = 60;
	private void ConfigLayout(ObjectCatalogController.CatalogType type){
		switch (type) {
		case ObjectCatalogController.CatalogType.OBJECT:
			rotation= objectRotation;
			scale=objectScale;
			position=objectPosition;
			break;
		case ObjectCatalogController.CatalogType.FRAME:
			rotation=  frameRotation;
			scale=frameScale;
			position=framePosition;
			break;
		case ObjectCatalogController.CatalogType.WALL:
			rotation=  wallRotation;
			scale=wallScale;
			position=wallPosition;
			break;
		case ObjectCatalogController.CatalogType.FLOOR:
			rotation=  floorRotation;
			scale=floorScale;
			position=floorPosition;
			break;
		case ObjectCatalogController.CatalogType.CEILING:
			rotation=  ceilingRotation;
			scale=ceilingScale;
			position=ceilingPosition;
			break;
		default:
			break;
		}
	}
	private GameObject GetGameObject (int index, ObjectCatalogController.CatalogType type)
	{
		switch (type) {
		case ObjectCatalogController.CatalogType.OBJECT:
			return Instantiate (Catalog.GetObject (index));
		case ObjectCatalogController.CatalogType.FRAME:
			return  Instantiate (Catalog.GetFrame (index));
		case ObjectCatalogController.CatalogType.WALL:
			return  Instantiate (Catalog.GetWall (index));
		case ObjectCatalogController.CatalogType.FLOOR:
			return  Instantiate (Catalog.GetFloor (index));
		case ObjectCatalogController.CatalogType.CEILING:
			return  Instantiate (Catalog.GetCeiling (index));
		default:
			return null;
		}
	}

	void ChangeLayersRecursively (Transform trans, int layer)
	{
		trans.gameObject.layer = layer;
		foreach (Transform child in trans) {
			ChangeLayersRecursively (child, layer);
		}
	}
}

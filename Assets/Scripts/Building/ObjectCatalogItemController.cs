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

	public void init (int i, Catalog.CatalogType type)
	{
		index = i; //important because addlistener would use i by reference instead of by value
		OnSelect (index, type);
		var model = GetGameObject (index, type);
		ConfigLayout (type);



	}
	// on click the index of the chosen object is saved and the catalog closed

	private void OnSelect (int index, Catalog.CatalogType type)
	{
		var model = GetGameObject (index, type);
		switch (type) {
		case Catalog.CatalogType.OBJECT:
			rotation = objectRotation;
			scale = objectScale;
			position = objectPosition;
			button.onClick.AddListener (() => {
				drawController.SetCurrentObject (index);
				catalog.Close ();
			});
			break;
		case Catalog.CatalogType.FRAME:
			rotation = frameRotation;
			scale = frameScale;
			position = framePosition;
			button.onClick.AddListener (() => {
				drawController.SetCurrentFrame (index);
				catalog.Close ();
			});
			break;
		case Catalog.CatalogType.WALL:
			rotation = wallRotation;
			scale = wallScale;
			position = wallPosition;
			button.onClick.AddListener (() => {
				drawController.SetCurrentWall (index);
				catalog.Close ();
			});

			break;
		case Catalog.CatalogType.FLOOR:
			rotation = floorRotation;
			scale = floorScale;
			position = floorPosition;
			button.onClick.AddListener (() => {
				drawController.SetCurrentFloor (index);
				catalog.Close ();
			});

			break;
		case Catalog.CatalogType.CEILING:
			rotation = ceilingRotation;
			scale = ceilingScale;
			position = ceilingPosition;
			button.onClick.AddListener (() => {
				drawController.SetCurrentCeiling (index);
				catalog.Close ();
			});
			break;
		case Catalog.CatalogType.ART:
			button.onClick.AddListener (() => {
				drawController.SetCurrentArt (index);
				catalog.Close ();
			});
			break;

		default:
			break;
		}
		if (type != Catalog.CatalogType.ART) {
			model.transform.SetParent (button.transform, false);
			model.transform.localPosition = new Vector3 (position [0], position [1], position [2]);
			model.transform.localRotation = Quaternion.Euler (new Vector3 (rotation [0], rotation [1], rotation [2]));
			model.transform.localScale = new Vector3 (scale, scale, scale);
			ChangeLayersRecursively (model.transform, LayerMask.NameToLayer ("UI"));
			//after transformation to show 3D object normalise back, it's rotated after adding
			Normalise ();
		} else {
			Art art = Catalog.getArt (index);
			if(art.image!=null){
				button.image.enabled = true;
				button.image.sprite = Sprite.Create (art.image, new Rect (0, 0, art.image.width, art.image.height), Vector2.zero);
			}else{
				Debug.Log("The art doesn't contain a thumbnail image.");
			}

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

	private void ConfigLayout (Catalog.CatalogType type)
	{
		switch (type) {
		case Catalog.CatalogType.OBJECT:
			rotation = objectRotation;
			scale = objectScale;
			position = objectPosition;
			break;
		case Catalog.CatalogType.FRAME:
			rotation = frameRotation;
			scale = frameScale;
			position = framePosition;
			break;
		case Catalog.CatalogType.WALL:
			rotation = wallRotation;
			scale = wallScale;
			position = wallPosition;
			break;
		case Catalog.CatalogType.FLOOR:
			rotation = floorRotation;
			scale = floorScale;
			position = floorPosition;
			break;
		case Catalog.CatalogType.CEILING:
			rotation = ceilingRotation;
			scale = ceilingScale;
			position = ceilingPosition;
			break;
		default:
			break;
		}
	}

	private GameObject GetGameObject (int index, Catalog.CatalogType type)
	{
		switch (type) {
		case Catalog.CatalogType.OBJECT:
			return Instantiate (Catalog.GetObject (index));
		case Catalog.CatalogType.FRAME:
			return  Instantiate (Catalog.GetFrame (index));
		case Catalog.CatalogType.WALL:
			return  Instantiate (Catalog.GetWall (index));
		case Catalog.CatalogType.FLOOR:
			return  Instantiate (Catalog.GetFloor (index));
		case Catalog.CatalogType.CEILING:
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

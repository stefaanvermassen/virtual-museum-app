using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ObjectCatalogItemController : GUIControl
{
	private int index;
	public Button button;
	public BuildMuseumActions actions;
	public ObjectCatalogController catalog;
	public ColorPicker colorPicker;
	public GameObject model;

	/// <summary>
	/// Shows the model.
	/// </summary>
	/// <param name="show">If set to <c>true</c> show.</param>
	public void ShowModel(bool show) {
		if (model != null) {
			model.SetActive(show);
		}
	}
	/// <summary>
	/// Init the specified i and type.
	/// </summary>
	/// <param name="i">The index.</param>
	/// <param name="type">Type.</param>
	public void init (int i, Catalog.CatalogType type)
	{
		index = i; //important because addlistener would use i by reference instead of by value
		ConfigLayout (index, type);




	}
	/// <summary>
	/// Configs the layout.
	/// On click the index of the chosen object is saved and the catalog closed
	/// </summary>
	/// <param name="index">Index.</param>
	/// <param name="type">Type.</param>
	private void ConfigLayout (int index, Catalog.CatalogType type)
	{
		model = GetGameObject (index, type);
		switch (type) {
		case Catalog.CatalogType.OBJECT:
			rotation = objectRotation;
			scale = objectScale;
			position = objectPosition;
			button.onClick.AddListener (() => {
				actions.SetObject(index);
				//catalog.Close ();
			});
			break;
		case Catalog.CatalogType.FRAME:
			rotation = frameRotation;
			scale = frameScale;
			position = framePosition;
			button.onClick.AddListener (() => {
				actions.SetFrame (index);
				//catalog.Close ();
			});
			break;
		case Catalog.CatalogType.WALL:
			rotation = wallRotation;
			scale = wallScale;
			position = wallPosition;
			if(model.GetComponent<Colorable>() != null) {
				model.GetComponent<Colorable>().Color = actions.GetWallColor();
			}
			button.onClick.AddListener (() => {
				actions.SetWall (index);
				if(model.GetComponent<Colorable>() != null) {
					colorPicker.Open(model.GetComponent<Colorable>());
					catalog.ShowModels(false);
					//catalog.Close ();
				}
				//catalog.Close ();
			});

			break;
		case Catalog.CatalogType.FLOOR:
			rotation = floorRotation;
			scale = floorScale;
			position = floorPosition;
			if(model.GetComponent<Colorable>() != null) {
				model.GetComponent<Colorable>().Color = actions.GetFloorColor();
			}
			button.onClick.AddListener (() => {
				actions.SetFloor (index);
				if(model.GetComponent<Colorable>() != null) {
					colorPicker.Open(model.GetComponent<Colorable>());
					catalog.ShowModels(false);
					//catalog.Close ();
				}
				//catalog.Close ();
			});

			break;
		case Catalog.CatalogType.CEILING:
			rotation = ceilingRotation;
			scale = ceilingScale;
			position = ceilingPosition;
			if(model.GetComponent<Colorable>() != null) {
				model.GetComponent<Colorable>().Color = actions.GetCeilingColor();
			}
			button.onClick.AddListener (() => {
				actions.SetCeiling (index);
				if(model.GetComponent<Colorable>() != null) {
					colorPicker.Open(model.GetComponent<Colorable>());
					catalog.ShowModels(false);
					//catalog.Close ();
				}
				//catalog.Close ();
			});
			break;
		case Catalog.CatalogType.ART:
			button.onClick.AddListener (() => {
				actions.SetArt (index);
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
			ObjectDisplayProperties properties = model.GetComponent<ObjectDisplayProperties>();
			if(properties != null) {
				//Vector3 scale = model.transform.localScale;
				Vector3 rot = model.transform.localRotation.eulerAngles;
				Vector3 pos = model.transform.localPosition;
				scale *= properties.scaleFactor;
				model.transform.localPosition = pos + (properties.addPosition * scale);
				model.transform.localRotation = Quaternion.Euler (rot + properties.addRotation);
			}
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
	private int[] objectPosition = {52,10,-30} ;
	private int[] framePosition = {50,50,-30} ;
	private int[] ceilingPosition = {10,90,-150} ;
	private int[] wallPosition = {83,5,-5} ;
	private int[] floorPosition = {90,90,-5} ;
	private float objectScale = 100;
	private float frameScale = 70;
	private float floorScale = 80;
	private float ceilingScale = 80;
	private float wallScale = 60;

	/// <summary>
	/// Gets the game object.
	/// </summary>
	/// <returns>The game object.</returns>
	/// <param name="index">Index.</param>
	/// <param name="type">Type.</param>
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
	/// <summary>
	/// Changes the layers recursively.
	/// needed to be able to display 3D gameObject in 2D UI
	/// </summary>
	/// <param name="trans">Trans.</param>
	/// <param name="layer">Layer.</param>
	void ChangeLayersRecursively (Transform trans, int layer)
	{
		trans.gameObject.layer = layer;
		foreach (Transform child in trans) {
			ChangeLayersRecursively (child, layer);
		}
	}
	/// <summary>
	/// Gets the I.
	/// </summary>
	/// <returns>The I.</returns>
	public int getID() {
		return index;
	}
}

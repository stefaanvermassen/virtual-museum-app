using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour {

	public BuildMuseumActions actions;
	public ObjectCatalogController cont;
	public GUIControl content;
	public Colorable model;

	// Use this for initialization
	void Start () {
	
	}

	void SwitchColors(Button selectedButt, Button[] deselectedButts) {
		if (selectedButt.colors.normalColor.g != Color.green.g) {
			ColorBlock colors = selectedButt.colors;
			colors.normalColor = new Color(Color.green.r, Color.green.g, Color.green.b, 75f/255f);
			colors.highlightedColor = new Color(Color.green.r, Color.green.g, Color.green.b, 150f/255f);
			colors.pressedColor = new Color(Color.green.r, Color.green.g, Color.green.b, 200f/255f);
			selectedButt.colors = colors;
		}
		foreach(Button butt in deselectedButts) {
			if(butt.colors.normalColor.g != Color.black.g) {
				ColorBlock colors = butt.colors;
				colors.normalColor = new Color(Color.black.r, Color.black.g, Color.black.b, 0f/255f);
				colors.highlightedColor = new Color(Color.black.r, Color.black.g, Color.black.b, 75f/255f);
				colors.pressedColor = new Color(Color.black.r, Color.black.g, Color.black.b, 150f/255f);
				butt.colors = colors;
			}
		}
	}

	public void Open(Colorable model) {
		GetComponent<GUIControl>().Open ();
		this.model = model;
	}

	public void Close() {
		model.Color = GetCurrentColor ();
		cont.ShowModels (true);
		GetComponent<GUIControl> ().Close ();
		//cont.Open ();
	}

	public void SetCurrentColor(Color color) {
		if (actions != null && cont != null) {
			switch (cont.catalogType) {
			case "WALL":
				actions.SetWallColor (color);
				break;
			case "FLOOR":
				actions.SetFloorColor (color);
				break;
			case "CEILING":
				actions.SetCeilingColor (color);
				break;
			}
		}
		Close ();
	}

	Color GetCurrentColor() {
		Color selectedColor = Color.white;
		if (actions != null && cont != null) {
			switch (cont.catalogType) {
			case "WALL":
				selectedColor = actions.GetWallColor ();
				break;
			case "FLOOR":
				selectedColor = actions.GetFloorColor ();
				break;
			case "CEILING":
				selectedColor = actions.GetCeilingColor ();
				break;
			default:
				selectedColor = Color.white;
				break;
			}
		}
		return selectedColor;
	}
	
	// Update is called once per frame
	void Update () {
		Color selectedColor = GetCurrentColor();
		Button[] items = content.GetComponentsInChildren<Button>();
		Button selectedItem = null;
		List<Button> unselectedItems = new List<Button>();
		foreach(Button item in items) {
			if(item.GetComponentInChildren<ColorPickerColor>().color.Equals(selectedColor)) {
				selectedItem = item;
			} else {
				unselectedItems.Add (item);
			}
		}
		if(selectedItem != null) {
			SwitchColors (selectedItem, unselectedItems.ToArray());
		}
	}
}

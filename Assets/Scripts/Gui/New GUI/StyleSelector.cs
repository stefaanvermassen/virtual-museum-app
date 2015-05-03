using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StyleSelector : MonoBehaviour {

	public Button wallTab;
	public Button floorTab;
	public Button ceilingTab;
	public Button frameTab;
	ObjectCatalogController cont;
	public BuildMuseumActions actions;

	// Use this for initialization
	void Start () {
		cont = GetComponent<ObjectCatalogController> ();
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
	
	// Update is called once per frame
	void Update () {
		if(cont != null) {
			int selectedID = -1;
			switch(cont.catalogType) {
			case "WALL":
				SwitchColors(wallTab, new[]{floorTab, ceilingTab, frameTab});
				selectedID = actions.GetWall();
				break;
			case "FLOOR":
				SwitchColors(floorTab, new[]{wallTab, ceilingTab, frameTab});
				selectedID = actions.GetFloor();
				break;
			case "CEILING":
				SwitchColors(ceilingTab, new[]{floorTab, wallTab, frameTab});
				selectedID = actions.GetCeiling();
				break;
			case "FRAME":
				SwitchColors(frameTab, new[]{floorTab, ceilingTab, wallTab});
				selectedID = actions.GetFrame();
				break;
			}
			ObjectCatalogItemController[] items = cont.catalogContent.GetComponentsInChildren<ObjectCatalogItemController>();
			Button selectedItem = null;
			List<Button> unselectedItems = new List<Button>();
			foreach(ObjectCatalogItemController item in items) {
				if(item.getID () == selectedID) {
					selectedItem = item.GetComponent<Button>();
				} else {
					unselectedItems.Add (item.GetComponent<Button>());
				}
			}
			if(selectedItem != null) {
				SwitchColors (selectedItem, unselectedItems.ToArray());
			}
		}
	}
}

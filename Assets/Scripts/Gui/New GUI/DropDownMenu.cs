using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DropDownMenu : MonoBehaviour {

	GUIControl control;
	RectTransform rect;

	// Use this for initialization
	void Start () {
		control = GetComponent<GUIControl> ();
		rect = GetComponent<RectTransform> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0) && !MouseOver ()) {
			control.Close();
		}
	}

	public bool MouseOver() {
		var scale = Screen.height / 600f;
		var x1 = Screen.width + ((rect.localPosition.x - rect.sizeDelta.x) * scale);
		var x2 = Screen.width + ((rect.localPosition.x) * scale);
		//print ("Sizedelta Y: " + rect.sizeDelta.y);
		var y1 = ((-rect.localPosition.y) * scale);
		var y2 = ((-rect.localPosition.y) * scale) + ((600f+rect.sizeDelta.y) * scale);
		var x = Input.mousePosition.x;
		var y = Screen.height - Input.mousePosition.y;
		//print ("X: " + x1 + " - " + x2 + ": " + x);
		//print ("Y: " + y1 + " - " + y2 + ": " + y);
		return (x >= x1 && x <= x2 && y >= y1 && y <= y2);
	}
}

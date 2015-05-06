using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ColorPickerColor : MonoBehaviour {
	public Color color;
	public ColorPicker picker;
	public Image image;

	// Use this for initialization
	void Start () {
		if (image == null) {
			image = GetComponent<Image> ();
		}
		if (image != null) {
			image.color = color;
		}
	}

	public void SetColor() {
		picker.SetCurrentColor (color);
	}
}

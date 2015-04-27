using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageHighlightButton : Button {

	public Image buttonImage = null;
	public Sprite normalSprite = null;
	public Sprite highlightSprite;


	// Use this for initialization
	void Start () {
		base.Start ();
		if (buttonImage == null) {
			buttonImage = GetComponentInChildren<Image> ();
		}
		if (buttonImage != null) {
			normalSprite = buttonImage.sprite;
		}

	}

	protected override void DoStateTransition (SelectionState state, bool instant)
	{
		base.DoStateTransition (state, instant);
		if (image != null && highlightSprite != null && normalSprite != null) {
			if (state == SelectionState.Disabled || state == SelectionState.Normal) {
				buttonImage.sprite = normalSprite;
			} else {
				buttonImage.sprite = highlightSprite;
			}
		}
	}
}

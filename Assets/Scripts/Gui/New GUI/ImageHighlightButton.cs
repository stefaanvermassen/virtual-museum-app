using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageHighlightButton : Button {

	public Image buttonImage = null;
	public Sprite normalSprite = null;
	public Sprite highlightSprite;

	public bool forceHighlight = false;


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

	public void Highlight(bool highlight) {
		if (highlight == true && forceHighlight != true) { 
			DoStateTransition (SelectionState.Highlighted, false);
		} else if(highlight == false && forceHighlight != false) {
			DoStateTransition (SelectionState.Normal, false);
		}
		forceHighlight = highlight;
	}

	void Update() {
		if (forceHighlight) {
			DoStateTransition (SelectionState.Highlighted, false);
		}
	}

	public string GetSelectionState() {
		return currentSelectionState.ToString();
	}

	public bool IsHighlighted() {
		return (currentSelectionState == SelectionState.Highlighted || currentSelectionState == SelectionState.Pressed);
	}
}

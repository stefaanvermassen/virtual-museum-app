using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MuseumListItem : MonoBehaviour {
	public string museumName;
	public string museumDescription;
	public int museumID;

	public MuseumList list;
	public GUIControl museumPopUp;
	public Text popUpName;
	public Text popUpDescription;

	public void UpdateLabels() {
		Text[] labels = GetComponentsInChildren<Text> ();
		foreach (Text label in labels) {
			if(label.name.Contains ("Title")) {
				if(museumName.Length < 17) {
					label.text = museumName;
				} else {
					label.text = museumName.Substring(0,14) + "...";
				}
			} else if(label.name.Contains ("Description")) {
				if(museumDescription.Length < 55) {
					label.text = museumDescription;
				} else {
					label.text = museumDescription.Substring(0,52) + "...";
				}
			}
		}
	}

	public void OnClick() {
		list.actions.currentMuseumID = museumID;
		/*MainMenuActions actions = FindObjectOfType<MainMenuActions> ();
		if (actions != null) {
			actions.currentMuseumID = museumID;
		}*/
		museumPopUp.FlipCloseOpen ();
		popUpName.text = museumName;
		popUpDescription.text = museumDescription;
	}
}

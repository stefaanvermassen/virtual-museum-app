using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MuseumListItem : MonoBehaviour {
	public string museumName;
	public string museumDescription;
	public int museumID;

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
				if(museumName.Length < 55) {
					label.text = museumName;
				} else {
					label.text = museumName.Substring(0,52) + "...";
				}
			}
		}
	}

	public void OnClick() {
		MainMenuActions actions = FindObjectOfType<MainMenuActions> ();
		if (actions != null) {
			actions.VisitMuseum (museumID, true);
		}
		// TODO: Open dialog with museum name, description, 2 big buttons: walk, edit, 2 small buttons: back, delete
	}
}

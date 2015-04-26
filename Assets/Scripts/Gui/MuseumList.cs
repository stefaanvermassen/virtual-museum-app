using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MuseumList : MonoBehaviour {

	public Text selectText;

	void Start () {
		InitList();
	}

	void ClearList() {
		MuseumListItem[] listItems = GetComponentsInChildren<MuseumListItem> ();
		foreach (var o in listItems) {
			Destroy(o.gameObject);
		}
	}

	void InitList() {
		ClearList ();
		var listItem = Resources.Load("gui/MuseumListItem");
		var museumController = API.MuseumController.Instance;
		var userController = API.UserController.Instance;
		selectText.text = "Loading museum list...";

		museumController.GetConnectedMuseums((success) => {
			if (success.Count == 0) {
				selectText.text = "You haven't created any museum yet.";
				// TODO: Open create museum popup dialog
			}
			foreach (API.Museum m in success) {
				selectText.text = "Select a museum";
				MuseumListItem item = ((GameObject) GameObject.Instantiate(listItem)).GetComponent<MuseumListItem>();
				item.transform.SetParent (transform, false);
				item.museumName = m.Name;
				item.museumDescription = m.Description;
				item.UpdateLabels();
			}
		});

	}
}

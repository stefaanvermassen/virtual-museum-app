using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoadMuseumBehaviour : MonoBehaviour {

    public Museum museum;

    void RemoveButtons() {
        var content = transform.FindChild("ScrollPanel").FindChild("Content");
        foreach (var b in content.GetComponentsInChildren<Button>()) {
            Destroy(b.gameObject);
        }
        foreach (var t in content.GetComponentsInChildren<Text>()) {
            Destroy(t.gameObject);
        }
    }

    public void InitializeButtons() {
        var buttonMaster = Resources.Load("gui/Button");
        var textMaster = Resources.Load("gui/Label");
        RemoveButtons();
        var content = transform.FindChild("ScrollPanel").FindChild("Content");
        var scrollbar = transform.FindChild("ScrollPanel").FindChild("Scrollbar").gameObject.GetComponent<Scrollbar>();
        var museumController = API.MuseumController.Instance;
        var userController = API.UserController.Instance;
        var panelText = ((GameObject)GameObject.Instantiate(textMaster)).GetComponent<Text>();
        panelText.fontSize = 20;
        panelText.text = "Fetching Museums from server...";
        panelText.transform.SetParent(content, false);
        museumController.GetConnectedMuseums((success) => {
            Destroy(panelText);
            if (success.Count == 0) {
                var emptyText = ((GameObject)GameObject.Instantiate(textMaster)).GetComponent<Text>();
                emptyText.fontSize = 20;
                emptyText.text = "You currently don't have any museums, go make some!";
                emptyText.transform.SetParent(content, false);
            }
            foreach (API.Museum m in success) {
                var button = ((GameObject) GameObject.Instantiate(buttonMaster)).GetComponent<Button>();
                var name = ((GameObject)GameObject.Instantiate(textMaster)).GetComponent<Text>();
                var author = ((GameObject)GameObject.Instantiate(textMaster)).GetComponent<Text>();
                var description = ((GameObject)GameObject.Instantiate(textMaster)).GetComponent<Text>();
                name.text = m.Name;
                name.fontSize = 20;
                author.text = m.OwnerName;
                author.fontSize = 20;
                description.text = m.Description;
                description.fontSize = 20;
                button.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
                Destroy(button.transform.FindChild("Text").gameObject);
                name.transform.SetParent(button.transform, false);
                author.transform.SetParent(button.transform, false);
                description.transform.SetParent(button.transform, false);
                button.transform.SetParent(content, false);
                var closedM = m;
                button.onClick.AddListener(() => {
                    museum.LoadRemote(""+closedM.MuseumID);
                    GetComponent<GUIControl>().Close();
                });
            }
        });
	}
	
}

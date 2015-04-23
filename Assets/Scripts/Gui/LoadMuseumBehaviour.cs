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
    }

    public void InitializeButtons() {
        RemoveButtons();
        var content = transform.FindChild("ScrollPanel").FindChild("Content");
        var controller = API.MuseumController.Instance;
        controller.GetConnectedMuseums((success) => {
            var buttonMaster = Resources.Load("gui/Button");
            var textMaster = Resources.Load("gui/Label");
            foreach (API.Museum m in success) {
                var button = ((GameObject) GameObject.Instantiate(buttonMaster)).GetComponent<Button>();
                var name = ((GameObject)GameObject.Instantiate(textMaster)).GetComponent<Text>();
                var author = ((GameObject)GameObject.Instantiate(textMaster)).GetComponent<Text>();
                var description = ((GameObject)GameObject.Instantiate(textMaster)).GetComponent<Text>();
                name.text = "Here comes the museum name";
                name.fontSize = 20;
                author.text = "Here comes the museum author";
                author.fontSize = 20;
                description.text = m.Description;
                description.fontSize = 20;
                button.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
                Destroy(button.transform.FindChild("Text").gameObject);
                name.transform.SetParent(button.transform, false);
                author.transform.SetParent(button.transform, false);
                description.transform.SetParent(button.transform, false);
                button.transform.SetParent(content, false);
                button.onClick.AddListener(() => {
                    museum.LoadRemote(""+m.MuseumID);
                    GetComponent<GUIControl>().close();
                });
            }
        });
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

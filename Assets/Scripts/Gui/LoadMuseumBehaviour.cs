using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadMuseumBehaviour : StatisticsBehaviour {

    public Museum museum;

    private Object buttonMaster;
    private Object textMaster;
    private Transform content;
    private Scrollbar scrollbar;
    private bool initialized = false;

    void Start() {
        buttonMaster = Resources.Load("gui/Button");
        textMaster = Resources.Load("gui/Label");
        content = transform.FindChild("ScrollPanel").FindChild("Content");
        scrollbar = transform.FindChild("ScrollPanel").FindChild("Scrollbar").gameObject.GetComponent<Scrollbar>();
        initialized = true;
        StartStatistics("LoadMuseum");
    }

    void Initialize() {
        if (!initialized) {
            Start();
        }
    }

    void RemoveButtons() {
        foreach (var b in content.GetComponentsInChildren<Button>()) {
            Destroy(b.gameObject);
        }
        foreach (var t in content.GetComponentsInChildren<Text>()) {
            Destroy(t.gameObject);
        }
    }

    public void ShowMuseums(ArrayList museums) {
        foreach (API.Museum m in museums) {
            var button = ((GameObject)GameObject.Instantiate(buttonMaster)).GetComponent<Button>();
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
                museum.LoadRemote("" + closedM.MuseumID);
                GetComponent<GUIControl>().Close();
                ClosingButton("LoadRemote");
            });
        }
    }

    public void InitializeButtons() {
        Initialize();
        RemoveButtons();
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
            ShowMuseums(success);
        });
	}
	
}

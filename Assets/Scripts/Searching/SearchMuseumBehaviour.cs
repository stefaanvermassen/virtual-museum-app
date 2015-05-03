using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SearchMuseumBehaviour : MonoBehaviour {

    public CanvasRenderer resultPanel;

    Text nameText, ownerText, descriptionText, ratingText;

    private Object buttonMaster;
    private Object textMaster;
    private Transform content;
    private Scrollbar scrollbar;

    void Start() {
        resultPanel.gameObject.SetActive(true);
        var inputs = transform.FindChild("Inputs");
        var fields = inputs.FindChild("Fields");
        var nameField = fields.FindChild("NameField");
        var ownerField = fields.FindChild("OwnerField");
        var descriptionField = fields.FindChild("DescriptionField");
        var ratingField = fields.FindChild("RatingField");
        nameText = nameField.FindChild("Text").gameObject.GetComponent<Text>();
        ownerText = ownerField.FindChild("Text").gameObject.GetComponent<Text>();
        descriptionText = descriptionField.FindChild("Text").gameObject.GetComponent<Text>();
        ratingText = ratingField.FindChild("Text").gameObject.GetComponent<Text>();

        buttonMaster = Resources.Load("gui/Button");
        textMaster = Resources.Load("gui/Label");
        content = resultPanel.transform.FindChild("ScrollPanel").FindChild("Content");
        scrollbar = resultPanel.transform.FindChild("ScrollPanel").FindChild("Scrollbar").gameObject.GetComponent<Scrollbar>();
        resultPanel.gameObject.SetActive(false);
    }

    public void Search() {
        Debug.Log(nameText.text);
        Debug.Log(ownerText.text);
        Debug.Log(descriptionText.text);
        Debug.Log(ratingText.text);
        var controller = API.MuseumController.Instance;
        API.MuseumSearchModel model = new API.MuseumSearchModel();
        model.Name = nameText.text;
        model.Rating = ratingText.text == "" ? 0 : int.Parse(ratingText.text);
        model.Description = descriptionText.text;
        model.OwnerName = ownerText.text;
        resultPanel.gameObject.SetActive(true);
        RemoveButtons();
        var emptyText = CreateText("Fetching results...");
        emptyText.transform.SetParent(content, false);
        controller.GetMuseums(model, (success) => {
            ShowMuseums(success);
        });
    }

	public void BackToMain() {
		Application.LoadLevel ("MainMenuScene");
	}

    void RemoveButtons() {
        foreach (var b in content.GetComponentsInChildren<Button>()) {
            Destroy(b.gameObject);
        }
        foreach (var t in content.GetComponentsInChildren<Text>()) {
            Destroy(t.gameObject);
        }
    }

    private Text CreateText(string text) {
        var t = ((GameObject)GameObject.Instantiate(textMaster)).GetComponent<Text>();
        t.text = text;
        t.fontSize = 20;
        return t;
    }

    private Button CreateButton() {
        var button = ((GameObject)GameObject.Instantiate(buttonMaster)).GetComponent<Button>();
        Destroy(button.transform.FindChild("Text").gameObject);
        button.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
        return button;
    }

    public void ShowMuseums(ArrayList museums) {
        RemoveButtons();
        if (museums.Count == 0) {
            var emptyText = CreateText("No results found");
            emptyText.transform.SetParent(content, false);
        }
        foreach (API.Museum m in museums) {
            var button = CreateButton();
            var name = CreateText(m.Name);
            var author = CreateText(m.OwnerName);
            var description = CreateText(m.Description);
            name.transform.SetParent(button.transform, false);
            author.transform.SetParent(button.transform, false);
            description.transform.SetParent(button.transform, false);
            button.transform.SetParent(content, false);
            var closedM = m;
            button.onClick.AddListener(() => {
                StartMuseum(""+closedM.MuseumID);
            });
        }
    }

    void StartMuseum(string id) {
        /*Catalog.Refresh();
        var museum = new GameObject().AddComponent<Museum>();
        museum.LoadRemote(id);
        AsyncLoader.CreateAsyncLoader(
            isDone: () => {
                return museum.IsLoaded();
            },
            whenDone: () => {
                var data = museum.Save();
                Stream TestFileStream = File.Create(Application.persistentDataPath + "/test.bin");
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(TestFileStream, data);
                TestFileStream.Close();
                Application.LoadLevel("WalkingController");
            });*/
		MuseumLoader.currentAction = MuseumLoader.MuseumAction.Visit;
		MuseumLoader.DeleteTempMuseum ();
		MuseumLoader.museumID = int.Parse (id);
		Application.LoadLevel ("WalkingController");
    }
}

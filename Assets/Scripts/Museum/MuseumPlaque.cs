using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MuseumPlaque : MonoBehaviour {

    public Vector2 size = new Vector2(1,1);
    public float detail = 100;
    public string plaqueText = "Plaque Text";

    private GameObject plaqueObject;
    private GameObject textObject;

    void Start() {
        plaqueObject = new GameObject();
        plaqueObject.transform.SetParent(this.gameObject.transform, false);
        plaqueObject.name = "Plaque";
        plaqueObject.layer = 0;
        var canvas = plaqueObject.AddComponent<Canvas>();
        var rect = plaqueObject.GetComponent<RectTransform>();
        rect.sizeDelta = detail*size;
        rect.localScale = new Vector3(1, 1, 1) / detail;
        rect.localPosition = new Vector3(0, 0, 0);
        canvas.renderMode = RenderMode.WorldSpace;

        textObject = new GameObject();
        textObject.transform.SetParent(plaqueObject.transform, false);
        textObject.name = "PlaqueText";
        var text = textObject.AddComponent<Text>();
        var textRect = textObject.GetComponent<RectTransform>();
        textRect.sizeDelta = detail*size;
        text.font = (Font)Resources.Load("Fonts/Artbrush");
        text.text = plaqueText;
        text.resizeTextForBestFit = true;
        text.color = Color.black;
	}
	
	void Update () {
	
	}

    void OnDestroy() {
        Util.Destroy(plaqueObject);
        Util.Destroy(textObject);
    }
    
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MuseumPlaque : MonoBehaviour {

    public Vector2 size = new Vector2(1,1);
    public float detail = 100;
    public string plaqueText = "Plaque Text";

    private GameObject plaqueObject;
    private GameObject textObject;
    private GameObject cubeObject;

	private static float PLAQUE_THICKNESS = 0.045f;
	private static float FLOATING_MARGIN = 0.0001f;

    void Start() {
        plaqueObject = new GameObject();
        plaqueObject.transform.SetParent(this.gameObject.transform, false);
        plaqueObject.name = "Plaque";
        plaqueObject.layer = 0;
        var canvas = plaqueObject.AddComponent<Canvas>();
        var rect = plaqueObject.GetComponent<RectTransform>();
        rect.sizeDelta = detail*size;
        rect.localScale = new Vector3(1, 1, 1) / detail;
        rect.localPosition = new Vector3(0, 0, PLAQUE_THICKNESS + FLOATING_MARGIN);
        rect.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
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
        text.resizeTextMaxSize = 20;
        text.resizeTextMinSize = 10;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;

        cubeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeObject.transform.localScale = new Vector3(size.x*1.1f, size.y*1.1f, PLAQUE_THICKNESS);
        cubeObject.transform.SetParent(gameObject.transform,false);
	}
	
    void OnDestroy() {
        Util.Destroy(plaqueObject);
        Util.Destroy(textObject);
    }
    
}

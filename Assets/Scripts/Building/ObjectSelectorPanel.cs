using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ObjectSelectorPanel : MonoBehaviour {

    public Sprite buttonImage;
    public GameObject toolPanel;
    public GameObject objectPanel;
    public DrawController drawController;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < Catalog.objects.Length; i++) {
            var ob = new GameObject();
            var image = ob.AddComponent<Image>();
            image.sprite = buttonImage;
            image.type = Image.Type.Sliced;
            var button = ob.AddComponent<Button>();
            button.targetGraphic = image;
            button.transform.SetParent(gameObject.transform, false);
            int index = i; //important because addlistener would use i by reference instead of by value
            button.onClick.AddListener(() => {
                objectPanel.SetActive(false);
                toolPanel.SetActive(true);
                drawController.SetCurrentObject(index);
            });
            var model = Instantiate(Catalog.GetObject(i));
            model.transform.SetParent(button.transform, false);
            model.transform.localPosition = new Vector3(0, -30, -30);
            model.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            model.transform.localScale = new Vector3(100, 100, 100);
            ChangeLayersRecursively(model.transform, LayerMask.NameToLayer("UI"));
        }
	}

    void ChangeLayersRecursively(Transform trans, int layer) {
        trans.gameObject.layer = layer;
        foreach (Transform child in trans) {
            ChangeLayersRecursively(child,layer);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

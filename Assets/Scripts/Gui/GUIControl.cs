using UnityEngine;
using System.Collections;

/// <summary>
/// notes: watch out when using objects from the editor as parameter in events!!!
/// Sometimes the argument will always convert to null
/// </summary>
public class GUIControl : MonoBehaviour {

	//Todo
	////create prefab for: checkbox, radio buttons, 
	/// create input adaptor to be used in input guicontrol
	/// implement scrolling in panel if vertical space is exceeded by children
	public enum types{
		Button, Window, Label,Panel,CatalogItem,Catalog
	}
	//defauld adapter is empty
	void Start () {
	}

	
	public void hide(){
		gameObject.SetActive (false);
	}
	public void show(){
		gameObject.SetActive (true);
	}
	//add gui control to children
	public void add(GUIControl control){
		control.gameObject.transform.SetParent(this.gameObject.transform);
	}
	//return an instantiatec prefab
//	//Transform.SetParent method with the worldPositionStays parameter set to false
//	//the UI Element is a child of a Layout Group it will be automatically positioned and the positioning step can be skipped
	public static GUIControl init(types controlType){
		return (GUIControl) Instantiate((GUIControl)Resources.Load("gui/"+controlType.ToString(), typeof(GUIControl)));
	}

	//show guicontrol on top of all siblings
	public void onTop(){
		this.gameObject.transform.SetSiblingIndex (0);
	}
	public void setRelativePosition(float x, float y){
		RectTransform rectTransform = (RectTransform)transform;
		rectTransform.anchoredPosition=new Vector2(x,y);
	}
	public float getRelativeX(){
		RectTransform rectTransform = (RectTransform)transform;
		return rectTransform.anchoredPosition.x;

	}

	public float getRelativeY(){
		RectTransform rectTransform = (RectTransform)transform;
		return rectTransform.anchoredPosition.y;
	}
	//switch place with GUIControl
	public void replace(GUIControl control){
		float x = control.getRelativeX();
		float y = control.getRelativeY ();
		control.setRelativePosition (getRelativeX (), getRelativeY ());
		setRelativePosition (x, y);
	}
	//Todo
	//basic methods to be added for: color, text, position, behaviour, layout in panel

	public void removeAllChildren(){
		foreach (Transform child in this.gameObject.transform) {
			GameObject.Destroy (child.gameObject);
		}
	}
	//on initialisation sometimes a gameobject is scaled
	public void normalise(){
		transform.localScale = Vector3.one;
	}


}

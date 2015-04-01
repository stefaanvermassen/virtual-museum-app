using UnityEngine;
using System.Collections;

/// <summary>
/// notes: watch out when using objects from the editor as parameter in events!!!
/// Sometimes the argument will always convert to null
/// </summary>
public class GUIControl : MonoBehaviour
{
	public GUIControl dynamicChild;
	//Todo
	//add field for dynamic child, wich will be used to populate a gui control that has a dynamic amount of children eg file browser , catalog,..., 
	public enum types
	{
		Button,
		Window,
		Label,
		Panel,
		CatalogItem,
		Catalog,
		FileBrowser
	}
	void Start () {
		//dynamic child is edited in Unity editor but not shown
		if (dynamicChild != null) {
			//detach dynmic child, because else it will be removed if children are removed
			transform.DetachChildren();
			dynamicChild.close ();
		}
	}

	//both show and hide can be overriden to add extra close and open logic
	
	public virtual void close ()
	{
		gameObject.SetActive (false);
	}

	public virtual void open ()
	{
		gameObject.SetActive (true);
	}
	public void flipCloseOpen(){
		if (isOpen ()) {
			close ();
		} else {
			open ();
		}
	}
	//add gui control to children
	public void add (GUIControl control)
	{
		control.gameObject.transform.SetParent (this.gameObject.transform,false);
		//when an transform is added to it's parent it is scaled, for the sake of layout, performance and graphics all GUIControls are scale 1
		control.normalise ();
	}
	public GUIControl addDynamicChild(){
		if (dynamicChild != null) {
			GUIControl newDynamicChild = GUIControl.init (dynamicChild);

			newDynamicChild.open();
			this.add (newDynamicChild);

			return newDynamicChild;
		}
		return null;
	}
	//return an instantiatec prefab
//	//Transform.SetParent method with the worldPositionStays parameter set to false
//	//the UI Element is a child of a Layout Group it will be automatically positioned and the positioning step can be skipped
	public static GUIControl init (types controlType)
	{
		return (GUIControl)Instantiate ((GUIControl)Resources.Load ("gui/" + controlType.ToString (), typeof(GUIControl)));
	}

	public static GUIControl init (GUIControl control)
	{
		GUIControl instance = Instantiate (control);
		activateAllChildScripts (instance.transform);
		return instance;
	}

	private static void activateAllChildScripts (Transform instance)
	{
		//activate all scripts
		foreach (MonoBehaviour script in instance.GetComponents<MonoBehaviour>()) {
			script.enabled = true;
		}
		//activate all children
		for (int i=0; i<instance.childCount; i++) {
			activateAllChildScripts(instance.GetChild (i));
		}
	}
	//show guicontrol on top of all siblings
	public void onTop ()
	{
		this.gameObject.transform.SetSiblingIndex (0);//qsmdjkfsjqdfi
	}

	public void setRelativePosition (float x, float y)
	{
		RectTransform rectTransform = (RectTransform)transform;
		rectTransform.anchoredPosition = new Vector2 (x, y);
	}

	public float getRelativeX ()
	{
		RectTransform rectTransform = (RectTransform)transform;
		return rectTransform.anchoredPosition.x;

	}

	public float getRelativeY ()
	{
		RectTransform rectTransform = (RectTransform)transform;
		return rectTransform.anchoredPosition.y;
	}
	//switch place with GUIControl
	public virtual void replace (GUIControl control)
	{
		float x = control.getRelativeX ();
		float y = control.getRelativeY ();
		control.setRelativePosition (getRelativeX (), getRelativeY ());
		setRelativePosition (x, y);
	}
	//Todo
	//basic methods to be added for: color, text, position, behaviour, layout in panel

	public void removeAllChildren ()
	{
		foreach (Transform child in this.transform) {
			GameObject.Destroy (child.gameObject);
		}
	}

	public GUIControl getChild (int index)
	{
		return this.transform.GetChild (index).GetComponent<GUIControl> ();
	}
	//on initialisation sometimes a gameobject is scaled
	//or even rotated if working with multiple camera's
	public void normalise ()
	{
		transform.localScale = Vector3.one;
		transform.localRotation = Quaternion.Euler(Vector3.zero);


	}

	public bool isOpen ()
	{
		return this.gameObject.activeSelf;
	}


}

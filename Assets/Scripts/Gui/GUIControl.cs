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

	/// <summary>
	/// Close this instance.
	/// both show and hide can be overriden to add extra close and open logic
	/// </summary>
	public virtual void close ()
	{
		gameObject.SetActive (false);
	}

	public virtual void open ()
	{
		gameObject.SetActive (true);
	}
	/// <summary>
	/// Flips the close open.
	/// </summary>
	public void flipCloseOpen(){
		if (isOpen ()) {
			close ();
		} else {
			open ();
		}
	}
	/// <summary>
	/// Add the specified control.
	/// </summary>
	/// <param name="control">Control.</param>
	public void add (GUIControl control)
	{
		control.gameObject.transform.SetParent (this.gameObject.transform,false);
		//when an transform is added to it's parent it is scaled, for the sake of layout, performance and graphics all GUIControls are scale 1
		control.normalise ();
	}
	/// <summary>
	/// Adds the dynamic child.
	/// </summary>
	/// <returns>The dynamic child.</returns>
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
	//Transform.SetParent method with the worldPositionStays parameter set to false
	//the UI Element is a child of a Layout Group it will be automatically positioned and the positioning step can be skipped
	/// <summary>
	/// Init the specified controlType.
	/// </summary>
	/// <param name="controlType">Control type.</param>
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
		RectTransform rectTransform = GetComponent<RectTransform> ();
		if (rectTransform == null) {
			rectTransform=this.gameObject.AddComponent<RectTransform>();
		}
		rectTransform.anchoredPosition = new Vector2 (x, y);
	}

	public float getRelativeX ()
	{
		RectTransform rectTransform = GetComponent<RectTransform> ();
		if (rectTransform == null) {
			rectTransform=this.gameObject.AddComponent<RectTransform>();
		}
		return rectTransform.anchoredPosition.x;

	}

	public float getRelativeY ()
	{
		RectTransform rectTransform = GetComponent<RectTransform> ();
		if (rectTransform == null) {
			rectTransform=this.gameObject.AddComponent<RectTransform>();
		}
		return rectTransform.anchoredPosition.y;
	}
	//switch place with GUIControl, TODO use absolute coordinates
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
		int children = this.transform.childCount;
		for (int i = 0; i < children; i++) {
			GameObject.DestroyImmediate(this.transform.GetChild (0).gameObject);
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

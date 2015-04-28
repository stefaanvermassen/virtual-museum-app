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

	void Start ()
	{
		SaveDynamicChild ();
	}

	public void SaveDynamicChild ()
	{
		//dynamic child is edited in Unity editor but not shown
		if (dynamicChild != null) {
			if (GetChild (0)!=null && GetChild (0).Equals (dynamicChild)) {
				//detach dynmic child, because else it will be removed if children are removed
				transform.DetachChildren ();
				dynamicChild.Close ();
			}
		
		}
	}

	/// <summary>
	/// Close this instance.
	/// both show and hide can be overriden to add extra close and open logic
	/// </summary>
	public virtual void Close ()
	{
		gameObject.SetActive (false);
	}

	public virtual void Open ()
	{
		gameObject.SetActive (true);
	}
	/// <summary>
	/// Flips the close open.
	/// </summary>
	public void FlipCloseOpen ()
	{
		if (IsOpen ()) {
			Close ();
		} else {
			Open ();
		}
	}
	//add gui control to children
	public void Add (GUIControl control)
	{
		control.gameObject.transform.SetParent (this.gameObject.transform, false);
		//when an transform is added to it's parent it is scaled, for the sake of layout, performance and graphics all GUIControls are scale 1
		control.Normalise ();
	}

	public GUIControl AddDynamicChild ()
	{
		if (dynamicChild != null) {
			GUIControl newDynamicChild = GUIControl.Init (dynamicChild);
			newDynamicChild.Open ();
			this.Add (newDynamicChild);
			return newDynamicChild;
		}
		return null;
	}

	//return an instantiatec prefab
	//Transform.SetParent method with the worldPositionStays parameter set to false
	//the UI Element is a child of a Layout Group it will be automatically positioned and the positioning step can be skipped
	public static GUIControl Init (types controlType)
	{
		return (GUIControl)Instantiate ((GUIControl)Resources.Load ("gui/" + controlType.ToString (), typeof(GUIControl)));
	}

	public static GUIControl Init (GUIControl control)
	{
		GUIControl instance = Instantiate (control);
		ActivateAllChildScripts (instance.transform);
		return instance;
	}

	private static void ActivateAllChildScripts (Transform instance)
	{
		//activate all scripts
		foreach (MonoBehaviour script in instance.GetComponents<MonoBehaviour>()) {
			if (script != null)
				script.enabled = true;
		}
		//activate all children
		for (int i=0; i<instance.childCount; i++) {
			ActivateAllChildScripts (instance.GetChild (i));
		}
	}

	//show guicontrol on top of all siblings
	public void OnTop ()
	{
		this.gameObject.transform.SetSiblingIndex (this.gameObject.transform.parent.childCount - 1);
	}

	public void SetRelativePosition (float x, float y)
	{
		RectTransform rectTransform = GetComponent<RectTransform> ();
		if (rectTransform == null) {
			rectTransform = this.gameObject.AddComponent<RectTransform> ();
		}
		rectTransform.anchoredPosition = new Vector2 (x, y);
	}

	public float GetRelativeX ()
	{
		RectTransform rectTransform = GetComponent<RectTransform> ();
		if (rectTransform == null) {
			rectTransform = this.gameObject.AddComponent<RectTransform> ();
		}
		return rectTransform.anchoredPosition.x;
	}

	public float GetRelativeY ()
	{
		RectTransform rectTransform = GetComponent<RectTransform> ();
		if (rectTransform == null) {
			rectTransform = this.gameObject.AddComponent<RectTransform> ();
		}
		return rectTransform.anchoredPosition.y;
	}

	//switch place with GUIControl
	public virtual void Replace (GUIControl control)
	{
		float x = control.GetRelativeX ();
		float y = control.GetRelativeY ();
		control.SetRelativePosition (GetRelativeX (), GetRelativeY ());
		SetRelativePosition (x, y);
		control.gameObject.SetActive (true);
		gameObject.SetActive (false);
	}

	public void RemoveAllChildren ()
	{
		//sometimes a GUIControl is not yet opened, thus the dynamic child has to be loaded before clearing children
		SaveDynamicChild ();
		int children = this.transform.childCount;
		for (int i = 0; i < children; i++) {
			GameObject.DestroyImmediate (this.transform.GetChild (0).gameObject);
		}
	}

	public GUIControl GetChild (int index)
	{
		if(transform.childCount>index){
			return this.transform.GetChild (index).GetComponent<GUIControl> ();
		}
		return null;
	}

	//on initialisation sometimes a gameobject is scaled
	//or even rotated if working with multiple camera's
	public void Normalise ()
	{
		transform.localScale = Vector3.one;
		transform.localRotation = Quaternion.Euler (Vector3.zero);
	}

	public bool IsOpen ()
	{
		return this.gameObject.activeSelf;
	}

}

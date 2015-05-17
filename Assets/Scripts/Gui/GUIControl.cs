using UnityEngine;
using System.Collections;

/// <summary>
/// notes: watch out when using objects from the editor as parameter in events!!!
/// Sometimes the argument will always convert to null
/// </summary>
public class GUIControl : StatisticsBehaviour
{
	public GUIControl dynamicChild;

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
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		SaveDynamicChild ();
	}

	/// <summary>
	/// Saves the dynamic child.
	/// Necessary to keep pointer to dynamicChild after deavtivation of the object
	/// </summary>
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
        End();
	}

	/// <summary>
	/// Open this instance.
	/// </summary>
	public virtual void Open ()
	{
        StartStatistics();
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

	/// <summary>
	/// Add the specified control.
	/// </summary>
	/// <param name="control">Control.</param>
	public void Add (GUIControl control)
	{
		control.gameObject.transform.SetParent (this.gameObject.transform, false);
		//when an transform is added to it's parent it is scaled, for the sake of layout, performance and graphics all GUIControls are scale 1
		control.Normalise ();
	}

	/// <summary>
	/// Adds the dynamic child.
	/// </summary>
	/// <returns>The dynamic child.</returns>
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

	/// <summary>
	/// Init the specified controlType.
	/// the UI Element is a child of a Layout Group it will be automatically positioned and the positioning step can be skipped
	/// </summary>
	/// <param name="controlType">Control type.</param>
	public static GUIControl Init (types controlType)
	{
		return (GUIControl)Instantiate ((GUIControl)Resources.Load ("gui/" + controlType.ToString (), typeof(GUIControl)));
	}

	/// <summary>
	/// Init the specified control.
	/// </summary>
	/// <param name="control">Control.</param>
	public static GUIControl Init (GUIControl control)
	{
		GUIControl instance = Instantiate (control);
		ActivateAllChildScripts (instance.transform);
		return instance;
	}

	/// <summary>
	/// Activates all child scripts.
	/// </summary>
	/// <param name="instance">Instance.</param>
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

	/// <summary>
	/// show guicontrol on top of all siblings
	/// </summary>
	public void OnTop ()
	{
		this.gameObject.transform.SetSiblingIndex (this.gameObject.transform.parent.childCount - 1);
	}

	/// <summary>
	/// Sets the relative position.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void SetRelativePosition (float x, float y)
	{
		RectTransform rectTransform = GetComponent<RectTransform> ();
		if (rectTransform == null) {
			rectTransform = this.gameObject.AddComponent<RectTransform> ();
		}
		rectTransform.anchoredPosition = new Vector2 (x, y);
	}


	/// <summary>
	/// Gets the relative x.
	/// </summary>
	/// <returns>The relative x.</returns>
	public float GetRelativeX ()
	{
		RectTransform rectTransform = GetComponent<RectTransform> ();
		if (rectTransform == null) {
			rectTransform = this.gameObject.AddComponent<RectTransform> ();
		}
		return rectTransform.anchoredPosition.x;
	}

	/// <summary>
	/// Gets the relative y.
	/// </summary>
	/// <returns>The relative y.</returns>
	public float GetRelativeY ()
	{
		RectTransform rectTransform = GetComponent<RectTransform> ();
		if (rectTransform == null) {
			rectTransform = this.gameObject.AddComponent<RectTransform> ();
		}
		return rectTransform.anchoredPosition.y;
	}

	/// <summary>
	/// Replace the specified control.
	/// switch place with GUIControl
	/// </summary>
	/// <param name="control">Control.</param>
	public virtual void Replace (GUIControl control)
	{
		float x = control.GetRelativeX ();
		float y = control.GetRelativeY ();
		control.SetRelativePosition (GetRelativeX (), GetRelativeY ());
		SetRelativePosition (x, y);
        control.Open();
        Close();
	}

	/// <summary>
	/// Removes all children.
	/// </summary>
	public void RemoveAllChildren ()
	{
		//sometimes a GUIControl is not yet opened, thus the dynamic child has to be loaded before clearing children
		SaveDynamicChild ();
		int children = this.transform.childCount;
		for (int i = 0; i < children; i++) {
			GameObject.DestroyImmediate (this.transform.GetChild (0).gameObject);
		}
	}

	/// <summary>
	/// Gets the child.
	/// </summary>
	/// <returns>The child.</returns>
	/// <param name="index">Index.</param>
	public GUIControl GetChild (int index)
	{
		if(transform.childCount>index){
			return this.transform.GetChild (index).GetComponent<GUIControl> ();
		}
		return null;
	}

	//on initialisation sometimes a gameobject is scaled
	//or even rotated if working with multiple camera's
	/// <summary>
	/// Normalise this instance.
	/// </summary>
	public void Normalise ()
	{
		transform.localScale = Vector3.one;
		transform.localRotation = Quaternion.Euler (Vector3.zero);
	}

	/// <summary>
	/// Determines whether this instance is open.
	/// </summary>
	/// <returns><c>true</c> if this instance is open; otherwise, <c>false</c>.</returns>
	public bool IsOpen ()
	{
		return this.gameObject.activeSelf;
	}
}

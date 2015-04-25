using UnityEngine;
using System.Collections;

/// <summary>
/// Provides various useful functions for coding in Unity 
/// </summary>
public class Util {

	/// <summary>
	/// Chooses best destroy function for edit (tests) and play mode
	/// </summary>
	public static void Destroy(Object obj) {
        if (obj == null) {
            return;
        }
		if(Application.isPlaying) {
			Object.Destroy(obj);
		} else Object.DestroyImmediate(obj);
	}

    /// <summary>
    /// Sets an objects layer, and all it's children's layers to a specific layer.
    /// Very useful as Unity does not provide this out of the box.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layer"></param>
    public static void SetLayerRecursively(GameObject obj, int layer) {
        obj.layer = layer;
        foreach (Transform child in obj.transform) {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}

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
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(ImageHighlightButton))]
class ImageHighlightButtonEditor : ButtonEditor {
	public override void OnInspectorGUI() {
		ImageHighlightButton button = (ImageHighlightButton)target;
		button.buttonImage = (Image)EditorGUILayout.ObjectField ("Highlightable Image", button.buttonImage, typeof(Image), true);
		button.normalSprite = (Sprite)EditorGUILayout.ObjectField ("Normal Sprite", button.normalSprite, typeof(Sprite), true);
		button.highlightSprite = (Sprite)EditorGUILayout.ObjectField ("Highlight Sprite", button.highlightSprite, typeof(Sprite), true);
		base.OnInspectorGUI ();
	}
}
using UnityEngine;
using System.Collections;

public class Colorable : MonoBehaviour {

	public MeshRenderer[] renderers;
	Material[] localMaterials = null;
	private Color color = Color.white;
	public Color Color {
		get { return color; }
		set {
			if(value != null) {
				SetColor(value);
				color = value;
			}
		}
	}

	void CopyMaterials() {
		if (localMaterials == null) {
			localMaterials = new Material[renderers.Length];
			for(int i = 0; i < renderers.Length; i++) {
				localMaterials[i] = new Material(renderers[i].material);
				renderers[i].material = localMaterials[i];
			}
		}
	}

	void SetColor(Color color) {
		if (localMaterials == null) {
			CopyMaterials();
		}
		foreach (Material material in localMaterials) {
			//material.SetColor ("_Color",color);
			material.color = color;
			material.shader = material.shader;
			//print (material.GetFloat("_BumpScale"));
			//material.SetTexture ("_BumpMap", material.GetTexture("_BumpMap"));

		}
	}
}

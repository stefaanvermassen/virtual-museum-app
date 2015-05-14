using UnityEngine;
using System.Collections;

public class Selectable : MonoBehaviour {

	public MeshRenderer mesh = null;
	Material[] originalMaterials;
	public Material selectionMaterial;
    public Material lineMaterial;
	Material localSelectionMaterial = null;
    public bool rotationVector = true;
	public bool pulse = true;
	private Color color = new Color(255/255.0F,146/255.0F,8/255.0F);
	public Color OutlineColor {
		get { return color; }
		set {
			if(value != null && localSelectionMaterial != null) {
				localSelectionMaterial.SetColor ("_OutlineColor", value);
			}
			if(value != null) color = value;
		}
	}
	bool pulseUp = true;
    LineRenderer line;

	public enum SelectionMode{ None, Selected, Preview };
	SelectionMode selected = SelectionMode.None;
	public SelectionMode Selected {
		get { return selected; }
		set {
			UpdateSelectionMode (value);
			selected = value;
		}
	}

	public enum LineMode{
		Rotate,
		Scale
	}
	public LineMode lineMode = LineMode.Rotate;

	// Use this for initialization
	void Awake () {
		if(mesh == null) mesh = GetComponent<MeshRenderer>();
		if(mesh == null) mesh = transform.GetComponentInChildren<MeshRenderer>();
		originalMaterials = mesh.materials;
        line = gameObject.AddComponent<LineRenderer>();
        line.SetVertexCount(2);
        line.useWorldSpace = true;
        line.SetWidth(0.2f,0);
        line.material = lineMaterial;
        line.material.color = OutlineColor;
        line.enabled = false;
	}
	
	// Update is called once per frame
    void Update() {
		var forward = transform.forward;
		var center = transform.position + new Vector3 (0, 0.5f, 0);
		if (lineMode == LineMode.Scale) {
			forward = -transform.up;
			center = transform.position;
		}
        line.material.color = OutlineColor;
        line.SetPosition(0, center);
        line.SetPosition(1, center + forward * 1.5f);
		if (pulse && selected != SelectionMode.None && localSelectionMaterial != null) {
			float outlineWidth = localSelectionMaterial.GetFloat ("_Outline");
			if(outlineWidth >= 0.07) {
				pulseUp = false;
			} else if(outlineWidth <= 0.015) {
				pulseUp = true;
			}
			localSelectionMaterial.SetFloat("_Outline",outlineWidth + (pulseUp? 0.001f : -0.001f));
		}
	}

	void UpdateSelectionMode(SelectionMode mode) {
        if (mode == selected) {
            return;
        }
		if (mode == SelectionMode.None) {
            line.enabled = false;
			mesh.materials = originalMaterials;
		} else if (mode == SelectionMode.Selected) {
            if (rotationVector) {
                line.enabled = true;
            }
			Material[] newMaterials = new Material[originalMaterials.Length+1];
			for(int i = 0; i < originalMaterials.Length; i++) {
				newMaterials[i] = originalMaterials[i];
			}
			InitMaterial ();
			newMaterials[originalMaterials.Length] = localSelectionMaterial;
			mesh.materials = newMaterials;
		} else if(mode == SelectionMode.Preview){
            if (rotationVector) {
                line.enabled = true;
            }
			InitMaterial ();
			mesh.material = localSelectionMaterial;
		}
	}

	void InitMaterial() {
		if (localSelectionMaterial == null) {
			localSelectionMaterial = new Material (selectionMaterial);
			if(pulse) localSelectionMaterial.SetFloat("_Outline",0.015f);
			OutlineColor = color;
		}
	}
}
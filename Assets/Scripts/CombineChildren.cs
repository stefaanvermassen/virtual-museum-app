using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildren : MonoBehaviour {
	
	void Start()
	{
		Matrix4x4 myTransform = transform.worldToLocalMatrix;
		Dictionary<Material, List<CombineInstance>> combines = new Dictionary<Material, List<CombineInstance>>();
		MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
		foreach (var meshRenderer in meshRenderers)
		{
			foreach (var material in meshRenderer.sharedMaterials)
				if (material != null && !combines.ContainsKey(material))
					combines.Add(material, new List<CombineInstance>());
		}
		
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		foreach(var filter in meshFilters)
		{
			if (filter.sharedMesh == null)
				continue;
			CombineInstance ci = new CombineInstance();
			ci.mesh = filter.sharedMesh;
			ci.transform = myTransform * filter.transform.localToWorldMatrix;
			combines[filter.GetComponent<Renderer>().sharedMaterial].Add(ci);
			filter.GetComponent<Renderer>().enabled = false;
		}
		
		foreach(Material m in combines.Keys)
		{
			var go = new GameObject("Combined mesh");
			go.transform.parent = transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			
			var filter = go.AddComponent<MeshFilter>();
			filter.mesh.CombineMeshes(combines[m].ToArray(), true, true);
			
			var renderer = go.AddComponent<MeshRenderer>();
			renderer.material = m;
		}
	}
}
using UnityEngine;
using System.Collections;

/// <summary>
/// Frame resize script. Resizes frame to fit art, but does not resize frame edges.
/// X and Z scale have to be 1!
/// </summary>
public class Frame : MonoBehaviour {

	public float innerWidth = 0.6f;
	public float innerHeight = 0.4f;

	public float artWidth;
	public float artHeight;

	public Texture2D texture;

	private Vector3[] oldVertices;

	/// <summary>
	/// Recalculates frame dimensions based on art dimensions.
	/// </summary>
	void Start () {
		Restart ();
	}

	public void Restart(){
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		if (oldVertices != null) {
			mesh.vertices = oldVertices;
			mesh.RecalculateBounds ();
		}
		Vector3[] vertices = mesh.vertices;
		oldVertices = mesh.vertices;
		float widthDiff = artWidth - innerWidth;
		float heightDiff = artHeight - innerHeight;
		
		for (int i = 0; i < vertices.Length; i++) {
			if(Mathf.Abs(vertices[i].x) < innerWidth/2f) print ("Recommended inner width: " + Mathf.Abs (vertices[i].x*2f));
			if(Mathf.Abs(vertices[i].z) < innerHeight/2f) print ("Recommended inner height: " + Mathf.Abs(vertices[i].z*2f));
			if(vertices[i].x < -innerWidth/2f) {
				vertices[i].x -= widthDiff/2f;
			} else if(vertices[i].x > innerWidth/2f) {
				vertices[i].x += widthDiff/2f;
			} else {
				vertices[i].x *= 1/innerWidth * artWidth;
			}
			if(vertices[i].z < -innerHeight/2f) {
				vertices[i].z -= heightDiff/2f;
			} else if(vertices[i].z > innerHeight/2f) {
				vertices[i].z += heightDiff/2f;
			} else {
				vertices[i].z *= 1/innerHeight * artHeight;
			}
		}
		
		mesh.vertices = vertices;
		mesh.RecalculateBounds();
		
		if(texture != null) {
			Material material = GetComponent<MeshRenderer>().materials[0];
			material.SetTexture ("_DetailAlbedoMap", texture);
		}
	}
}

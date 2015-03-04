using UnityEngine;
using System.Collections;

public class MuseumArt : MonoBehaviour {

    public int x, y, z;
    public int orientation;

    public Texture2D texture;
    public Material material;

    private GameObject ob;

	// Use this for initialization
	void Start () {
        ob = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var renderer = ob.GetComponent<MeshRenderer>();
        renderer.material = material;
        renderer.material.mainTexture = texture;

        ob.transform.position = new Vector3(x, y+0.5f, z);
        ob.transform.localScale = new Vector3(0.5f, 0.5f * texture.height / texture.width, 0.05f);
        ob.transform.Rotate(new Vector3(0,90*orientation,0));
        ob.transform.Translate(new Vector3(0, 0, -0.5f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Remove() {
        Destroy(ob);
    }
}

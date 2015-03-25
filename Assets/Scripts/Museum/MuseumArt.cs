using UnityEngine;
using System.Collections;

/// <summary>
/// Contains all information of a art piece, already inside a museum. Generally this should only be used inside Museum.
/// </summary>
public class MuseumArt : MonoBehaviour, Storable<MuseumArt, MuseumArtData> {

    public int x, y, z;
    public int orientation;
    public int artID;

    public Texture2D texture;
    public Material material;

    private GameObject ob;

    public MuseumArtData Save(){
        return new MuseumArtData(artID, x, y, z, orientation);
    }

    public void Load(MuseumArtData data) {
        artID = data.ArtID;
        x = data.X;
        y = data.Y;
        z = data.Z;
        orientation = data.Orientation;
        Start();
    }

	void Start () {
        Remove();
        ob = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var renderer = ob.GetComponent<MeshRenderer>();
        renderer.material = material;
        renderer.material.mainTexture = texture;

        ob.transform.position = new Vector3(x, y+0.5f, z);
        ob.transform.localScale = new Vector3(0.5f, 0.5f * texture.height / texture.width, 0.05f);
        ob.transform.Rotate(new Vector3(0,90*orientation,0));
        ob.transform.Translate(new Vector3(0, 0, -0.5f));
	}

    /// <summary>
    /// Should be called before destroying this GameObject.
    /// </summary>
    public void Remove() {
		Util.Destroy(ob);
    }
}

using UnityEngine;
using System.Collections;

/// <summary>
/// Contains all information of a art piece, already inside a museum. Generally this should only be used inside Museum.
/// </summary>
public class MuseumArt : MonoBehaviour, Storable<MuseumArt, MuseumArtData> {

    public Vector3 position;
    public Vector3 rotation;
    public float scale = 1;
    public int tileX, tileY, tileZ;

    public Art art;

    public Texture2D texture;
    public Material material;

    private GameObject ob;

    public MuseumArtData Save(){
        var artData = art.Save();
        return new MuseumArtData(artData, position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, scale);
    }

    public void Load(MuseumArtData data) {
        art.Load(data.Art);
        position = new Vector3(data.X, data.Y, data.Z);
        rotation = new Vector3(data.RX, data.RY, data.RZ);
        scale = data.Scale;
        //texture = new Texture2D(1, 1);
        //texture.LoadImage(art.image);
        Start();
    }

	void Start () {
        Remove();
        ob = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var renderer = ob.GetComponent<MeshRenderer>();
        renderer.material = material;
        renderer.material.mainTexture = texture;

        ob.transform.position = position;
        ob.transform.localScale = new Vector3(0.5f * scale, scale * 0.5f * texture.height / texture.width, 0.05f);
        ob.transform.Rotate(rotation);
        var normal = Quaternion.Euler(rotation) * Vector3.forward;
        tileX = (int)Mathf.Floor(position.x + normal.x / 2 + 0.5f);
        tileY = 0;
        tileZ = (int)Mathf.Floor(position.z + normal.z / 2 + 0.5f);
	}

    /// <summary>
    /// Should be called before destroying this GameObject.
    /// </summary>
    public void Remove() {
		Util.Destroy(ob);
    }
}

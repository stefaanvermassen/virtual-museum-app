using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
	public int frameStyle;

    private GameObject ob;
    private MuseumPlaque plaque;

    public MuseumArtData Save(){
        var artData = art.Save();
        return new MuseumArtData(artData, position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, scale, frameStyle);
    }

    public void Load(MuseumArtData data) {
        art.Load(data.Art);
        position = new Vector3(data.X, data.Y, data.Z);
        rotation = new Vector3(data.RX, data.RY, data.RZ);
        scale = data.Scale;
		frameStyle = data.FrameStyle;
        Start();
    }

    public void Reload() {
        Start();
    }

	void Start () {
        Remove();
        if (art.image != null) {
            texture = art.image;
        }
		ob = GameObject.Instantiate (Catalog.GetFrame (frameStyle));
		var frame = ob.GetComponent<Frame> ();
		frame.artWidth = 0.5f * scale;
		frame.artHeight = scale * 0.5f * texture.height / texture.width;
		frame.texture = texture;
		ob.transform.position = position;
		ob.transform.localEulerAngles = new Vector3 (rotation.x + 90, rotation.y + 180, rotation.z);
		//ob.transform.Rotate(rotation);

        //ob = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //var renderer = ob.GetComponent<MeshRenderer>();
		//renderer.material = material;
        //renderer.material.mainTexture = texture;

        //ob.transform.position = position;
        //ob.transform.localScale = new Vector3(0.5f * scale, scale * 0.5f * texture.height / texture.width, 0.05f);
        //ob.transform.Rotate(rotation);

        var normal = Quaternion.Euler(rotation) * Vector3.forward;
        tileX = (int)Mathf.Floor(position.x + normal.x / 2 + 0.5f);
        tileY = 0;
        tileZ = (int)Mathf.Floor(position.z + normal.z / 2 + 0.5f);
        plaque = new GameObject().AddComponent<MuseumPlaque>();
        plaque.size = new Vector2(2, 1);
        plaque.transform.localScale = new Vector3(0.2f, 0.2f, 2f);
        plaque.plaqueText = art.description;
        plaque.transform.localPosition = position - new Vector3(0, 0.2f + scale * 0.25f * texture.height / texture.width, 0);
        plaque.transform.Rotate(rotation);
	}

    /// <summary>
    /// Should be called before destroying this GameObject.
    /// </summary>
    public void Remove() {
        if (plaque != null) {
            Util.Destroy(plaque.gameObject);
        }
		Util.Destroy(ob);
    }
}

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

	static float PAINTING_MARGIN = 0.05f;
	static float BOTTOM_PAINTING_MIN = PAINTING_MARGIN;
	static float TOP_PAINTING_MAX = Museum.UNIT_HEIGHT - PAINTING_MARGIN;
	static float PLAQUE_MARGIN = 0.2f;
	static float PLAQUE_HEIGHT = 0.2f;
	static float FRAME_BORDER = 0.1f;
	static float MAX_PAINTING_HEIGHT = Museum.UNIT_HEIGHT - 2 * PAINTING_MARGIN - FRAME_BORDER - PLAQUE_HEIGHT - PLAQUE_MARGIN;

	void Start () {
        Remove();
		ob = GameObject.Instantiate (Catalog.GetFrame (frameStyle));
        plaque = new GameObject().AddComponent<MuseumPlaque>();
		Restart ();
	}

	public void Restart(){
		if (art!=null && art.image != null) {
			texture = art.image;
		}
		var frame = ob.GetComponent<Frame> ();
		var ratio = texture.height / (float)texture.width;
		var maxScale = MAX_PAINTING_HEIGHT / ratio;
		var realScale = scale;
		if (scale > maxScale) {
			realScale = maxScale;
		}else if(scale < 0.5f){
			realScale = 0.5f;
		}
		frame.artWidth = realScale;
		frame.artHeight = realScale * ratio;
		frame.texture = texture;
		frame.Restart ();
		var bottom = position.y - frame.artHeight/2 - PLAQUE_HEIGHT - PLAQUE_MARGIN;
		var top = position.y + frame.artHeight/2 + FRAME_BORDER;
		var realPosition = position;
		if (bottom < BOTTOM_PAINTING_MIN) {
			realPosition = position - new Vector3 (0, bottom - BOTTOM_PAINTING_MIN, 0);
		} else if (top > TOP_PAINTING_MAX) {
			realPosition = position - new Vector3(0,top-TOP_PAINTING_MAX,0); 
		}
		ob.transform.position = realPosition;
		ob.transform.localEulerAngles = new Vector3 (rotation.x + 90, rotation.y + 180, rotation.z);
		
		var normal = Quaternion.Euler(rotation) * Vector3.forward;
		tileX = (int)Mathf.Floor(position.x + normal.x / 2 + 0.5f);
		tileY = 0;
		tileZ = (int)Mathf.Floor(position.z + normal.z / 2 + 0.5f);
		plaque.size = new Vector2(2, 1);
		plaque.transform.localScale = new Vector3(0.2f, PLAQUE_HEIGHT, 2f);
		plaque.plaqueText = art.description;
		plaque.transform.localPosition = realPosition - new Vector3(0, PLAQUE_HEIGHT + frame.artHeight/2, 0);
		plaque.transform.localEulerAngles = rotation;
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

	public Selectable Select(Selectable.SelectionMode mode, Color color) {
		var selectable = ob.GetComponent<Selectable>();
		selectable.Selected = mode;
		selectable.OutlineColor = color;
		selectable.lineMode = Selectable.LineMode.Scale;
		return selectable;
	}
}

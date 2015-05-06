using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Contains all information of a particular tile. Generally this should only be used inside Museum.
/// </summary>
public class MuseumTile : MonoBehaviour, Storable<MuseumTile, MuseumTileData> {

    public int x, y, z;
    public bool left, right, front, back;
    public int ceilingStyle;
    public int wallStyle;
    public int floorStyle;
	public Color ceilingColor;
	public Color wallColor;
	public Color floorColor;
    public Material frontMaterial;
    public Material backMaterial;

    private static float HEIGHT = 3;
    private static float METER_PER_UNIT = 2;
    private static float UNIT_HEIGHT = HEIGHT / METER_PER_UNIT;

    private enum QuadType {
        Wall,
        Ceiling,
        Floor
    }

    private GameObject upObject, downObject, leftObject, rightObject, frontObject, backObject;

    public MuseumTileData Save() {
        return new MuseumTileData(x, y, z, left, right, front, back, ceilingStyle, wallStyle, floorStyle,
		                          wallColor, floorColor, ceilingColor);
    }

    public void Load(MuseumTileData data) {
        x = data.X;
        y = data.Y;
        z = data.Z;
        left = data.Left;
        right = data.Right;
        front = data.Front;
        back = data.Back;
        ceilingStyle = data.CeilingStyle;
        wallStyle = data.WallStyle;
        floorStyle = data.FloorStyle;
		if (data.CeilingColor != null) {
			ceilingColor = data.CeilingColor.ToColor ();
			wallColor = data.WallColor.ToColor ();
			floorColor = data.FloorColor.ToColor ();
		}
    }

    GameObject ReversedQuad() {
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.Rotate(new Vector3(0, 180, 0));
        return quad;
    }

    GameObject CreateFace(Vector3 localPosition, Vector3 scale, Vector3 angles, int id, QuadType quadType = QuadType.Floor){
        var ob = new GameObject();
        ob.transform.parent = gameObject.transform;
        GameObject frontSide = null;
        switch (quadType) {
            case QuadType.Ceiling:
                frontSide = GameObject.Instantiate(Catalog.GetCeiling(id));
				if(frontSide.GetComponent<Colorable>() != null) {
					frontSide.GetComponent<Colorable>().Color = ceilingColor;
				}
                frontSide.transform.Rotate(new Vector3(90, 0, 0));
                frontSide.transform.Translate(new Vector3(-0.5f, -1.5f, -0.5f));
                break;
            case QuadType.Floor:
                frontSide = GameObject.Instantiate(Catalog.GetFloor(id));
				if(frontSide.GetComponent<Colorable>() != null) {
					frontSide.GetComponent<Colorable>().Color = floorColor;
				}
                frontSide.transform.Rotate(new Vector3(-90, 0, 0));
                frontSide.transform.Translate(new Vector3(-0.5f, 0, -0.5f));
                break;
            case QuadType.Wall:
                frontSide = GameObject.Instantiate(Catalog.GetWall(id));
				if(frontSide.GetComponent<Colorable>() != null) {
					frontSide.GetComponent<Colorable>().Color = wallColor;
				}
                frontSide.transform.localScale = new Vector3(1, 2 / 3f, 1);
                frontSide.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                frontSide.transform.localPosition = new Vector3(0.5f, -0.5f, 0);
                break;
        }
        frontSide.transform.SetParent(ob.transform,false);
        if (quadType == QuadType.Wall) {
            Util.SetLayerRecursively(frontSide, LayerMask.NameToLayer("Walls"));
        }
        var backSide = ReversedQuad();
        backSide.transform.parent = ob.transform;
        backSide.GetComponent<MeshRenderer>().material = backMaterial;

        ob.transform.localPosition = localPosition;
        ob.transform.localScale = scale;
        ob.transform.Rotate(angles);
        return ob;
    }

	void Start () {
        transform.position = new Vector3(x, y, z);
        upObject = CreateFace(new Vector3(0, UNIT_HEIGHT, 0), new Vector3(1,1,1), new Vector3(-90, 0, 0), ceilingStyle, QuadType.Ceiling);
        downObject = CreateFace(new Vector3(0, 0, 0), new Vector3(1, 1, 1), new Vector3(90, 0, 0), floorStyle, QuadType.Floor);
        UpdateEdges();
	}

    public void Remove() {
        if (upObject != null) {
            Destroy(upObject);
        }
        if (downObject != null) {
            Destroy(downObject);
        }
        if (leftObject != null) {
            Destroy(leftObject);
        }
        if (rightObject != null){
            Destroy(rightObject);
        }
        if (frontObject != null){
            Destroy(frontObject);
        }
        if (backObject != null) {
            Destroy(backObject);
        }
    }

    /// <summary>
    /// Checks the neighbour booleans and creates or removes faces accordingly.
    /// </summary>
    public void UpdateEdges() {
        UpdateLeftEdge();
        UpdateRightEdge();
        UpdateFrontEdge();
        UpdateBackEdge();
    }

    void UpdateLeftEdge() {
        if (left && leftObject == null) {
            leftObject = CreateFace(new Vector3(-0.5f, UNIT_HEIGHT / 2, 0), new Vector3(1, UNIT_HEIGHT, 1), new Vector3(0, -90, 0), wallStyle, QuadType.Wall);
        }
        if (!left && leftObject != null) {
            Util.Destroy(leftObject);
            leftObject = null;
        }
    }

    void UpdateRightEdge() {
        if (right && rightObject == null) {
            rightObject = CreateFace(new Vector3(0.5f, UNIT_HEIGHT / 2, 0), new Vector3(1, UNIT_HEIGHT, 1), new Vector3(0, 90, 0), wallStyle, QuadType.Wall);
        }
        if (!right && rightObject != null) {
            Util.Destroy(rightObject);
            rightObject = null;
        }
    }

    void UpdateFrontEdge() {
        if (front && frontObject == null) {
            frontObject = CreateFace(new Vector3(0, UNIT_HEIGHT / 2, 0.5f), new Vector3(1, UNIT_HEIGHT, 1), new Vector3(0, 0, 0), wallStyle, QuadType.Wall);
        }
        if (!front && frontObject != null) {
            Util.Destroy(frontObject);
            frontObject = null;
        }
    }

    void UpdateBackEdge() {
        if (back && backObject == null) {
            backObject = CreateFace(new Vector3(0, UNIT_HEIGHT / 2, -0.5f), new Vector3(1, UNIT_HEIGHT, 1), new Vector3(0, 180, 0), wallStyle, QuadType.Wall);
        }
        if (!back && backObject != null) {
            Util.Destroy(backObject);
            backObject = null;
        }
    }
}

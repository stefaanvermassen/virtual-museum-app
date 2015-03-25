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
    public Material frontMaterial;
    public Material backMaterial;

    private static float HEIGHT = 3;
    private static float METER_PER_UNIT = 2;
    private static float UNIT_HEIGHT = HEIGHT / METER_PER_UNIT;

    private static Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();

    private GameObject upObject, downObject, leftObject, rightObject, frontObject, backObject;

    public MuseumTileData Save() {
        return new MuseumTileData(x, y, z, left, right, front, back, ceilingStyle, wallStyle, floorStyle);
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
    }

    GameObject ReversedQuad() {
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.Rotate(new Vector3(0, 180, 0));
        return quad;
    }

    GameObject FastResource(string resource) {
        if (!objects.ContainsKey(resource)) {
            objects.Add(resource, (GameObject) Resources.Load(resource));
        }
        return objects[resource];
    }

    GameObject CreateFace(Vector3 localPosition, Vector3 scale, Vector3 angles, string type){
        var ob = new GameObject();
        ob.transform.parent = gameObject.transform;
        var frontSide = (GameObject)GameObject.Instantiate(FastResource(type));
        frontSide.transform.parent = ob.transform;
        //frontSide.GetComponent<MeshRenderer>().material = frontMaterial;
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
        upObject = CreateFace(new Vector3(0, UNIT_HEIGHT, 0), new Vector3(1,1,1), new Vector3(-90, 0, 0), "CeilingQuad");
        downObject = CreateFace(new Vector3(0, 0, 0), new Vector3(1, 1, 1), new Vector3(90, 0, 0), "FloorQuad");
        UpdateEdges();
	}

    public void Remove() {
        if (upObject != null)       Destroy(upObject);
        if (downObject != null)     Destroy(downObject);
        if (leftObject != null)     Destroy(leftObject);
        if (rightObject != null)    Destroy(rightObject);
        if (frontObject != null)    Destroy(frontObject);
        if (backObject != null)     Destroy(backObject);
    }

    /// <summary>
    /// Checks the neighbour booleans and creates or removes faces accordingly.
    /// </summary>
    public void UpdateEdges() {
        if (left && leftObject == null) {
            leftObject = CreateFace(new Vector3(-0.5f, UNIT_HEIGHT / 2, 0), new Vector3(1, UNIT_HEIGHT, 1), new Vector3(0, -90, 0), "WallQuad");
        }
        if (!left && leftObject != null) {
			Util.Destroy(leftObject);
            leftObject = null;
        }
        if (right && rightObject == null) {
            rightObject = CreateFace(new Vector3(0.5f, UNIT_HEIGHT / 2, 0), new Vector3(1, UNIT_HEIGHT, 1), new Vector3(0, 90, 0), "WallQuad");
        }
        if (!right && rightObject != null) {
			Util.Destroy(rightObject);
            rightObject = null;
        }
        if (front && frontObject == null) {
            frontObject = CreateFace(new Vector3(0, UNIT_HEIGHT / 2, 0.5f), new Vector3(1, UNIT_HEIGHT, 1), new Vector3(0, 0, 0), "WallQuad");
        }
        if (!front && frontObject != null) {
			Util.Destroy(frontObject);
            frontObject = null;
        }
        if (back && backObject == null) {
            backObject = CreateFace(new Vector3(0, UNIT_HEIGHT / 2, -0.5f), new Vector3(1, UNIT_HEIGHT, 1), new Vector3(0, 180, 0), "WallQuad");
        }
        if (!back && backObject != null) {
			Util.Destroy(backObject);
            backObject = null;
        }
    }
}

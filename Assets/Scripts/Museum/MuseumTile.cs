using UnityEngine;
using System.Collections;

public class MuseumTile : MonoBehaviour, Storable<MuseumTile, MuseumTileData> {

    public int x, y, z;
    public bool left, right, front, back;
    public int ceilingStyle;
    public int wallStyle;
    public int floorStyle;
    public Material frontMaterial;
    public Material backMaterial;

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

    GameObject CreateFace(Vector3 localPosition, Vector3 angles){
        var ob = new GameObject();
        ob.transform.parent = gameObject.transform;
        var frontSide = GameObject.CreatePrimitive(PrimitiveType.Quad);
        frontSide.transform.parent = ob.transform;
        frontSide.GetComponent<MeshRenderer>().material = frontMaterial;
        var backSide = ReversedQuad();
        backSide.transform.parent = ob.transform;
        backSide.GetComponent<MeshRenderer>().material = backMaterial;

        ob.transform.localPosition = localPosition;
        ob.transform.Rotate(angles);
        return ob;
    }

	void Start () {
        transform.position = new Vector3(x, y, z);
        upObject = CreateFace(new Vector3(0, 0, 0), new Vector3(90, 0, 0));
        downObject = CreateFace(new Vector3(0, 1, 0), new Vector3(-90, 0, 0));
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

    public void UpdateEdges() {
        if (left && leftObject == null) {
            leftObject = CreateFace(new Vector3(-0.5f, 0.5f, 0), new Vector3(0, -90, 0));
        }
        if (!left && leftObject != null) {
            Destroy(leftObject);
            leftObject = null;
        }
        if (right && rightObject == null) {
            rightObject = CreateFace(new Vector3(0.5f, 0.5f, 0), new Vector3(0, 90, 0));
        }
        if (!right && rightObject != null) {
            Destroy(rightObject);
            rightObject = null;
        }
        if (front && frontObject == null) {
            frontObject = CreateFace(new Vector3(0, 0.5f, 0.5f), new Vector3(0, 0, 0));
        }
        if (!front && frontObject != null) {
            Destroy(frontObject);
            frontObject = null;
        }
        if (back && backObject == null) {
            backObject = CreateFace(new Vector3(0, 0.5f, -0.5f), new Vector3(0, 180, 0));
        }
        if (!back && backObject != null) {
            Destroy(backObject);
            backObject = null;
        }
    }
}

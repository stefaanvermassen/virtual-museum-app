using UnityEngine;
using System.Collections;

/// <summary>
/// Contains all information of a particular object already inside a museum. Generally this should only be used inside Museum.
/// </summary>
public class MuseumObject : MonoBehaviour, Storable<MuseumObject, MuseumObjectData> {

    public int x, y, z;
    public float angle;
    public int objectID;

    private GameObject ob;

    public MuseumObjectData Save() {
        return new MuseumObjectData(objectID, x, y, z, angle);
    }

    public void Load(MuseumObjectData data) {
        objectID = data.ObjectID;
        x = data.X;
        y = data.Y;
        z = data.Z;
        angle = data.Angle;
        Start();
    }

    public void Start() {
        Remove();
        var master = Catalog.GetObject(objectID);
        ob = (GameObject)Instantiate(master, new Vector3(x, y, z), Quaternion.Euler(new Vector3(0, angle, 0)));
    }

    public Selectable Select(Selectable.SelectionMode mode, Color color) {
        var selectable = ob.GetComponent<Selectable>();
        selectable.Selected = mode;
        selectable.OutlineColor = color;
        return selectable;
    }

    /// <summary>
    /// Should be called before destroying this GameObject.
    /// </summary>
    public void Remove() {
        if(ob != null) Destroy(ob);
    }

    public GameObject GetGameObject() {
        return ob;
    }

    public Vector3 GetPosition() {
        return new Vector3(x, y, z);
    }

    public void SetRotation(float angle) {
        ob.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
    }
}

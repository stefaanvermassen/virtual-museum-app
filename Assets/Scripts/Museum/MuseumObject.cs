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

    void Start() {
        Remove();
        var master = Resources.Load<GameObject>("Vase1");

        ob = (GameObject) Instantiate(master, new Vector3(x, y, z), Quaternion.Euler(new Vector3(0, angle, 0)));
    }

    /// <summary>
    /// Should be called before destroying this GameObject.
    /// </summary>
    public void Remove() {
        if(ob != null) Destroy(ob);
    }
}

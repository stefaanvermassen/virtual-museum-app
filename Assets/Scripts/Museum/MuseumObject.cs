using UnityEngine;
using System.Collections;

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
        var master = Resources.Load<GameObject>("texmonkey");

        ob = (GameObject) Instantiate(master, new Vector3(x, y + 0.5f, z), Quaternion.Euler(new Vector3(0, angle, 0)));
    }

    // Update is called once per frame
    void Update() {

    }

    public void Remove() {
        if(ob != null) Destroy(ob);
    }
}

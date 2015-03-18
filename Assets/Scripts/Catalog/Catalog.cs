using UnityEngine;
using System.Collections;

public static class Catalog {

    public static string[] objects = new string[] { "texmonkey", "Vase1" };

    public static GameObject GetObject(int objectID) {
        return Resources.Load<GameObject>("Objects/"+objects[objectID]);
    }

}
